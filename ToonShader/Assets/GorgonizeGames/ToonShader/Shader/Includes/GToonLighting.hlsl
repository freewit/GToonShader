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

// Sadece yoğunluk hesaplayan basit aydınlatma fonksiyonu (Ek ışıklar için)
half CalculateToonLightIntensity(half3 lightDir, half3 normal, half shadowAtten)
{
    half NdotL = dot(normal, lightDir);
    half lightIntensity = NdotL * shadowAtten;
    
    half halfSoftness = _TransitionSoftness / 2.0;
    lightIntensity = smoothstep(_ShadowThreshold - halfSoftness, _ShadowThreshold + halfSoftness, lightIntensity);

    return saturate(lightIntensity);
}

// Ana ışık için tam renkli difüz hesaplaması yapan ana fonksiyon
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
        
    #elif defined(_LIGHTINGMODE_BANDED)
        half halfSoftness = _BandSoftness / 2.0;
        float totalSteps = floor(_BandCount);
        half totalIntensity = 0;
        for (float i = 0.0; i < totalSteps; i += 1.0)
        {
            half currentStep = i / (totalSteps - 1.0);
            half threshold = lerp(_ShadowThreshold, _MidtoneThreshold, pow(currentStep, _BandDistribution));
            totalIntensity += smoothstep(threshold - halfSoftness, threshold + halfSoftness, lightVal);
        }
        half lightIntensity = totalIntensity / totalSteps;
        half3 litColor = baseColor * light.color;
        return litColor * lerp(_ShadowColor.rgb, half3(1,1,1), lightIntensity);
        
    #elif defined(_LIGHTINGMODE_GRADIENT_RAMP)
        half3 rampColor = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(lightVal, 0.5)).rgb;
        return baseColor * light.color * lerp(half3(1,1,1), rampColor, _RampIntensity);
        
    #elif defined(_LIGHTINGMODE_CUSTOM_RAMP)
        half3 rampColor = SAMPLE_TEXTURE2D(_CustomRamp, sampler_CustomRamp, float2(lightVal, 0.5)).rgb;
        return baseColor * light.color * lerp(half3(1,1,1), rampColor, _RampIntensity);
    
    #else // Fallback
        return baseColor * light.color * lightVal;
    #endif
}

// Specular (parlama) hesaplaması için ana fonksiyon
half3 CalculateSpecular(half3 normal, half3 lightDir, half3 viewDir, half3 lightColor, half shadowAttenuation, half4 tangentWS, float2 uv)
{
    half3 halfVector = normalize(lightDir + viewDir);
    half NdotH = saturate(dot(normal, halfVector));
    half3 finalSpecular = half3(0,0,0);
    half3 tangent = tangentWS.xyz;

    #if defined(_SPECULARMODE_STEPPED)
        half spec = pow(abs(NdotH), (1.0 - _SpecularSize) * 128.0 * _SteppedFalloff);
        spec = floor(spec * _SpecularSteps) / _SpecularSteps;
        spec = smoothstep(0, _SpecularSmoothness, spec);
        finalSpecular = _SpecularColor.rgb * spec;
    #elif defined(_SPECULARMODE_SOFT)
        half spec = pow(NdotH, _SoftSpecularGlossiness * 128.0);
        #if defined(_SOFT_SPECULAR_MASK_ON)
            spec *= SAMPLE_TEXTURE2D(_SoftSpecularMask, sampler_SoftSpecularMask, uv).r;
        #endif
        finalSpecular = _SpecularColor.rgb * spec * _SoftSpecularStrength;
    #elif defined(_SPECULARMODE_ANISOTROPIC)
        #if defined(_ANISOTROPIC_FLOWMAP_ON)
            half2 flow = (SAMPLE_TEXTURE2D(_AnisotropicFlowMap, sampler_AnisotropicFlowMap, uv).xy * 2.0 - 1.0);
            tangent = normalize(tangent + flow.x * cross(normal, tangent));
        #endif
        half3 bitangent = cross(normal, tangent) * tangentWS.w;
        half3 anisotropicDir = lerp(tangent, bitangent, _AnisotropicDirection * 0.5 + 0.5);
        half aniso = 1.0 - pow(saturate(dot(anisotropicDir, halfVector) + _AnisotropicOffset), _AnisotropicSharpness * 64.0);
        aniso = pow(saturate(aniso), _AnisotropicSharpness * 128.0);
        finalSpecular = _SpecularColor.rgb * aniso * _AnisotropicIntensity * NdotH;
    #elif defined(_SPECULARMODE_SPARKLE)
        float2 sparkleUV = uv * _SparkleDensity;
        sparkleUV.x += _Time.y * _SparkleAnimSpeed;
        half sparkleNoise = SAMPLE_TEXTURE2D(_SparkleMap, sampler_SparkleMap, sparkleUV).r;
        half spec = pow(NdotH, 64.0);
        half sparkleThreshold = smoothstep(1.0 - (_SparkleSize * 0.1), 1.0, sparkleNoise);
        finalSpecular = _SparkleColor.rgb * spec * sparkleThreshold;
    #elif defined(_SPECULARMODE_DOUBLE_TONE)
        half innerSpec = pow(abs(NdotH), (1.0 - _SpecularInnerSize) * 256.0);
        innerSpec = smoothstep(0.5 - _SpecularDoubleToneSoftness, 0.5 + _SpecularDoubleToneSoftness, innerSpec);
        half outerSpec = pow(abs(NdotH), (1.0 - _SpecularOuterSize) * 128.0);
        outerSpec = smoothstep(0.5 - _SpecularDoubleToneSoftness, 0.5 + _SpecularDoubleToneSoftness, outerSpec);
        finalSpecular = lerp(_SpecularOuterColor.rgb, _SpecularInnerColor.rgb, innerSpec) * outerSpec;
    #elif defined(_SPECULARMODE_MATCAP)
        float3 viewNormal = mul((float3x3)UNITY_MATRIX_V, normal);
        float2 matcapUV = viewNormal.xy * 0.5 + 0.5;
        half3 matcapColor = SAMPLE_TEXTURE2D(_MatcapTex, sampler_MatcapTex, matcapUV).rgb * _MatcapIntensity;
        #if defined(_MATCAP_BLEND_ON)
             finalSpecular = matcapColor;
        #else
             return matcapColor; // Return directly if not blending
        #endif
    #elif defined(_SPECULARMODE_HAIR)
        half TdotH = dot(tangent, halfVector);
        half sinTdotH = sqrt(1.0 - TdotH * TdotH);

        half dirAtten = smoothstep(-1.0, 0.0, dot(tangent, lightDir));
        
        half shiftTangent1 = tangent + normal * _HairPrimaryShift;
        half shiftTangent2 = tangent + normal * _HairSecondaryShift;
        
        half3 specular1 = _HairPrimaryColor.rgb * pow(saturate(dot(lightDir, normalize(shiftTangent1))), _HairPrimaryExponent);
        half3 specular2 = _HairSecondaryColor.rgb * pow(saturate(dot(lightDir, normalize(shiftTangent2))), _HairSecondaryExponent);
        
        finalSpecular = (specular1 + specular2) * dirAtten;
    #endif

    return finalSpecular * lightColor * shadowAttenuation;
}


// Rim light (kenar aydınlatması) hesaplaması
half3 CalculateRimLighting(half3 normal, half3 viewDir, half3 lightDir, half3 lightColor, float2 uv)
{
    #if !defined(_ENABLERIM_ON)
        return half3(0,0,0);
    #endif

    half rimDot = 1.0 - saturate(dot(viewDir, normal));
    half3 finalRim = half3(0,0,0);
    
    #if defined(_RIMMODE_STANDARD)
        half rim = pow(saturate(rimDot + _RimOffset), _RimPower);
        finalRim = _RimColor.rgb * rim;
    #elif defined(_RIMMODE_STEPPED)
        half rim = smoothstep(_RimThreshold - _RimSoftness, _RimThreshold + _RimSoftness, rimDot);
        finalRim = _RimColor.rgb * rim;
    #elif defined(_RIMMODE_LIGHTBASED)
        half lightDot = saturate(dot(normal, lightDir));
        half rim = pow(saturate(rimDot + _RimOffset), _RimPower) * saturate(lightDot * _RimLightInfluence);
        finalRim = _RimColor.rgb * rim;
    #elif defined(_RIMMODE_TEXTURED)
        float2 scrolledUV = uv + _Time.y * _RimScrollSpeed * float2(0.1, 0.1);
        half4 rimTex = SAMPLE_TEXTURE2D(_RimTexture, sampler_RimTexture, scrolledUV);
        half rim = pow(saturate(rimDot + _RimOffset), _RimPower);
        finalRim = _RimColor.rgb * rimTex.rgb * rim;
    #elif defined(_RIMMODE_FRESNEL_ENHANCED)
        half fresnel = _FresnelBias + (1.0 - _FresnelBias) * pow(1.0 - dot(viewDir, normal), _FresnelPower);
        finalRim = _RimColor.rgb * fresnel;
    #elif defined(_RIMMODE_COLOR_GRADIENT)
        half gradient = pow(saturate(normal.y * 0.5 + 0.5), _RimGradientPower);
        half3 gradientColor = lerp(_RimColorBottom.rgb, _RimColorTop.rgb, gradient);
        half rim = pow(saturate(rimDot + _RimOffset), _RimPower);
        finalRim = gradientColor * rim;
    #endif

    return finalRim * _RimIntensity * lightColor;
}


// Subsurface scattering (yüzey altı saçılımı) hesaplaması
half3 CalculateSubsurface(half3 normal, half3 viewDir, Light mainLight, float2 uv)
{
    #if defined(_ENABLESUBSURFACE_ON)
        half subsurfaceMask = 1.0;
        half thickness = 1.0;
        
        #if defined(_SUBSURFACE_ADVANCED)
            subsurfaceMask = SAMPLE_TEXTURE2D(_SubsurfaceMap, sampler_SubsurfaceMap, uv).r;
            thickness = SAMPLE_TEXTURE2D(_ThicknessMap, sampler_ThicknessMap, uv).r;
        #endif

        half3 subsurfaceDir = mainLight.direction + normal * _SubsurfaceDistortion;
        half subsurfaceDot = pow(saturate(dot(viewDir, -subsurfaceDir)), _SubsurfacePower) * _SubsurfaceIntensity;
        
        return _SubsurfaceColor.rgb * subsurfaceDot * mainLight.color * subsurfaceMask * thickness;
    #else
        return half3(0,0,0);
    #endif
}


#endif // GTOON_LIGHTING_INCLUDED

