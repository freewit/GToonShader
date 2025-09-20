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
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON)
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
    
    // Rüzgar animasyonunu uygula
    ApplyWind(positionOS, input.positionOS);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.normalWS = normalInput.normalWS;
    
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON)
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
    
    // Temel doku ve renk örneklemesi
    half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    half4 baseColor = baseMapColor * _BaseColor;
    
    // PBR-like yüzey özellikleri
    half oneMinusReflectivity = 1.0 - kDielectricSpec.r;
    half3 albedo = baseColor.rgb * (1.0 - _Metallic * oneMinusReflectivity);
    half3 specColor = lerp(kDielectricSpec.rgb, baseColor.rgb, _Metallic);

    // Normal map hesaplamaları
    half3 normalWS = normalize(input.normalWS);
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON)
        half3 normalTS = half3(0,0,1);
        #if defined(_NORMALMAP)
            normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _NormalStrength);
        #endif
        #if defined(_DETAIL)
            half3 detailNormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw), 1.0);
            #if defined(_NORMALMAP)
                 normalTS = BlendNormal(normalTS, detailNormalTS);
            #else
                 normalTS = detailNormalTS;
            #endif
        #endif
        #if defined(_NORMALMAP) || defined(_DETAIL) // Only create matrix if we actually have a tangent space normal
            half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
            normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
        #endif
    #endif

    // Detail albedo
    #if defined(_DETAIL)
        half4 detailColor = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw);
        albedo = lerp(albedo, albedo * detailColor.rgb * 2, _DetailStrength);
    #endif
    
    half3 viewDirWS = normalize(input.viewDirWS);
    
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

    // === AYDINLATMA HESAPLAMALARI BAŞLANGICI (YENİ MANTIK) ===

    // 1. Toon Diffuse ve GI hesaplaması (Temel Işık)
    half3 diffuseComponent = CalculateToonDiffuse(mainLight, albedo, normalWS, shadowAttenuation);
    diffuseComponent += SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS) * albedo * _LightmapInfluence * _OcclusionStrength;

    // 2. Çevre Yansımaları (Environment Reflections)
    half3 specularComponent = half3(0,0,0);
    #if defined(_ENVIRONMENTREFLECTIONS_ON)
        half perceptualRoughness = 1.0h - _Smoothness;
        half3 reflectVec = reflect(-viewDirWS, normalWS);
        half occlusion = 1.0h;
        half3 reflection = GlossyEnvironmentReflection(reflectVec, perceptualRoughness, occlusion);
        specularComponent = reflection * _EnvironmentReflections * specColor;
    #endif

    // 3. Fresnel ile Diffuse ve Specular birleştirme
    half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 5.0);
    half fresnelTerm = lerp(fresnel, 1.0h, _Metallic); // Metalik ise her yerden, değilse kenarlardan yansıt
    
    // Metalik olmayan yüzeyler için yansımayı albedo rengiyle karıştır, metalik yüzeyler için olduğu gibi bırak.
    // Bu, "beyaz plastik" gibi malzemelerin doğru görünmesini sağlar.
    half3 surfaceColor = lerp(diffuseComponent, specularComponent, fresnelTerm);

    // 4. Final Rengi oluşturmaya başla
    half3 finalColor = surfaceColor;

    // 5. Eklemeli Efektler (Additive Effects)
    // Toon specular artık yansımaların üzerine ekleniyor, böylece kaybolmuyor.
    #if defined(_SPECULARHIGHLIGHTS_ON)
        #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON)
            half4 tangentWS = input.tangentWS;
        #else
            half4 tangentWS = half4(1,0,0,1); // Dummy data
        #endif
        finalColor += CalculateSpecular(normalWS, mainLight.direction, viewDirWS, mainLight.color, shadowAttenuation, tangentWS, input.uv);
    #endif

    finalColor += CalculateRimLighting(normalWS, viewDirWS, mainLight.direction, mainLight.color, input.uv);
    finalColor += CalculateSubsurface(normalWS, viewDirWS, mainLight);

    // Emission
    #if defined(_EMISSION)
        half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb * _EmissionIntensity;
        finalColor += emission;
    #endif
    
    // Sis
    finalColor = MixFog(finalColor, input.fogFactorAndVertexLight.x);
    
    return half4(finalColor, baseColor.a);
}

#endif // GTOON_FORWARD_PASS_INCLUDED

