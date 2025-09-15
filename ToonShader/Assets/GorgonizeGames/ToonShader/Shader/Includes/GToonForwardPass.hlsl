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
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC)
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
    
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC)
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
    half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
    
    // Normal map hesaplamaları
    half3 normalWS = normalize(input.normalWS);
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC)
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
        half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
        normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
    #endif

    // Detail albedo
    #if defined(_DETAIL)
        half4 detailColor = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw);
        baseColor.rgb = lerp(baseColor.rgb, baseColor.rgb * detailColor.rgb * 2, _DetailStrength);
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

    // AYDINLATMA MANTIĞI
    half3 mainLightDiffuse = CalculateToonDiffuse(mainLight, baseColor.rgb, normalWS, shadowAttenuation);
    
    // Renkleri birleştirmeye başla
    half3 finalColor = mainLightDiffuse;
    
    // Ek ışıklar (Additional Lights)
    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
        {
            Light light = GetAdditionalLight(lightIndex, input.positionWS);
            half atten = light.shadowAttenuation * light.distanceAttenuation;
            
            // Ek ışıklar her zaman daha basit bir aydınlatma kullanır, Ramp modu burada geçerli olmaz
            half additionalLightIntensity = CalculateToonLightIntensity(light.direction, normalWS, atten);
            finalColor += baseColor.rgb * light.color * additionalLightIntensity;
        }
    #endif

    // Eklemeli efektler
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC)
        half4 tangentWS = input.tangentWS;
    #else
        half4 tangentWS = half4(1,0,0,1); // Anisotropic için dummy veri
    #endif
    
    half3 specularColor = CalculateSpecular(normalWS, mainLight.direction, viewDirWS, mainLight.color, shadowAttenuation, tangentWS, input.uv);
    half3 rimColor = CalculateRimLighting(normalWS, viewDirWS, mainLight.direction, mainLight.color, input.uv);
    half3 subsurfaceColor = CalculateSubsurface(normalWS, viewDirWS, mainLight);

    // Lightmap / Baked GI
    half3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
    
    // Tüm aydınlatma bileşenlerini birleştir
    finalColor += specularColor;
    finalColor += rimColor;
    finalColor += subsurfaceColor;
    finalColor += bakedGI * baseColor.rgb * _LightmapInfluence * _OcclusionStrength;
    
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

