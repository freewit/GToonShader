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
        
        // OUTLINE PASS - İlk olarak render edilir
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag
            
            #pragma shader_feature_local _ENABLEOUTLINE_ON
            #pragma shader_feature_local _ENABLEWIND_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                half _OutlineWidth;
                half _WindSpeed;
                half _WindStrength;
                float4 _WindDirection;
            CBUFFER_END
            
            struct OutlineAttributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct OutlineVaryings
            {
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            OutlineVaryings OutlineVert(OutlineAttributes input)
            {
                OutlineVaryings output = (OutlineVaryings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                #if defined(_ENABLEOUTLINE_ON)
                
                // Wind animation
                float3 positionOS = input.positionOS.xyz;
                #if defined(_ENABLEWIND_ON)
                float time = _Time.y * _WindSpeed;
                float windFactor = sin(time + positionOS.x * 2 + positionOS.z * 2) * _WindStrength;
                positionOS += _WindDirection.xyz * windFactor * input.positionOS.y;
                #endif
                
                // Outline expansion
                float3 normalOS = normalize(input.normalOS);
                positionOS += normalOS * _OutlineWidth * 0.001; // Scale down for reasonable values
                
                output.positionCS = TransformObjectToHClip(positionOS);
                
                #else
                // Outline disabled, move vertex outside clip space
                output.positionCS = float4(0, 0, 0, -1);
                #endif
                
                return output;
            }
            
            half4 OutlineFrag(OutlineVaryings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                #if defined(_ENABLEOUTLINE_ON)
                return _OutlineColor;
                #else
                discard;
                return half4(0, 0, 0, 0);
                #endif
            }
            
            ENDHLSL
        }
        
        // MAIN FORWARD PASS
        Pass
        {
            Name "ToonForward"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            ZWrite On
            ZTest LEqual
            
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
            
            // URP keywords
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
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
                float3 positionWS               : TEXCOORD2;
                float3 normalWS                 : TEXCOORD3;
                #if defined(_NORMALMAP)
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
            
            // Helper functions
            half3 BlendNormal(half3 n1, half3 n2)
            {
                return normalize(half3(n1.xy + n2.xy, n1.z * n2.z));
            }
            
            // Toon lighting calculation
            half3 CalculateToonLighting(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir, half shadowAtten, float3 positionWS)
            {
                half NdotL = dot(normal, lightDir);
                half lightIntensity = NdotL * shadowAtten + _ShadowOffset;
                
                #if defined(_LIGHTINGMODE_STEPPED)
                // Stepped lighting
                lightIntensity = floor(lightIntensity * _ShadowSteps) / _ShadowSteps;
                #elif defined(_LIGHTINGMODE_SMOOTH)
                // Smooth lighting with smoothstep
                lightIntensity = smoothstep(0, _ShadowSmoothness, lightIntensity);
                #elif defined(_LIGHTINGMODE_RAMP)
                // Ramp texture based lighting
                lightIntensity = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(lightIntensity, 0.5)).r;
                #endif
                
                lightIntensity = saturate(lightIntensity);
                
                // Shadow tinting
                half3 shadowTint = lerp(half3(1,1,1), _ShadowColor.rgb, _ShadowIntensity);
                half3 finalColor = lerp(shadowTint, half3(1,1,1), lightIntensity);
                
                #if defined(_TINT_SHADOW_ON_BASE)
                return finalColor * lightColor;
                #else
                return lerp(shadowTint * lightColor, lightColor, lightIntensity);
                #endif
            }
            
            // Rim lighting calculation
            half3 CalculateRimLighting(half3 normal, half3 viewDir, half3 lightColor)
            {
                #if defined(_ENABLERIM_ON)
                half rim = 1.0 - saturate(dot(viewDir, normal));
                rim = pow(rim + _RimOffset, _RimPower) * _RimIntensity;
                return _RimColor.rgb * rim * lightColor;
                #else
                return half3(0,0,0);
                #endif
            }
            
            // Specular calculation
            half3 CalculateSpecular(half3 normal, half3 lightDir, half3 viewDir, half3 lightColor, half shadowAttenuation)
            {
                #if defined(_ENABLEHIGHLIGHTS_ON)
                half3 halfVector = normalize(lightDir + viewDir);
                half NdotH = saturate(dot(normal, halfVector));
                half spec = pow(NdotH, (1.0 - _SpecularSize) * 128.0);
                
                // Toon-style stepped specular
                spec = floor(spec * _SpecularSteps) / _SpecularSteps;
                spec = smoothstep(0, _SpecularSmoothness, spec);
                
                return _SpecularColor.rgb * lightColor * spec * shadowAttenuation;
                #else
                return half3(0,0,0);
                #endif
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionOS = input.positionOS.xyz;
                
                // Wind animation
                #if defined(_ENABLEWIND_ON)
                float time = _Time.y * _WindSpeed;
                float windFactor = sin(time + positionOS.x * 2 + positionOS.z * 2) * _WindStrength;
                positionOS += _WindDirection.xyz * windFactor * input.positionOS.y;
                #endif

                VertexPositionInputs vertexInput = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.normalWS = normalInput.normalWS;
                #if defined(_NORMALMAP)
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
                #endif
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                output.positionWS = vertexInput.positionWS;

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                output.shadowCoord = GetShadowCoord(vertexInput);
                #endif

                output.positionCS = vertexInput.positionCS;

                return output;
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
                half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz), input.normalWS);
                normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
                #endif
                normalWS = normalize(normalWS);
                half3 viewDirWS = normalize(input.viewDirWS);
                
                // Shadow coordinate calculation
                float4 shadowCoord;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    shadowCoord = input.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                    shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                #else
                    shadowCoord = float4(0, 0, 0, 0);
                #endif

                // Main lighting
                Light mainLight = GetMainLight(shadowCoord);
                half shadowAttenuation = mainLight.shadowAttenuation;
                
                half3 diffuseColor = CalculateToonLighting(mainLight.color, mainLight.direction, normalWS, viewDirWS, shadowAttenuation, input.positionWS);
                
                half3 specularColor = CalculateSpecular(normalWS, mainLight.direction, viewDirWS, mainLight.color, shadowAttenuation);
                
                half3 rimColor = CalculateRimLighting(normalWS, viewDirWS, mainLight.color);
                
                // Subsurface scattering
                half3 subsurfaceColor = half3(0,0,0);
                #if defined(_ENABLESUBSURFACE_ON)
                half3 subsurfaceDir = mainLight.direction + normalWS * _SubsurfaceDistortion;
                half subsurfaceDot = pow(saturate(dot(viewDirWS, -subsurfaceDir)), _SubsurfacePower) * _SubsurfaceIntensity;
                subsurfaceColor = _SubsurfaceColor.rgb * subsurfaceDot * mainLight.color;
                #endif

                // Additional lights
                #if defined(_ENABLEADDITIONALLIGHTS_ON)
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    half3 additionalDiffuse = CalculateToonLighting(light.color, light.direction, normalWS, viewDirWS, light.shadowAttenuation * light.distanceAttenuation, input.positionWS);
                    diffuseColor += additionalDiffuse;
                    
                    specularColor += CalculateSpecular(normalWS, light.direction, viewDirWS, light.color, light.shadowAttenuation * light.distanceAttenuation);
                    rimColor += CalculateRimLighting(normalWS, viewDirWS, light.color);
                }
                #endif

                // Lightmap support
                half3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
                
                // Combine all lighting
                half3 finalColor = baseColor.rgb * diffuseColor;
                finalColor += specularColor;
                finalColor += rimColor;
                finalColor += subsurfaceColor;
                finalColor += bakedGI * baseColor.rgb * _LightmapInfluence;
                
                // Emission
                #if defined(_EMISSION)
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb * _EmissionIntensity;
                finalColor += emission;
                #endif
                
                // Fog
                finalColor = MixFog(finalColor, input.fogFactorAndVertexLight.x);
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
        
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5

            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Depth Only Pass
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

            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // Depth Normals Pass
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }
    }

    // Material Editor GUI
    CustomEditor "Gorgonize.ToonShader.Editor.GorgonizeToonShaderGUI"
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}