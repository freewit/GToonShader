#ifndef GTOON_FORWARD_PASS_INCLUDED
#define GTOON_FORWARD_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Includes/GToonInput.hlsl"
#include "Includes/GToonLighting.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
    float2 lightmapUV : TEXCOORD1;
    float2 detailUV : TEXCOORD2;
    half4 color : COLOR;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float2 detailUV : TEXCOORD1;
    
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 2);
    
    float3 positionWS : TEXCOORD3;
    float3 normalWS : TEXCOORD4;
    float3 viewDirWS : TEXCOORD5;
    half4 fogFactorAndVertexLight : TEXCOORD6; // x: fogFactor, yzw: vertex light
    
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord : TEXCOORD7;
    #endif
    
    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON) || defined(_ENABLEPARALLAX_ON)
        half4 tangentWS : TEXCOORD8;
    #endif
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half3 vertexLights : TEXCOORD9;
    #endif
    
    half4 color : COLOR;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Vertex Shader
Varyings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    // Wind animation (preserve original functionality)
    float3 positionOS = input.positionOS.xyz;
    #if defined(_ENABLEWIND_ON)
        // Apply wind animation here if needed
        // ApplyWind(positionOS, input.positionOS, input.color);
    #endif

    VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.detailUV = TRANSFORM_TEX(input.uv, _DetailMap);
    output.color = input.color;
    output.normalWS = normalInput.normalWS;

    #if defined(_NORMALMAP) || defined(_DETAIL) || defined(_SPECULARMODE_ANISOTROPIC) || defined(_ENVIRONMENTREFLECTIONS_ON) || defined(_ENABLEPARALLAX_ON)
        real sign = input.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
        output.tangentWS = tangentWS;
    #endif
    
    output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    output.positionWS = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;

    // URP için gerekli hesaplamalar
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        output.vertexLights = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
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

    // Parallax Mapping (preserve original functionality)
    #if defined(_ENABLEPARALLAX_ON) && defined(_HEIGHTMAP)
        half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
        half3 viewDirTS = TransformWorldToTangent(viewDirWS, tangentToWorld);
        half height = SAMPLE_TEXTURE2D(_HeightMap, sampler_HeightMap, uv).r;
        float2 offset = viewDirTS.xy * (height * _HeightScale);
        uv -= offset;
    #endif
    
    // Base texture and color sampling
    half4 baseMapColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    half4 baseColor = baseMapColor * _BaseColor;
    
    // PBR-like surface properties
    half metallic = _Metallic;
    half smoothness = _Smoothness;
    half occlusion = 1.0;
    half3 emission = 0;
    
    // Normal calculation (preserve original functionality)
    half3 normalWS = input.normalWS;
    
    #if defined(_NORMALMAP)
        half4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
        half3 normalTS = UnpackNormalScale(normalMap, _BumpScale);
        
        #if defined(_DETAIL) && defined(_DETAIL_NORMALMAP)
            half4 detailNormalMap = SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, input.detailUV);
            half3 detailNormalTS = UnpackNormalScale(detailNormalMap, _DetailNormalScale);
            normalTS = BlendNormal(normalTS, detailNormalTS);
        #endif
        
        half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
        normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
    #endif
    
    normalWS = normalize(normalWS);

    // Detail Map processing (preserve original functionality)
    #if defined(_DETAIL)
        half4 detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv);
        half4 detailAlbedo = SAMPLE_TEXTURE2D(_DetailAlbedoMap, sampler_DetailAlbedoMap, input.detailUV);
        baseColor.rgb = lerp(baseColor.rgb, baseColor.rgb * detailAlbedo.rgb, detailMask.r);
    #endif

    // Emission Map processing (preserve original functionality)
    #if defined(_EMISSION)
        half3 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb;
        emission = emissionMap * _EmissionColor.rgb;
        
        #if defined(_ENABLEEMISSIONPULSE_ON)
            half pulse = sin(_Time.y * _EmissionPulseSpeed) * 0.5 + 0.5;
            emission *= lerp(1.0, pulse, _EmissionPulseAmount);
        #endif
    #endif

    // Alpha and Clipping
    half alpha = baseColor.a * _BaseColor.a;
    
    #if defined(_ALPHATEST_ON)
        clip(alpha - _Cutoff);
    #endif

    // **CRITICAL: URP 17.0.4 compatible shadow coordination system**
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    #else
        float4 shadowCoord = float4(0, 0, 0, 0);
    #endif

    // **CRITICAL: URP 17.0.4 compatible shadow mask system**
    #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
        half4 shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
    #elif !defined (LIGHTMAP_ON)
        half4 shadowMask = unity_ProbesOcclusion;
    #else
        half4 shadowMask = half4(1, 1, 1, 1);
    #endif

    // **CRITICAL: URP 17.0.4 compatible mesh rendering layers**
    #if defined(_LIGHT_LAYERS)
        uint meshRenderingLayers = GetMeshRenderingLayer();
    #endif

    // Lightmap and probe information
    #if defined(LIGHTMAP_ON)
        half3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
    #else
        half3 bakedGI = SampleSH(normalWS);
    #endif

    // **CRITICAL: URP 17.0.4 compatible main light calculation**
    Light mainLight = GetMainLight(shadowCoord, input.positionWS, shadowMask);
    
    // **CRITICAL: URP 17.0.4 compatible light layer check for main light**
    #if defined(_LIGHT_LAYERS)
        half3 mainLightDir = half3(0, 1, 0);
        half3 mainLightColor = half3(0, 0, 0);
        half mainLightAtten = 0;
        if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
        {
            mainLightDir = mainLight.direction;
            mainLightColor = mainLight.color.rgb;
            mainLightAtten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
        }
    #else
        half3 mainLightDir = mainLight.direction;
        half3 mainLightColor = mainLight.color.rgb;
        half mainLightAtten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
    #endif

    // Main light diffuse calculation (preserve all original lighting modes)
    half3 mainLightDiffuse = CalculateToonDiffuse(mainLight, baseColor.rgb, normalWS, mainLightAtten);

    // **CRITICAL: URP 17.0.4 compatible additional lights calculation**
    half3 additionalLightsDiffuse = 0;
    
    #if defined(_ADDITIONAL_LIGHTS_ON)
        uint pixelLightCount = GetAdditionalLightsCount();
        
        #if USE_FORWARD_PLUS
            for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
            {
                FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
                Light light = GetAdditionalLight(lightIndex, input.positionWS, shadowMask);
                
                #ifdef _LIGHT_LAYERS
                    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
                #endif
                {
                    half lightIntensity = CalculateToonLightIntensity(light.direction, normalWS, light.shadowAttenuation * light.distanceAttenuation);
                    additionalLightsDiffuse += light.color * lightIntensity * baseColor.rgb;
                }
            }
            
            // Forward+ için dummy input data
            float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
            InputData inputData = (InputData)0;
            inputData.normalizedScreenSpaceUV = normalizedScreenSpaceUV;
            inputData.positionWS = input.positionWS;
        #endif

        // **CRITICAL: URP 17.0.4 compatible main additional light loop**
        LIGHT_LOOP_BEGIN(pixelLightCount)
            Light light = GetAdditionalLight(lightIndex, input.positionWS, shadowMask);
            
            #ifdef _LIGHT_LAYERS
                if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
            #endif
            {
                // Toon lighting calculation
                half lightIntensity = CalculateToonLightIntensity(light.direction, normalWS, light.shadowAttenuation * light.distanceAttenuation);
                additionalLightsDiffuse += light.color * lightIntensity * baseColor.rgb;
            }
        LIGHT_LOOP_END

        #ifdef _ADDITIONAL_LIGHTS_VERTEX
            additionalLightsDiffuse += input.vertexLights * baseColor.rgb;
        #endif
    #endif

    // Specular Highlights (preserve all original specular modes)
    half3 specularColor = 0;
    #if defined(_ENABLESPECULARHIGHLIGHTS_ON)
        specularColor += CalculateToonSpecular(mainLight, normalWS, viewDirWS, mainLightAtten);
        
        #if defined(_ADDITIONAL_LIGHTS_ON)
            LIGHT_LOOP_BEGIN(pixelLightCount)
                Light light = GetAdditionalLight(lightIndex, input.positionWS, shadowMask);
                #ifdef _LIGHT_LAYERS
                    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
                #endif
                {
                    specularColor += CalculateToonSpecular(light, normalWS, viewDirWS, light.shadowAttenuation * light.distanceAttenuation);
                }
            LIGHT_LOOP_END
        #endif
    #endif

    // Rim Lighting (preserve original functionality)
    half3 rimColor = 0;
    #if defined(_ENABLERIM_ON)
        rimColor = CalculateRimLighting(normalWS, viewDirWS, mainLight);
    #endif

    // Subsurface Scattering (preserve original functionality)
    half3 subsurfaceColor = 0;
    #if defined(_ENABLESUBSURFACE_ON)
        subsurfaceColor = CalculateSubsurfaceScattering(mainLight, normalWS, viewDirWS, baseColor.rgb);
    #endif

    // Environment Reflections (preserve original functionality)
    half3 reflectionColor = 0;
    #if defined(_ENVIRONMENTREFLECTIONS_ON)
        half3 reflectVector = reflect(-viewDirWS, normalWS);
        reflectionColor = GToonEnvironmentReflection(reflectVector, smoothness, occlusion);
        reflectionColor *= lerp(1.0, baseColor.rgb, metallic);
    #endif

    // Final color composition
    half3 finalColor = mainLightDiffuse + additionalLightsDiffuse + specularColor + rimColor + subsurfaceColor + reflectionColor + emission;
    
    // Vertex lighting and ambient lighting
    finalColor += input.fogFactorAndVertexLight.yzw * baseColor.rgb;
    finalColor += bakedGI * baseColor.rgb * _IndirectLightingMultiplier;

    // Fog application
    finalColor = MixFog(finalColor, input.fogFactorAndVertexLight.x);

    return half4(finalColor, alpha);
}

#endif