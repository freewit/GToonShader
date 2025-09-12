Shader "Gorgonize/Gorgonize Toon Shader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        
        [Header(Lighting System)]
        [KeywordEnum(Stepped, Smooth, Ramp)] _LightingMode ("Lighting Mode", Float) = 0
        _ShadowSteps ("Shadow Steps", Range(2, 8)) = 3
        _ShadowSmoothness ("Shadow Smoothness", Range(0, 1)) = 0.1
        _ShadowRamp ("Shadow Ramp", 2D) = "white" {} [NoScaleOffset]
        
        [Header(Shadow Configuration)]
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.8, 1)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.5
        _ShadowOffset ("Shadow Offset", Range(-1, 1)) = 0
        _OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
        
        [Header(Highlight System)]
        [Toggle] _EnableHighlights ("Enable Highlights", Float) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize ("Specular Size", Range(0, 1)) = 0.1
        _SpecularSmoothness ("Specular Smoothness", Range(0, 1)) = 0.5
        _SpecularSteps ("Specular Steps", Range(1, 8)) = 2
        
        
        [Header(Rim Lighting)]
        [Toggle] _EnableRim ("Enable Rim Lighting", Float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0, 10)) = 2
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1
        _RimOffset ("Rim Offset", Range(-1, 1)) = 0
        
        [Header(Advanced Features)]
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1
        _EmissionMap ("Emission", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 0
        
        [Header(Detail Textures)]
        _DetailMap ("Detail Albedo", 2D) = "gray" {}
        _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailStrength ("Detail Strength", Range(0, 2)) = 1
        
        [Header(Subsurface Scattering)]
        [Toggle] _EnableSubsurface ("Enable Subsurface", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.4, 0.25, 1)
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 0
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 1
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 1
        
        [Header(Outline)]
        [Toggle] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        
        [Header(Wind Animation)]
        [Toggle] _EnableWind ("Enable Wind", Float) = 0
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.1
        _WindDirection ("Wind Direction", Vector) = (1, 0, 1, 0)
        
        [Header(Performance)]
        [Toggle] _ReceiveShadows ("Receive Shadows", Float) = 1
        [Toggle] _EnableAdditionalLights ("Additional Lights", Float) = 1
        _LightmapInfluence ("Lightmap Influence", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "ToonForward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            // Feature keywords
            #pragma shader_feature_local _ENABLEHIGHLIGHTS_ON
            #pragma shader_feature_local _ENABLERIM_ON
            #pragma shader_feature_local _ENABLESUBSURFACE_ON
            #pragma shader_feature_local _LIGHTINGMODE_STEPPED _LIGHTINGMODE_SMOOTH _LIGHTINGMODE_RAMP
            #pragma shader_feature_local _ENABLEOUTLINE_ON
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _RECEIVESHADOWS_ON
            #pragma shader_feature_local _ENABLEADDITIONALLIGHTS_ON
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _DETAIL
            
            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            // Manual LerpWhiteTo fonksiyonu tanımları - URP 17.0.4 uyumluluğu için
            half LerpWhiteTo(half b, half t)
            {
                half oneMinusT = 1.0 - t;
                return oneMinusT + b * t;
            }

            half3 LerpWhiteTo(half3 b, half t)
            {
                half oneMinusT = 1.0 - t;
                return half3(oneMinusT, oneMinusT, oneMinusT) + b * t;
            }

            half4 LerpWhiteTo(half4 b, half t)
            {
                half oneMinusT = 1.0 - t;
                return half4(oneMinusT, oneMinusT, oneMinusT, oneMinusT) + b * t;
            }
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float2 lightmapUV   : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS               : SV_POSITION;
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                float3 tangentWS                : TEXCOORD3;
                float3 bitangentWS              : TEXCOORD4;
                float3 viewDirWS                : TEXCOORD5;
                float4 shadowCoord              : TEXCOORD6;
                float distanceToCamera          : TEXCOORD7;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 8);
                float fogCoord                  : TEXCOORD9;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            // Textures
            TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_ShadowRamp);         SAMPLER(sampler_ShadowRamp);
            TEXTURE2D(_NormalMap);          SAMPLER(sampler_NormalMap);
            TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);
            TEXTURE2D(_DetailMap);          SAMPLER(sampler_DetailMap);
            TEXTURE2D(_DetailNormalMap);    SAMPLER(sampler_DetailNormalMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _DetailMap_ST;
                half4 _BaseColor;
                
                // Lighting
                half _ShadowSteps;
                half _ShadowSmoothness;
                half4 _ShadowColor;
                half _ShadowIntensity;
                half _ShadowOffset;
                half _OcclusionStrength;
                
                // Specular
                half4 _SpecularColor;
                half _SpecularSize;
                half _SpecularSmoothness;
                half _SpecularSteps;
                
                // Rim
                half4 _RimColor;
                half _RimPower;
                half _RimIntensity;
                half _RimOffset;
                
                // Advanced
                half _NormalStrength;
                half4 _EmissionColor;
                half _EmissionIntensity;
                half _DetailStrength;
                
                // Subsurface
                half4 _SubsurfaceColor;
                half _SubsurfaceIntensity;
                half _SubsurfaceDistortion;
                half _SubsurfacePower;
                
                // Outline
                half4 _OutlineColor;
                half _OutlineWidth;
                
                // Wind
                half _WindSpeed;
                half _WindStrength;
                float4 _WindDirection;
                
                // Performance
                half _LightmapInfluence;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                // Wind animation
                #ifdef _ENABLEWIND_ON
                float3 worldPos = positionInputs.positionWS;
                float windPhase = dot(worldPos, _WindDirection.xyz) * 0.1 + _Time.y * _WindSpeed;
                float windNoise = sin(windPhase) * 0.5 + 0.5;
                float3 windOffset = normalInputs.normalWS * windNoise * _WindStrength;
                positionInputs.positionWS += windOffset;
                positionInputs.positionCS = TransformWorldToHClip(positionInputs.positionWS);
                #endif
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = normalInputs.tangentWS;
                output.bitangentWS = normalInputs.bitangentWS;
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.distanceToCamera = distance(_WorldSpaceCameraPos.xyz, positionInputs.positionWS);
                
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
                
                #ifdef _RECEIVESHADOWS_ON
                output.shadowCoord = GetShadowCoord(positionInputs);
                #endif
                
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                return output;
            }
            
            // Enhanced shadow sampling with PCF
            half SampleShadowWithPCF(float4 shadowCoord, float3 worldPos)
            {
                #if defined(_MAIN_LIGHT_SHADOWS) && defined(_RECEIVESHADOWS_ON)
                    // Calculate shadow map texel size for PCF
                    float shadowMapSize = _MainLightShadowmapSize.x;
                    float texelSize = 1.0 / shadowMapSize;
                    
                    half shadowAttenuation = 0;
                    // 5x5 PCF sampling for smoother shadows
                    [unroll]
                    for(int x = -2; x <= 2; x++)
                    {
                        [unroll]
                        for(int y = -2; y <= 2; y++)
                        {
                            float2 offset = float2(x, y) * texelSize;
                            float4 sampleCoord = shadowCoord;
                            sampleCoord.xy += offset;
                            shadowAttenuation += MainLightRealtimeShadow(sampleCoord);
                        }
                    }
                    shadowAttenuation /= 25.0; // Average of 25 samples
                    
                    return shadowAttenuation;
                #else
                    return 1.0;
                #endif
            }
            
            // Advanced toon lighting calculation with enhanced shadow handling
            half3 CalculateToonLighting(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir, half shadowAttenuation, float3 worldPos)
            {
                half NdotL = dot(normal, lightDir);
                half rawNdotL = NdotL * 0.5 + 0.5; // Remap to 0-1
                rawNdotL += _ShadowOffset;
                rawNdotL = saturate(rawNdotL);
                
                half toonLighting = 0;
                
                #if defined(_LIGHTINGMODE_STEPPED)
                    // Stepped lighting
                    toonLighting = floor(rawNdotL * _ShadowSteps) / _ShadowSteps;
                    if(_ShadowSmoothness > 0.01)
                    {
                        half stepSize = 1.0 / _ShadowSteps;
                        for(int i = 1; i < _ShadowSteps; i++)
                        {
                            half stepThreshold = i * stepSize;
                            half smoothStep = smoothstep(stepThreshold - _ShadowSmoothness * 0.5, 
                                                       stepThreshold + _ShadowSmoothness * 0.5, rawNdotL);
                            toonLighting = lerp(toonLighting, stepThreshold, smoothStep);
                        }
                    }
                    
                #elif defined(_LIGHTINGMODE_SMOOTH)
                    // Smooth toon lighting
                    toonLighting = smoothstep(0.1, 0.9, rawNdotL);
                    if(_ShadowSmoothness < 0.99)
                    {
                        half stepped = floor(rawNdotL * _ShadowSteps) / _ShadowSteps;
                        toonLighting = lerp(stepped, toonLighting, _ShadowSmoothness);
                    }
                    
                #elif defined(_LIGHTINGMODE_RAMP)
                    // Ramp-based lighting
                    half3 rampSample = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, half2(rawNdotL, 0.5)).rgb;
                    toonLighting = dot(rampSample, half3(0.299, 0.587, 0.114)); // Luminance
                #endif
                
                // Apply enhanced shadow with PCF
                #ifdef _RECEIVESHADOWS_ON
                toonLighting *= shadowAttenuation;
                #endif
                
                // Apply shadow color using lerp instead of LerpWhiteTo
                half3 shadowTint = lerp(_ShadowColor.rgb, half3(1,1,1), toonLighting);
                shadowTint = lerp(half3(1,1,1), shadowTint, _ShadowIntensity);
                
                return lightColor * toonLighting * shadowTint;
            }
            
            // Toon specular highlights
            half3 CalculateToonSpecular(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir)
            {
                half3 halfDir = normalize(lightDir + viewDir);
                half NdotH = saturate(dot(normal, halfDir));
                
                half specular = pow(NdotH, (1.0 - _SpecularSize) * 100 + 1);
                // Toon-ify specular
                specular = floor(specular * _SpecularSteps) / _SpecularSteps;
                if(_SpecularSmoothness > 0.01)
                {
                    half smoothSpec = pow(NdotH, (1.0 - _SpecularSize) * 100 + 1);
                    specular = lerp(specular, smoothSpec, _SpecularSmoothness);
                }
                
                return lightColor * specular * _SpecularColor.rgb;
            }
            
            // Rim lighting
            half3 CalculateRimLight(half3 normal, half3 viewDir, half3 lightColor)
            {
                half rim = 1.0 - saturate(dot(normal, viewDir));
                rim += _RimOffset;
                rim = saturate(rim);
                rim = pow(rim, _RimPower);
                
                return lightColor * rim * _RimColor.rgb * _RimIntensity;
            }
            
            // Subsurface scattering
            half3 CalculateSubsurface(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir)
            {
                half3 scatterDir = lightDir + normal * _SubsurfaceDistortion;
                half scatter = pow(saturate(dot(viewDir, -scatterDir)), _SubsurfacePower);
                return lightColor * scatter * _SubsurfaceColor.rgb * _SubsurfaceIntensity;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Sample base textures
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                // Detail textures
                #ifdef _DETAIL
                half4 detailColor = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw);
                baseColor.rgb = lerp(baseColor.rgb, baseColor.rgb * detailColor.rgb * 2, _DetailStrength);
                #endif
                
                // Normal mapping
                half3 normalWS = input.normalWS;
                #ifdef _NORMALMAP
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _NormalStrength);
                #ifdef _DETAIL
                half3 detailNormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw), _DetailStrength);
                normalTS = BlendNormal(normalTS, detailNormalTS);
                #endif
                half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                #endif
                normalWS = normalize(normalWS);
                half3 viewDirWS = normalize(input.viewDirWS);
                
                // Enhanced shadow sampling
                half shadowAttenuation = SampleShadowWithPCF(input.shadowCoord, input.positionWS);
                // Main lighting with enhanced shadows
                Light mainLight = GetMainLight();
                mainLight.shadowAttenuation = shadowAttenuation;
                
                half3 diffuseColor = CalculateToonLighting(mainLight.color, mainLight.direction, normalWS, viewDirWS, shadowAttenuation, input.positionWS);
                
                half3 specularColor = half3(0,0,0);
                #if defined(_ENABLEHIGHLIGHTS_ON)
                specularColor = CalculateToonSpecular(mainLight.color, mainLight.direction, normalWS, viewDirWS);
                #endif
                
                half3 rimColor = half3(0,0,0);
                #if defined(_ENABLERIM_ON)
                rimColor = CalculateRimLight(normalWS, viewDirWS, mainLight.color);
                #endif

                half3 subsurfaceColor = half3(0,0,0);
                #if defined(_ENABLESUBSURFACE_ON)
                subsurfaceColor = CalculateSubsurface(mainLight.color, mainLight.direction, normalWS, viewDirWS);
                #endif

                // Additional lights
                #ifdef _ENABLEADDITIONALLIGHTS_ON
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    diffuseColor += CalculateToonLighting(light.color, light.direction, normalWS, viewDirWS, light.shadowAttenuation, input.positionWS);
                    #if defined(_ENABLEHIGHLIGHTS_ON)
                    specularColor += CalculateToonSpecular(light.color, light.direction, normalWS, viewDirWS);
                    #endif
                    #if defined(_ENABLERIM_ON)
                    rimColor += CalculateRimLight(normalWS, viewDirWS, light.color);
                    #endif
                }
                #endif
                
                // Ambient lighting
                half3 ambient = SampleSH(normalWS);
                #ifdef LIGHTMAP_ON
                ambient = SampleLightmap(input.lightmapUV, normalWS) * _LightmapInfluence + ambient * (1 - _LightmapInfluence);
                #endif
                
                // Combine lighting
                half3 color = baseColor.rgb * (diffuseColor + ambient * 0.3) + specularColor + rimColor + subsurfaceColor;
                // Emission
                #ifdef _EMISSION
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb * _EmissionIntensity;
                color += emission;
                #endif
                
                // Apply fog
                color = MixFog(color, input.fogCoord);
                return half4(color, baseColor.a);
            }
            ENDHLSL
        }
        
        // Enhanced Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            
            // LerpWhiteTo fonksiyonu tanımları - include'lardan ÖNCE
            half LerpWhiteTo(half b, half t)
            {
                half oneMinusT = 1.0 - t;
                return oneMinusT + b * t;
            }

            half3 LerpWhiteTo(half3 b, half t)
            {
                half oneMinusT = 1.0 - t;
                return half3(oneMinusT, oneMinusT, oneMinusT) + b * t;
            }

            half4 LerpWhiteTo(half4 b, half t)
            {
                half oneMinusT = 1.0 - t;
                return half4(oneMinusT, oneMinusT, oneMinusT, oneMinusT) + b * t;
            }
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 texcoord     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END
            
            float3 _LightDirection;
            float3 _LightPosition;
            
            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(positionWS);
                return output;
            }
            
            half4 ShadowPassFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return 0;
            }
            ENDHLSL
        }
        
        // Depth pass
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}
            ZWrite On
            ColorMask 0
            Cull[_Cull]
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
        
        // Outline pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            ZTest LEqual
            ZWrite On
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #pragma shader_feature_local _ENABLEOUTLINE_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct AttributesOutline
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct VaryingsOutline
            {
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                half _OutlineWidth;
            CBUFFER_END
            
            VaryingsOutline vertOutline(AttributesOutline input)
            {
                VaryingsOutline output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #if defined(_ENABLEOUTLINE_ON)
                    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                    
                    positionWS += normalWS * _OutlineWidth * 0.01;
                    output.positionCS = TransformWorldToHClip(positionWS);
                #else
                    output.positionCS = float4(0,0,0,0);
                #endif
                
                return output;
            }
            
            half4 fragOutline(VaryingsOutline input) : SV_Target
            {
                #if defined(_ENABLEOUTLINE_ON)
                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                    return _OutlineColor;
                #else
                    discard;
                    return half4(0,0,0,0);
                #endif
            }
            ENDHLSL
        }
    }
    
    CustomEditor "Gorgonize.ToonShader.Editor.AdvancedToonShaderGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

