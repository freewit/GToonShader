Shader "Custom/ToonShaderFinal"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        
        [Header(Toon Lighting)]
        _ToonSteps ("Toon Steps", Range(2, 10)) = 4
        _ShadowRamp ("Shadow Ramp Texture", 2D) = "white" {}
        _ShadowIntensity ("Shadow Intensity", Range(0, 2)) = 1
        
        [Header(Shadow Colors)]
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.8, 1)
        _UseShadowColor ("Use Custom Shadow Color", Range(0, 1)) = 0
        _ShadowColorBlend ("Shadow Color Blend", Range(0, 1)) = 0.5
        
        [Header(Multiple Lights)]
        _AdditionalLightIntensity ("Additional Light Intensity", Range(0, 2)) = 1
        _LightFalloff ("Light Falloff", Range(0.1, 5)) = 1
        
        [Header(Subsurface Scattering)]
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.4, 0.4, 1)
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 0.5
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 0.2
        
        [Header(Rim Light)]
        _RimPower ("Rim Power", Range(0.1, 10)) = 2
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimIntensity ("Rim Intensity", Range(0, 2)) = 1
        
        // Shadow acne fix değerleri - gizli ve sabit
        [HideInInspector] _ShadowSharpness ("Shadow Sharpness", Range(0.01, 1)) = 0.01
        [HideInInspector] _ShadowOffset ("Shadow Offset", Range(-1, 1)) = -0.5
        [HideInInspector] _MyShadowBias ("My Shadow Bias", Range(0, 1.0)) = 0.65
        [HideInInspector] _MyNormalBias ("My Normal Bias", Range(0, 20)) = 0
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
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            
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
                float4 shadowCoord : TEXCOORD4;
            };
            
            TEXTURE2D(_BaseMap);
            TEXTURE2D(_ShadowRamp);
            SAMPLER(sampler_BaseMap);
            SAMPLER(sampler_ShadowRamp);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _ShadowRamp_ST;
                float _ToonSteps;
                float _ShadowIntensity;
                float4 _ShadowColor;
                float _UseShadowColor;
                float _ShadowColorBlend;
                float _AdditionalLightIntensity;
                float _LightFalloff;
                float4 _SubsurfaceColor;
                float _SubsurfaceIntensity;
                float _SubsurfaceDistortion;
                float _RimPower;
                float4 _RimColor;
                float _RimIntensity;
                float _ShadowSharpness;
                float _ShadowOffset;
                float _MyShadowBias;
                float _MyNormalBias;
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
                
                // Çalışan shadow acne fix ayarları - sabit değerler
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float NdotL = dot(normalInputs.normalWS, lightDir);
                
                // Sabit bias değerleri (çalışan ayarlar)
                float bias = (1.0 - abs(NdotL)) * 0 * 0.02; // My Normal Bias = 0
                float3 offsetPos = positionInputs.positionWS + normalInputs.normalWS * bias;
                
                output.shadowCoord = TransformWorldToShadowCoord(offsetPos);
                output.shadowCoord.z -= 0.65; // My Shadow Bias = 0.65 (sabit)
                
                return output;
            }
            
            // Gelişmiş Toon shading fonksiyonu
            float AdvancedToonShading(float NdotL, float2 uv)
            {
                // Sabit shadow offset (-0.5)
                NdotL += -0.5;
                
                // Shadow ramp texture kullan
                float rampSample = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(saturate(NdotL), 0.5)).r;
                
                // Basamaklı toon shading
                float toonFactor = floor((NdotL + rampSample) * _ToonSteps) / _ToonSteps;
                
                // Sabit shadow sharpness (0.01)
                toonFactor = smoothstep(0.5 - 0.01, 0.5 + 0.01, toonFactor);
                
                return saturate(toonFactor * _ShadowIntensity);
            }
            
            // Subsurface scattering hesaplama
            float3 CalculateSubsurface(float3 lightDir, float3 normal, float3 viewDir, float3 lightColor)
            {
                float3 scatterDir = lightDir + normal * _SubsurfaceDistortion;
                float scatterDot = pow(saturate(dot(viewDir, -scatterDir)), _RimPower);
                return lightColor * scatterDot * _SubsurfaceColor.rgb * _SubsurfaceIntensity;
            }
            
            // Rim light hesaplama
            float3 CalculateRimLight(float3 normal, float3 viewDir, float3 lightColor)
            {
                float rim = 1.0 - saturate(dot(normal, viewDir));
                rim = pow(rim, _RimPower);
                return _RimColor.rgb * rim * _RimIntensity * lightColor;
            }
            
            // Ek ışıklar için toon shading
            float3 CalculateAdditionalLights(float3 positionWS, float3 normalWS, float3 viewDirWS)
            {
                float3 additionalLighting = float3(0, 0, 0);
                
                #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, positionWS);
                    
                    float distanceAtten = pow(light.distanceAttenuation, _LightFalloff);
                    float NdotL = saturate(dot(normalWS, light.direction));
                    float toonValue = floor(NdotL * _ToonSteps) / _ToonSteps;
                    toonValue *= distanceAtten;
                    
                    additionalLighting += light.color * toonValue * _AdditionalLightIntensity;
                }
                #endif
                
                return additionalLighting;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Temel renk ve texture sampling
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                
                // Normal ve view direction normalize
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Ana ışık bilgilerini al - shadow mapping kullanmadan
                Light mainLight = GetMainLight();
                
                // CUSTOM SHADOW SİSTEMİ - inline hesaplama (z-fighting yok)
                float NdotL_shadow = dot(normalWS, mainLight.direction);
                float customShadow = smoothstep(0.1, 0.6, NdotL_shadow);
                
                // Kamera uzaklığı bazlı fade
                float camDistance = length(_WorldSpaceCameraPos - input.positionWS);
                float distanceFade = saturate(camDistance * 0.1);
                customShadow = lerp(customShadow, 1.0, distanceFade);
                
                // Lambert lighting hesapla
                float NdotL = dot(normalWS, mainLight.direction);
                
                // Gelişmiş toon shading uygula
                float toonShading = AdvancedToonShading(NdotL, input.uv);
                
                // Custom shadow sistemi uygula - Z-fighting free!
                toonShading *= customShadow;
                
                // Ana ışık rengi
                half3 mainLightColor = mainLight.color;
                
                // DÜZELTME: Çalışan Custom shadow color sistemi
                half3 baseShadowColor = half3(1, 1, 1);
                half3 customShadowColor = _ShadowColor.rgb;
                
                // Shadow tint hesapla
                half3 shadowTint = lerp(baseShadowColor, customShadowColor, _UseShadowColor);
                
                // Light influence hesapla - daha güçlü blending
                half3 lightInfluence = lerp(shadowTint, baseShadowColor, toonShading);
                
                // Shadow color blend uygula
                half3 finalShadowTint = lerp(baseShadowColor, lightInfluence, _ShadowColorBlend);
                
                // Diffuse lighting - shadow color ile
                half3 diffuse = baseColor.rgb * mainLightColor * toonShading * finalShadowTint;
                
                // Subsurface scattering
                half3 subsurface = CalculateSubsurface(mainLight.direction, normalWS, viewDirWS, mainLight.color);
                
                // Rim light hesapla
                half3 rimLight = CalculateRimLight(normalWS, viewDirWS, mainLight.color);
                
                // Ek ışıkları hesapla
                half3 additionalLights = CalculateAdditionalLights(input.positionWS, normalWS, viewDirWS);
                
                // Ambient lighting (SH) - shadow color ile etkilenir
                half3 ambient = SampleSH(normalWS) * baseColor.rgb * 0.3;
                ambient *= finalShadowTint; // Ambient'e de shadow color uygula
                
                // Shadow bölgelerinde ambient'i artır
                ambient *= lerp(1.5, 1.0, toonShading);
                
                // Final color hesaplama
                half3 finalColor = diffuse + subsurface + rimLight + additionalLights + ambient;
                
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