Shader "Custom/NewToonShaderFixed"
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
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Shadow mode keywords
            #pragma multi_compile _SHADOWMODE_SIMPLE _SHADOWMODE_BANDED _SHADOWMODE_RAMP
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };
            
            TEXTURE2D(_BaseMap);
            TEXTURE2D(_RampTexture);
            SAMPLER(sampler_BaseMap);
            SAMPLER(sampler_RampTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _RampTexture_ST;
                float4 _ShadowColor;
                float _ShadowThreshold;
                float4 _BandedShadowColor;
                float _BandedShadowThreshold;
                float _BandCount;
                float _BandSmooth;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Base color ve texture sampling
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // Normal normalize
                float3 normalWS = normalize(input.normalWS);
                
                // Main light al
                Light mainLight = GetMainLight();
                
                // NdotL hesapla
                float NdotL = dot(normalWS, mainLight.direction);
                
                // Final color başlangıç
                half3 finalColor = baseColor.rgb;
                
                #if defined(_SHADOWMODE_SIMPLE)
                    // Simple shadow - CELL SHADING (keskin geçiş)
                    float shadowValue = step(_ShadowThreshold, NdotL);
                    float3 shadowTint = _ShadowColor.rgb;
                    float3 lightInfluence = lerp(shadowTint, float3(1,1,1), shadowValue);
                    finalColor = baseColor.rgb * lightInfluence;
                    
                #elif defined(_SHADOWMODE_BANDED)
                    // Banded shadow - düzeltilmiş smoothness
                    float shadowValue = step(_BandedShadowThreshold, NdotL);
                    float bandedShadow = floor(shadowValue * _BandCount) / _BandCount;
                    
                    // Düzeltilmiş smoothness - daha iyi çalışan
                    if(_BandSmooth > 0)
                    {
                        float currentBand = floor(shadowValue * _BandCount);
                        float nextBand = currentBand + 1.0;
                        float bandProgress = (shadowValue * _BandCount) - currentBand;
                        
                        float smoothStart = 1.0 - _BandSmooth;
                        float smoothedProgress = smoothstep(smoothStart, 1.0, bandProgress);
                        
                        bandedShadow = (currentBand + smoothedProgress) / _BandCount;
                    }
                    
                    float3 shadowTint = _BandedShadowColor.rgb;
                    float3 lightInfluence = lerp(shadowTint, float3(1,1,1), bandedShadow);
                    finalColor = baseColor.rgb * lightInfluence;
                    
                #elif defined(_SHADOWMODE_RAMP)
                    // Ramp shadow - tiling/offset yok
                    float shadowValue = smoothstep(0.4, 0.6, NdotL);
                    float3 rampColor = SAMPLE_TEXTURE2D(_RampTexture, sampler_RampTexture, float2(shadowValue, 0.5)).rgb;
                    finalColor = baseColor.rgb * rampColor;
                #endif
                
                // Main light color uygula
                finalColor *= mainLight.color;
                
                // Basic ambient
                half3 ambient = SampleSH(normalWS) * baseColor.rgb * 0.3;
                finalColor += ambient;
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
        
        // Shadow caster pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}