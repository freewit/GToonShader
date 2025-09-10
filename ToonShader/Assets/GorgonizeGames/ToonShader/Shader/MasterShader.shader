Shader "Custom/GorgonizeToonShader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        
        [Header(Shadow Mode)]
        [KeywordEnum(Simple, Banded, Ramp)] _ShadowMode ("Shadow Mode", Float) = 0
        
        [Header(Simple Shadow)]
        _ShadowColor ("Shadow Color", Color) = (0.6, 0.6, 0.8, 1)
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        
        [Header(Banded Shadow)]
        _BandedShadowColor ("Banded Shadow Color", Color) = (0.6, 0.6, 0.8, 1)
        _BandedShadowThreshold ("Banded Shadow Threshold", Range(0, 1)) = 0.5
        _BandCount ("Band Count", Range(2, 5)) = 3
        _BandSmooth ("Band Smoothness", Range(0, 0.5)) = 0.1
        
        [Header(Ramp Shadow)]
        _RampTexture ("Ramp Texture", 2D) = "white" {}
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
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma target 4.5
            
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma shader_feature_local _SHADOWMODE_SIMPLE _SHADOWMODE_BANDED _SHADOWMODE_RAMP
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
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
                float3 positionWS   : TEXCOORD1;
                float3 normalWS     : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_RampTexture);
            SAMPLER(sampler_RampTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _ShadowColor;
                half _ShadowThreshold;
                half4 _BandedShadowColor;
                half _BandedShadowThreshold;
                half _BandCount;
                half _BandSmooth;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Base color
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half3 result = baseColor.rgb;
                
                // Lighting setup
                half3 normalWS = normalize(input.normalWS);
                Light mainLight = GetMainLight();
                half mainNdotL = saturate(dot(normalWS, mainLight.direction));
                half mainShadow = smoothstep(0.1h, 0.6h, mainNdotL);
                
                // Additional lights
                half additionalContribution = 0.0h;
                #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    half lightNdotL = saturate(dot(normalWS, light.direction));
                    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
                    half pointShadow = smoothstep(0.1h, 0.6h, lightNdotL);
                    additionalContribution += pointShadow * lightAttenuation * 0.5h;
                }
                #endif
                
                // Combined lighting
                half totalLighting = saturate(mainShadow + additionalContribution);
                
                // Apply shadow modes
                #if defined(_SHADOWMODE_SIMPLE)
                    half isLit = totalLighting >= _ShadowThreshold ? 1.0h : 0.0h;
                    half3 shadowTint = _ShadowColor.rgb;
                    half3 lightInfluence = lerp(shadowTint, half3(1,1,1), isLit);
                    result = baseColor.rgb * lightInfluence;
                    
                #elif defined(_SHADOWMODE_BANDED)
                    half lightValue = totalLighting >= _BandedShadowThreshold ? totalLighting : 0.0h;
                    half bandedValue = floor(lightValue * _BandCount) / _BandCount;
                    
                    if(_BandSmooth > 0.01h)
                    {
                        half currentBandIndex = floor(lightValue * _BandCount);
                        half nextBandIndex = min(currentBandIndex + 1.0h, _BandCount);
                        half positionInBand = (lightValue * _BandCount) - currentBandIndex;
                        half smoothStart = 1.0h - _BandSmooth;
                        
                        if(positionInBand > smoothStart)
                        {
                            half smoothProgress = (positionInBand - smoothStart) / _BandSmooth;
                            smoothProgress = smoothstep(0.0h, 1.0h, smoothProgress);
                            half currentBandValue = currentBandIndex / _BandCount;
                            half nextBandValue = nextBandIndex / _BandCount;
                            bandedValue = lerp(currentBandValue, nextBandValue, smoothProgress);
                        }
                    }
                    
                    half3 shadowTint = _BandedShadowColor.rgb;
                    half3 lightInfluence = lerp(shadowTint, half3(1,1,1), bandedValue);
                    result = baseColor.rgb * lightInfluence;
                    
                #elif defined(_SHADOWMODE_RAMP)
                    half rampInput = smoothstep(0.0h, 1.0h, totalLighting);
                    half3 rampColor = SAMPLE_TEXTURE2D(_RampTexture, sampler_RampTexture, half2(rampInput, 0.5h)).rgb;
                    result = baseColor.rgb * rampColor;
                #endif
                
                // Apply main light color
                result *= mainLight.color;
                
                // Add ambient
                half3 ambient = SampleSH(normalWS) * baseColor.rgb * 0.3h;
                result += ambient;
                
                return half4(result, baseColor.a);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}
            ZWrite On
            ColorMask 0
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    
    CustomEditor "GorgonizeToonShaderGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}