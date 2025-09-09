Shader "Custom/BasicToonShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        _ToonSteps ("Toon Steps", Range(2, 10)) = 4
        _RimPower ("Rim Power", Range(0.1, 10)) = 2
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
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
            
            // URP gerekli keyword'ler
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS
            
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
                float3 viewDirWS : TEXCOORD3;
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float _ToonSteps;
                float _RimPower;
                float4 _RimColor;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                
                return output;
            }
            
            // Toon shading fonksiyonu
            float ToonShading(float NdotL)
            {
                // Işığı basamaklı hale getir
                float toonFactor = floor(NdotL * _ToonSteps) / _ToonSteps;
                return saturate(toonFactor);
            }
            
            // Rim light hesaplama
            float CalculateRimLight(float3 normal, float3 viewDir)
            {
                float rim = 1.0 - saturate(dot(normal, viewDir));
                return pow(rim, _RimPower);
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Temel renk ve texture sampling
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // Normal ve view direction normalize
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Ana ışık bilgilerini al
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                
                // Lambert lighting hesapla
                float NdotL = dot(normalWS, mainLight.direction);
                NdotL = max(0, NdotL);
                
                // Toon shading uygula
                float toonShading = ToonShading(NdotL);
                
                // Shadow attenuation
                float shadowAttenuation = mainLight.shadowAttenuation;
                toonShading *= shadowAttenuation;
                
                // Rim light hesapla
                float rimLight = CalculateRimLight(normalWS, viewDirWS);
                
                // Final color hesaplama
                half3 lightColor = mainLight.color;
                half3 diffuse = baseColor.rgb * lightColor * toonShading;
                half3 ambient = SampleSH(normalWS) * baseColor.rgb * 0.3; // Basit ambient
                half3 rim = _RimColor.rgb * rimLight;
                
                half3 finalColor = diffuse + ambient + rim;
                
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
        
        // Depth only pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}