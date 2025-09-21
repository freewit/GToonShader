#ifndef GTOON_FORWARD_PASS_INCLUDED
#define GTOON_FORWARD_PASS_INCLUDED

// Bu dosya, ana render pass'i olan UniversalForward için vertex ve fragment shader'ları içerir.
// Diğer HLSL dosyalarındaki fonksiyonları ve verileri kullanarak nihai rengi oluşturur.

#include "Includes/GToonInput.hlsl"
#include "Includes/GToonWind.hlsl"
#include "Includes/GToonLighting.hlsl"

// Vertex shader'a giren mesh verileri
struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float2 lightmapUV   : TEXCOORD1;
    float4 color        : COLOR; // Vertex color maskelemesi için eklendi
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Vertex shader'dan fragment shader'a aktarılan veriler
struct Varyings
{
    float4 positionCS               : SV_POSITION;
    float2 uv                       : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
    float3 positionWS               : TEXCOORD2;
    float3 normalWS                 : TEXCOORD3;
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON) || defined(_ENABLEPARALLAX_ON)
        half4 tangentWS                 : TEXCOORD4;
    #endif
    float3 viewDirWS                : TEXCOORD5;
    half4 fogFactorAndVertexLight   : TEXCOORD6;
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord              : TEXCOORD7;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


// Ana Vertex Shader
Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    float3 positionOS = input.positionOS.xyz;
    
    // Rüzgar animasyonunu uygula (3 parametreli doğru çağrı)
    ApplyWind(positionOS, input.positionOS, input.color);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.normalWS = normalInput.normalWS;
    
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON) || defined(_ENABLEPARALLAX_ON)
        real sign = input.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
        output.tangentWS = tangentWS;
    #endif
    
    output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    output.positionWS = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;

    // URP için gerekli hesaplamalar (sis, vertex lighting, lightmap, gölge koordinatları)
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    return output;
}

// Ana Fragment Shader
half4 frag(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half3 viewDirWS = normalize(input.viewDirWS);
    float2 uv = input.uv;

    // Parallax Mapping
    #if defined(_ENABLEPARALLAX_ON) && defined(_HEIGHTMAP)
        half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
        half3 viewDirTS = TransformWorldToTangent(viewDirWS, tangentToWorld);
        half height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, uv).r;
        float2 offset = viewDirTS.xy * (height * _HeightScale);
        uv -= offset;
    #endif
    
    // Temel doku ve renk örneklemesi
    half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    half4 baseColor = baseMapColor * _BaseColor;
    
    // PBR-like yüzey özellikleri
    half oneMinusReflectivity = 1.0 - kDielectricSpec.r;
    half3 albedo = baseColor.rgb * (1.0 - _Metallic * oneMinusReflectivity);
    half3 specColor = lerp(kDielectricSpec.rgb, baseColor.rgb, _Metallic);

    // Normal map hesaplamaları
    half3 normalWS = normalize(input.normalWS);
    #if defined(_NORMALMAP) || defined(_DETAIL)
        half3 normalTS = half3(0,0,1);
        #if defined(_NORMALMAP)
            normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv), _NormalStrength);
        #endif
        #if defined(_DETAIL)
            half3 detailNormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, uv * _DetailMap_ST.xy + _DetailMap_ST.zw), _DetailNormalScale);
            #if defined(_NORMALMAP)
                 normalTS = BlendNormal(normalTS, detailNormalTS);
            #else
                 normalTS = detailNormalTS;
            #endif
        #endif
        half3x3 tangentToWorld_Normal = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
        normalWS = TransformTangentToWorld(normalTS, tangentToWorld_Normal);
    #endif

    // Detail albedo
    #if defined(_DETAIL)
        half4 detailColor = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, uv * _DetailMap_ST.xy + _DetailMap_ST.zw);
        albedo = lerp(albedo, albedo * detailColor.rgb * 2, _DetailStrength);
    #endif
    
    // Gölge koordinatlarını al
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    #else
        float4 shadowCoord = float4(0, 0, 0, 0);
    #endif

    // Ana ışık ve gölge bilgileri
    Light mainLight = GetMainLight(shadowCoord);
    half shadowAttenuation = mainLight.shadowAttenuation;

    // === AYDINLATMA HESAPLAMALARI BAŞLANGICI ===

    // 1. Toon Diffuse (Gölge/Işık) ve Baked GI hesaplaması
    half3 diffuseLighting = CalculateToonDiffuse(mainLight, albedo, normalWS, shadowAttenuation);
    diffuseLighting += SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS) * albedo * _LightmapInfluence * _OcclusionStrength;
    
    // 2. Yansıma ve Parlama Hesaplamaları
    half3 specularLighting = half3(0,0,0);

    // 2a. Çevre Yansımaları (Environment Reflections)
    #if defined(_ENVIRONMENTREFLECTIONS_ON)
        half perceptualRoughness = 1.0h - _Smoothness;
        half3 reflectVec = reflect(-viewDirWS, normalWS);
        half occlusion = 1.0h;
        half3 reflection = GlossyEnvironmentReflection(reflectVec, perceptualRoughness, occlusion);
        
        half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 5.0);
        half fresnelTerm = lerp(fresnel, 1.0h, _Metallic); 

        specularLighting += reflection * specColor * fresnelTerm * _EnvironmentReflections;
    #endif

    // 2b. Toon Specular (Stilize Parlama)
    #if defined(_ENABLESPECULARHIGHLIGHTS_ON)
        #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON) || defined(_ENABLEPARALLAX_ON)
            half4 tangentWS = input.tangentWS;
        #else
            half4 tangentWS = half4(1,0,0,1);
        #endif
        specularLighting += CalculateSpecular(normalWS, mainLight.direction, viewDirWS, mainLight.color, shadowAttenuation, tangentWS, uv);
    #endif

    // 3. Diğer Efektler
    half3 rimColor = CalculateRimLighting(normalWS, viewDirWS, mainLight.direction, mainLight.color, uv);
    half3 subsurfaceColor = CalculateSubsurface(normalWS, viewDirWS, mainLight, uv);
    
    // 4. Final Renk Birleştirme
    half3 finalColor = diffuseLighting + specularLighting + rimColor + subsurfaceColor;

    // Emission
    #if defined(_EMISSION)
        half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb * _EmissionColor.rgb;
        #if defined(_ENABLEEMISSIONPULSE_ON)
            emission *= (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) + 0.1; // Add 0.1 to prevent it from going completely black
        #endif
        finalColor += emission * _EmissionIntensity;
    #endif
    
    // Sis
    finalColor = MixFog(finalColor, input.fogFactorAndVertexLight.x);
    
    return half4(finalColor, baseColor.a);
}

#endif // GTOON_FORWARD_PASS_INCLUDED

