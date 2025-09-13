Shader "Gorgonize/Gorgonize Toon Shader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        
        [Header(Shadow System)]
        [KeywordEnum(Stepped, Smooth, Ramp)] _LightingMode ("Lighting Mode", Float) = 0
        [Toggle(_TINT_SHADOW_ON_BASE)] _TintShadowOnBase ("Tint On Full Object", Float) = 0
        _ShadowSteps ("Shadow Steps", Range(2, 8)) = 3
        _ShadowSmoothness ("Shadow Smoothness", Range(0, 1)) = 0.1
        _ShadowRamp ("Shadow Ramp", 2D) = "white" {} [NoScaleOffset]
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
            #pragma shader_feature_local _TINT_SHADOW_ON_BASE
            #pragma shader_feature_local _ENABLEOUTLINE_ON
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _RECEIVESHADOWS_ON
            #pragma shader_feature_local _ENABLEADDITIONALLIGHTS_ON
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _DETAIL
            
            // URP keywords - with SHADOW FIX
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                half _LightingMode;
                half _TintShadowOnBase;
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
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    float4 shadowCoord          : TEXCOORD6;
                #endif
                float distanceToCamera          : TEXCOORD7;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 8);
                half fogFactor                  : TEXCOORD9;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                // Wind animation
                #if defined(_ENABLEWIND_ON)
                float3 worldPos = positionInputs.positionWS;
                float windPhase = (_Time.y * _WindSpeed) + (worldPos.x + worldPos.z) * 0.1;
                float3 windOffset = _WindDirection.xyz * sin(windPhase) * _WindStrength;
                positionInputs.positionWS += windOffset * input.positionOS.y; // vertex height affects wind
                positionInputs.positionCS = TransformWorldToHClip(positionInputs.positionWS);
                #endif
                
                output.positionCS = positionInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = normalInputs.tangentWS;
                output.bitangentWS = normalInputs.bitangentWS;
                output.viewDirWS = GetCameraPositionWS() - positionInputs.positionWS;
                output.distanceToCamera = distance(_WorldSpaceCameraPos, positionInputs.positionWS);

                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS, output.vertexSH);
                
                // SHADOW FIX - New URP shadow coordinate system
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = GetShadowCoord(positionInputs);
                #endif
                
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }

            // Toon lighting calculation - enhanced
            half3 CalculateToonLighting(half3 lightColor, half3 lightDir, half3 normalWS, half3 viewDirWS, half shadowAttenuation, float3 positionWS)
            {
                half NdotL = dot(normalWS, lightDir);
                half lightIntensity = saturate(NdotL) * shadowAttenuation;
                
                // Apply shadow offset
                lightIntensity = saturate(lightIntensity + _ShadowOffset);
                
                half3 diffuse;
                
                // Lighting modes
                #if defined(_LIGHTINGMODE_STEPPED)
                    // Stepped lighting
                    lightIntensity = round(lightIntensity * _ShadowSteps) / _ShadowSteps;
                    diffuse = lerp(_ShadowColor.rgb, lightColor, lightIntensity * _ShadowIntensity);
                    
                #elif defined(_LIGHTINGMODE_RAMP)
                    // Ramp texture lighting
                    half2 rampUV = half2(lightIntensity, 0.5);
                    half3 ramp = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, rampUV).rgb;
                    diffuse = lightColor * ramp * _ShadowIntensity;
                    
                #else
                    // Smooth lighting (default)
                    lightIntensity = smoothstep(0.0, _ShadowSmoothness, lightIntensity);
                    diffuse = lerp(_ShadowColor.rgb, lightColor, lightIntensity * _ShadowIntensity);
                #endif
                
                return diffuse;
            }

            // Enhanced specular calculation
            half3 CalculateSpecular(half3 lightColor, half3 lightDir, half3 normalWS, half3 viewDirWS, half shadowAttenuation)
            {
                half3 halfDir = normalize(lightDir + viewDirWS);
                half NdotH = saturate(dot(normalWS, halfDir));
                half spec = pow(NdotH, (1.0 - _SpecularSize) * 128.0);
                
                #if defined(_LIGHTINGMODE_STEPPED)
                    spec = round(spec * _SpecularSteps) / _SpecularSteps;
                #else
                    spec = smoothstep(0.5 - _SpecularSmoothness, 0.5 + _SpecularSmoothness, spec);
                #endif
                
                return _SpecularColor.rgb * lightColor * spec * shadowAttenuation;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Sample base textures
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // Detail textures
                #if defined(_DETAIL)
                half4 detailColor = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw);
                baseColor.rgb = lerp(baseColor.rgb, baseColor.rgb * detailColor.rgb * 2, _DetailStrength);
                #endif
                
                // Normal mapping
                half3 normalWS = input.normalWS;
                #if defined(_NORMALMAP)
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _NormalStrength);
                #if defined(_DETAIL)
                half3 detailNormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, input.uv * _DetailMap_ST.xy + _DetailMap_ST.zw), _DetailStrength);
                normalTS = BlendNormal(normalTS, detailNormalTS);
                #endif
                half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                #endif
                normalWS = normalize(normalWS);
                half3 viewDirWS = normalize(input.viewDirWS);
                
                // SHADOW FIX - New URP shadow coordinate system
                float4 shadowCoord;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    shadowCoord = input.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                #else
                    shadowCoord = float4(0, 0, 0, 0);
                #endif

                // Main lighting with fixed shadows
                Light mainLight = GetMainLight(shadowCoord);
                half shadowAttenuation = mainLight.shadowAttenuation;
                
                half3 diffuseColor = CalculateToonLighting(mainLight.color, mainLight.direction, normalWS, viewDirWS, shadowAttenuation, input.positionWS);
                
                half3 specularColor = half3(0,0,0);
                #if defined(_ENABLEHIGHLIGHTS_ON)
                specularColor = CalculateSpecular(mainLight.color, mainLight.direction, normalWS, viewDirWS, shadowAttenuation);
                #endif
                
                // Rim lighting
                half3 rimColor = half3(0,0,0);
                #if defined(_ENABLERIM_ON)
                half VdotN = 1.0 - saturate(dot(viewDirWS, normalWS));
                half rimIntensity = pow(VdotN, _RimPower) * _RimIntensity;
                rimIntensity = saturate(rimIntensity + _RimOffset);
                rimColor = _RimColor.rgb * rimIntensity * shadowAttenuation;
                #endif
                
                // Subsurface scattering
                half3 subsurfaceColor = half3(0,0,0);
                #if defined(_ENABLESUBSURFACE_ON)
                half3 lightDir = mainLight.direction;
                half3 subsurfaceDir = normalize(lightDir + normalWS * _SubsurfaceDistortion);
                half subsurface = pow(saturate(dot(viewDirWS, -subsurfaceDir)), _SubsurfacePower);
                subsurfaceColor = _SubsurfaceColor.rgb * subsurface * _SubsurfaceIntensity * shadowAttenuation;
                #endif
                
                // Additional lights
                #if defined(_ENABLEADDITIONALLIGHTS_ON)
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    half3 additionalDiffuse = CalculateToonLighting(light.color, light.direction, normalWS, viewDirWS, light.shadowAttenuation, input.positionWS);
                    diffuseColor += additionalDiffuse * 0.5;
                    
                    #if defined(_ENABLEHIGHLIGHTS_ON)
                    specularColor += CalculateSpecular(light.color, light.direction, normalWS, viewDirWS, light.shadowAttenuation) * 0.5;
                    #endif
                }
                #endif
                
                // Combine colors
                half3 color = baseColor.rgb * diffuseColor + specularColor + rimColor + subsurfaceColor;
                
                // Emission
                #if defined(_EMISSION)
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb * _EmissionIntensity;
                color += emission;
                #endif
                
                // Global illumination
                half3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
                color += baseColor.rgb * bakedGI * _LightmapInfluence * _OcclusionStrength;
                
                // Apply fog
                color = MixFog(color, input.fogFactor);
                
                return half4(color, baseColor.a);
            }
            ENDHLSL
        }
        
        // Enhanced Shadow Caster Pass - with SHADOW FIX
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
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // LerpWhiteTo functions - must be defined before Shadows.hlsl for URP 17.0.4
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
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            float3 _LightDirection;
            float3 _LightPosition;

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

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = GetShadowPositionHClip(input);
                output.uv = input.texcoord;
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return 0;
            }

            ENDHLSL
        }
        
        // Enhanced Depth Pass
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 position     : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.position.xyz);
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return 0;
            }

            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Gorgonize.ToonShader.Editor.GorgonizeToonShaderGUI"
}