#ifndef GTOON_LIGHTING_INCLUDED
#define GTOON_LIGHTING_INCLUDED

// Bu dosya, Toon Shader için tüm aydınlatma hesaplama fonksiyonlarını içerir.
// (Gölge, Specular, Rim Light, vb.)

#include "Includes/GToonInput.hlsl"

// Normal map'leri karıştırmak için yardımcı fonksiyon
half3 BlendNormal(half3 n1, half3 n2)
{
    return normalize(half3(n1.xy + n2.xy, n1.z * n2.z));
}

// **CRITICAL: URP 17.0.4 compatible point lights için optimize edilmiş intensity hesaplama fonksiyonu**
half CalculateToonLightIntensity(half3 lightDir, half3 normal, half shadowAtten)
{
    half NdotL = dot(normal, lightDir);
    half lightIntensity = saturate(NdotL) * shadowAtten;
    
    // Toon shading için smooth step transition
    half halfSoftness = _TransitionSoftness / 2.0;
    lightIntensity = smoothstep(_ShadowThreshold - halfSoftness, _ShadowThreshold + halfSoftness, lightIntensity);
    
    // Shadow contrast uygulması
    lightIntensity = pow(lightIntensity, _ShadowContrast);

    return saturate(lightIntensity);
}

// Ana ışık için tam renkli difüz hesaplaması yapan ana fonksiyon (TÜM ORIGINAL FEATURES KORUNDU)
half3 CalculateToonDiffuse(Light light, half3 baseColor, half3 normal, half shadowAtten)
{
    half NdotL = dot(normal, light.direction);
    half lightVal = saturate(NdotL * shadowAtten);
    
    #if defined(_LIGHTINGMODE_SINGLE_CELL)
        half halfSoftness = _TransitionSoftness / 2.0;
        half lightIntensity = smoothstep(_ShadowThreshold - halfSoftness, _ShadowThreshold + halfSoftness, lightVal);
        lightIntensity = pow(lightIntensity, _ShadowContrast);
        half3 litColor = baseColor * light.color;
        
        #if defined(_TINT_SHADOW_ON_BASE)
            return lerp(_ShadowColor.rgb, litColor, lightIntensity);
        #else
            return litColor * lerp(_ShadowColor.rgb, half3(1,1,1), lightIntensity);
        #endif

    #elif defined(_LIGHTINGMODE_DUAL_CELL)
        half halfSoftness = _TransitionSoftness / 2.0;
        half primaryLight = smoothstep(_PrimaryThreshold - halfSoftness, _PrimaryThreshold + halfSoftness, lightVal);
        half secondaryLight = smoothstep(_SecondaryThreshold - halfSoftness, _SecondaryThreshold + halfSoftness, lightVal);
        half3 litColor = baseColor * light.color;
        half3 finalColor = lerp(_SecondaryShadowColor.rgb, _PrimaryShadowColor.rgb, secondaryLight);
        finalColor = lerp(finalColor, litColor, primaryLight);
        return finalColor;

    #elif defined(_LIGHTINGMODE_ENHANCED_BANDED) || defined(_LIGHTINGMODE_BANDED)
        half bandedLight = floor(lightVal * _BandCount) / _BandCount;
        
        #if defined(_LIGHTINGMODE_ENHANCED_BANDED)
            // Enhanced banded lighting with distribution control
            half distributedLight = pow(lightVal, _BandDistribution);
            half midtoneInfluence = smoothstep(0, _MidtoneThreshold, distributedLight);
            bandedLight = lerp(bandedLight, distributedLight, midtoneInfluence);
            bandedLight = lerp(bandedLight, smoothstep(0, 1, bandedLight), _BandSoftness);
        #endif
        
        return baseColor * light.color * bandedLight;

    #elif defined(_LIGHTINGMODE_GRADIENT_RAMP)
        half3 rampColor = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(lightVal, 0.5)).rgb;
        return baseColor * light.color * rampColor * _RampIntensity;

    #elif defined(_LIGHTINGMODE_CUSTOM_RAMP)
        half3 rampColor = SAMPLE_TEXTURE2D(_CustomRamp, sampler_CustomRamp, float2(lightVal, 0.5)).rgb;
        return baseColor * light.color * rampColor * _RampIntensity;
        
    #else
        // Fallback to simple lighting
        return baseColor * light.color * lightVal;
    #endif
}

// Specular Highlight hesaplaması (TÜM ORIGINAL SPECULAR MODES KORUNDU)
half3 CalculateToonSpecular(Light light, half3 normal, half3 viewDir, half shadowAtten)
{
    #if defined(_ENABLESPECULARHIGHLIGHTS_ON)
        half3 lightColor = light.color;
        half3 lightDir = light.direction;
        half lightAtten = shadowAtten * light.distanceAttenuation;
        
        #if defined(_SPECULARMODE_STEPPED)
            half3 halfDir = normalize(lightDir + viewDir);
            half NdotH = saturate(dot(normal, halfDir));
            half specularPower = exp2(_SpecularSize * 10.0);
            half specularIntensity = pow(NdotH, specularPower) * lightAtten;
            specularIntensity = step(_SpecularSteps, specularIntensity);
            return _SpecularColor.rgb * specularIntensity * lightColor * _SpecularColor.a;
            
        #elif defined(_SPECULARMODE_SOFT)
            half3 halfDir = normalize(lightDir + viewDir);
            half NdotH = saturate(dot(normal, halfDir));
            half specularPower = exp2(_SoftSpecularGlossiness * 10.0);
            half specularIntensity = pow(NdotH, specularPower) * lightAtten;
            specularIntensity = smoothstep(0, 1, specularIntensity * _SoftSpecularStrength);
            
            #if defined(_SOFT_SPECULAR_MASK_ON)
                half specularMask = SAMPLE_TEXTURE2D(_SoftSpecularMask, sampler_SoftSpecularMask, uv).r;
                specularIntensity *= specularMask;
            #endif
            
            return _SpecularColor.rgb * specularIntensity * lightColor * _SpecularColor.a;
            
        #elif defined(_SPECULARMODE_ANISOTROPIC)
            // Anisotropic specular calculation
            half3 tangent = normalize(cross(normal, float3(0, 1, 0)));
            half3 binormal = normalize(cross(normal, tangent));
            
            #if defined(_ANISOTROPIC_FLOWMAP_ON)
                float2 flowUV = TRANSFORM_TEX(uv, _AnisotropicFlowMap);
                half2 flowDir = SAMPLE_TEXTURE2D(_AnisotropicFlowMap, sampler_AnisotropicFlowMap, flowUV).rg * 2 - 1;
                tangent = normalize(tangent + flowDir.x * binormal);
                binormal = normalize(cross(normal, tangent));
            #endif
            
            half3 halfDir = normalize(lightDir + viewDir);
            half TdotH = dot(tangent, halfDir);
            half BdotH = dot(binormal, halfDir);
            half NdotH = dot(normal, halfDir);
            
            half aniso = sqrt(max(0, 1 - NdotH * NdotH)) / max(0.001, NdotH);
            half anisoTerm = max(0, sin(aniso * _AnisotropicDirection + _AnisotropicOffset));
            half specularIntensity = pow(anisoTerm, _AnisotropicSharpness) * _AnisotropicIntensity * lightAtten;
            
            return _SpecularColor.rgb * specularIntensity * lightColor * _SpecularColor.a;
            
        #elif defined(_SPECULARMODE_SPARKLE)
            half3 halfDir = normalize(lightDir + viewDir);
            half NdotH = saturate(dot(normal, halfDir));
            
            // Sparkle noise generation
            float2 sparkleUV = uv * _SparkleDensity + _Time.y * _SparkleAnimSpeed;
            half sparkleNoise = frac(sin(dot(sparkleUV, float2(12.9898, 78.233))) * 43758.5453);
            half sparkle = step(0.95, sparkleNoise) * step(_SparkleSize, NdotH);
            
            half specularIntensity = sparkle * lightAtten;
            return _SparkleColor.rgb * specularIntensity * lightColor * _SparkleColor.a;
            
        #elif defined(_SPECULARMODE_DOUBLE_TONE)
            half3 halfDir = normalize(lightDir + viewDir);
            half NdotH = saturate(dot(normal, halfDir));
            half specularPower = exp2(_SpecularSize * 10.0);
            half specularIntensity = pow(NdotH, specularPower) * lightAtten;
            
            half innerMask = smoothstep(_SpecularInnerSize - _SpecularDoubleToneSoftness, _SpecularInnerSize + _SpecularDoubleToneSoftness, specularIntensity);
            half outerMask = smoothstep(_SpecularOuterSize - _SpecularDoubleToneSoftness, _SpecularOuterSize + _SpecularDoubleToneSoftness, specularIntensity);
            
            half3 innerColor = _SpecularInnerColor.rgb * innerMask;
            half3 outerColor = _SpecularOuterColor.rgb * (outerMask - innerMask);
            
            return (innerColor + outerColor) * lightColor;
            
        #elif defined(_SPECULARMODE_MATCAP)
            half3 worldNormal = normal;
            half3 worldViewDir = viewDir;
            half3 worldUp = float3(0, 1, 0);
            half3 worldRight = normalize(cross(worldUp, worldViewDir));
            worldUp = cross(worldViewDir, worldRight);
            
            float2 matcapUV = float2(dot(worldRight, worldNormal), dot(worldUp, worldNormal)) * 0.5 + 0.5;
            half3 matcap = SAMPLE_TEXTURE2D(_MatCapTexture, sampler_MatCapTexture, matcapUV).rgb;
            
            #if defined(_MATCAP_BLEND_ON)
                matcap *= lightColor * lightAtten;
            #endif
            
            return matcap * _MatcapIntensity;
            
        #elif defined(_SPECULARMODE_HAIR)
            half3 tangent = normalize(cross(normal, float3(0, 1, 0)));
            half3 halfDir = normalize(lightDir + viewDir);
            
            // Primary hair highlight
            half TdotH1 = dot(tangent, halfDir);
            half sinTH1 = sqrt(max(0, 1 - TdotH1 * TdotH1));
            half primarySpec = pow(sinTH1, _HairPrimaryExponent) * step(_HairPrimaryShift, TdotH1);
            
            // Secondary hair highlight
            half TdotH2 = dot(tangent, halfDir);
            half sinTH2 = sqrt(max(0, 1 - TdotH2 * TdotH2));
            half secondarySpec = pow(sinTH2, _HairSecondaryExponent) * step(_HairSecondaryShift, TdotH2);
            
            half3 hairColor = _HairPrimaryColor.rgb * primarySpec + _HairSecondaryColor.rgb * secondarySpec;
            return hairColor * lightColor * lightAtten;
            
        #else
            // Default stepped specular
            half3 halfDir = normalize(lightDir + viewDir);
            half NdotH = saturate(dot(normal, halfDir));
            half specularPower = exp2(_Glossiness * 10.0);
            half specularIntensity = pow(NdotH, specularPower) * lightAtten;
            specularIntensity = step(_SpecularThreshold, specularIntensity);
            return _SpecularColor.rgb * specularIntensity * lightColor * _SpecularColor.a;
        #endif
    #else
        return 0;
    #endif
}

// Rim Lighting hesaplaması (TÜM ORIGINAL RIM MODES KORUNDU)
half3 CalculateRimLighting(half3 normal, half3 viewDir, Light mainLight)
{
    #if defined(_ENABLERIM_ON)
        half NdotV = saturate(dot(normal, viewDir));
        half rimFactor = 1.0 - NdotV;
        
        #if defined(_RIMMODE_STEPPED)
            rimFactor = step(_RimThreshold, rimFactor);
            
        #elif defined(_RIMMODE_STANDARD)
            rimFactor = pow(rimFactor, _RimPower);
            rimFactor = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimFactor);
            
        #elif defined(_RIMMODE_LIGHTBASED)
            half LdotV = dot(mainLight.direction, viewDir);
            rimFactor *= saturate(LdotV * _RimLightInfluence);
            rimFactor = pow(rimFactor, _RimPower);
            rimFactor = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimFactor);
            
        #elif defined(_RIMMODE_TEXTURED)
            half rimTexture = SAMPLE_TEXTURE2D(_RimGradient, sampler_RimGradient, float2(rimFactor, 0.5)).r;
            rimFactor = rimTexture * pow(rimFactor, _RimPower);
            rimFactor = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimFactor);
            
        #elif defined(_RIMMODE_FRESNEL_ENHANCED)
            rimFactor = pow(rimFactor, _FresnelPower) + _FresnelBias;
            rimFactor = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimFactor);
            
        #elif defined(_RIMMODE_COLOR_GRADIENT)
            half worldPosY = normalize(normal).y;
            half3 rimColorGradient = lerp(_RimColorBottom.rgb, _RimColorTop.rgb, pow(saturate(worldPosY * 0.5 + 0.5), _RimGradientPower));
            rimFactor = pow(rimFactor, _RimPower);
            rimFactor = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimFactor);
            return rimColorGradient * rimFactor * _RimIntensity;
            
        #else
            rimFactor = pow(rimFactor, _RimPower);
            rimFactor = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimFactor);
        #endif
        
        return _RimColor.rgb * rimFactor * _RimIntensity;
    #else
        return 0;
    #endif
}

// Subsurface Scattering hesaplaması (TÜM ORIGINAL SUBSURFACE MODES KORUNDU)
half3 CalculateSubsurfaceScattering(Light light, half3 normal, half3 viewDir, half3 baseColor)
{
    #if defined(_ENABLESUBSURFACE_ON)
        half3 lightDir = light.direction;
        half VdotL = dot(viewDir, -lightDir);
        half subsurface = saturate(VdotL + _SubsurfaceDistortion);
        
        #if defined(_SUBSURFACE_ADVANCED)
            half backLightIntensity = saturate(dot(-normal, lightDir));
            subsurface *= backLightIntensity;
            
            // Advanced subsurface with thickness simulation
            half thickness = 1.0; // Could be sampled from texture
            subsurface *= thickness;
        #endif
        
        subsurface = pow(subsurface, _SubsurfacePower) * _SubsurfaceIntensity;
        return _SubsurfaceColor.rgb * subsurface * light.color * baseColor;
    #else
        return 0;
    #endif
}

// Environment Reflections hesaplaması (ORIGINAL FUNCTIONALITY KORUNDU)
half3 GToonEnvironmentReflection(half3 reflectVector, half smoothness, half occlusion)
{
    #if defined(_ENVIRONMENTREFLECTIONS_ON)
        half mip = (1.0 - smoothness) * UNITY_SPECCUBE_LOD_STEPS;
        half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip);
        
        #if !defined(UNITY_USE_NATIVE_HDR)
            half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
        #else
            half3 irradiance = encodedIrradiance.rgb;
        #endif
        
        return irradiance * occlusion * _ReflectionIntensity;
    #else
        return 0;
    #endif
}

// **CRITICAL: URP 17.0.4 Additional Light Shadow Support eklendi**
half GetAdditionalLightShadowAttenuation(Light light, float3 positionWS)
{
    #if defined(_ADDITIONAL_LIGHT_SHADOWS)
        return AdditionalLightRealtimeShadow(light.layerMask, positionWS, light.direction);
    #else
        return 1.0;
    #endif
}

// **URP 17.0.4 Light layer compatibility function**
bool IsLightLayerCompatible(uint lightLayers, uint meshRenderingLayers)
{
    #if defined(_LIGHT_LAYERS)
        return (lightLayers & meshRenderingLayers) != 0;
    #else
        return true;
    #endif
}

// **URP 17.0.4 Light distance attenuation için yardımcı fonksiyon**
half GetDistanceAttenuation(half distanceSqr, half2 distanceAttenuation)
{
    // Smooth attenuation calculation based on URP
    half factor = distanceSqr * distanceAttenuation.x;
    half smoothFactor = saturate(1.0 - factor * factor);
    return smoothFactor * smoothFactor;
}

#endif