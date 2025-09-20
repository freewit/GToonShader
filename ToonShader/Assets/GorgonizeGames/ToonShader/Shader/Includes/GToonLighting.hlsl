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
    
    #if defined(_LIGHTINGMODE_SINGLE_CELL)
        half halfSoftness = _TransitionSoftness / 2.0;
        lightIntensity = smoothstep(_ShadowThreshold - halfSoftness, _ShadowThreshold + halfSoftness, lightIntensity);
    #elif defined(_LIGHTINGMODE_BANDED)
        half clampedMidtoneThreshold = max(_ShadowThreshold, _MidtoneThreshold);
        half totalIntensity = 0;
        float totalSteps = floor(_BandCount) + 1.0; 
        half halfSoftness = _BandSoftness / 2.0;

        for (float i = 0.0; i < totalSteps; i += 1.0)
        {
            half threshold = lerp(_ShadowThreshold, clampedMidtoneThreshold, i / (totalSteps - 1.0));
            totalIntensity += smoothstep(threshold - halfSoftness, threshold + halfSoftness, lightIntensity);
        }
        
        lightIntensity = totalIntensity / totalSteps;
    #else // Ramp modu ek ışıklar için bu basit fallback'i kullanır
        lightIntensity = smoothstep(0.5, 0.5 + 0.001, lightIntensity);
    #endif

    return saturate(lightIntensity);
}

// Ana ışık için tam renkli difüz hesaplaması yapan ana fonksiyon
half3 CalculateToonDiffuse(Light light, half3 baseColor, half3 normal, half shadowAtten)
{
    half NdotL = dot(normal, light.direction);
    half lightVal = saturate(NdotL * shadowAtten);

    #if defined(_LIGHTINGMODE_RAMP)
        half3 rampColor = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(lightVal, 0.5)).rgb;
        return baseColor * light.color * rampColor;
    #else
        half lightIntensity = CalculateToonLightIntensity(light.direction, normal, shadowAtten);
        half3 litColor = baseColor * light.color;
        half3 shadowColorResult = _ShadowColor.rgb; 
        
        #if defined(_TINT_SHADOW_ON_BASE)
            return lerp(shadowColorResult, litColor, lightIntensity);
        #else
            return litColor * lerp(shadowColorResult, half3(1,1,1), lightIntensity);
        #endif
    #endif
}

// Specular (parlama) hesaplaması için ana fonksiyon
half3 CalculateSpecular(half3 normal, half3 lightDir, half3 viewDir, half3 lightColor, half shadowAttenuation, half4 tangentWS, float2 uv)
{
    half3 halfVector = normalize(lightDir + viewDir);
    half NdotH = saturate(dot(normal, halfVector));

    half3 finalSpecular = half3(0,0,0);

    #if defined(_SPECULARMODE_STEPPED)
        half spec = pow(NdotH, (1.0 - _SpecularSize) * _SteppedFalloff * 64.0);
        spec = floor(spec * _SpecularSteps) / _SpecularSteps;
        spec = smoothstep(0, _SpecularSmoothness, spec);
        finalSpecular = _SpecularColor.rgb * spec;
    #elif defined(_SPECULARMODE_SOFT)
        half spec = pow(NdotH, _SoftSpecularGlossiness * 128.0);
        #if defined(_SPECULARMASK_ON)
            spec *= SAMPLE_TEXTURE2D(_SoftSpecularMask, sampler_SoftSpecularMask, uv).r;
        #endif
        finalSpecular = _SpecularColor.rgb * spec * _SoftSpecularStrength;
    #elif defined(_SPECULARMODE_ENHANCED_ANISOTROPIC)
        half3 tangent = tangentWS.xyz;
        half3 bitangent = cross(normal, tangent) * tangentWS.w;
        #if defined(_ANISOTROPIC_FLOWMAP_ON)
            half2 flow = SAMPLE_TEXTURE2D(_AnisotropicFlowMap, sampler_AnisotropicFlowMap, uv).rg * 2 - 1;
            tangent = normalize(tangent + flow.x * bitangent + flow.y * normal);
        #endif
        half3 anisotropicDir = lerp(tangent, bitangent, _AnisotropicDirection * 0.5 + 0.5);
        half aniso = 1.0 - pow(saturate(dot(anisotropicDir, halfVector) + _AnisotropicOffset), _AnisotropicSharpness * 64.0);
        aniso = pow(saturate(aniso), _AnisotropicSharpness * 128.0);
        finalSpecular = _SpecularColor.rgb * aniso * _AnisotropicIntensity * NdotH;
    #elif defined(_SPECULARMODE_ANIMATED_SPARKLE)
        float2 sparkleUV = uv * _SparkleDensity + (_Time.y * _SparkleAnimSpeed);
        half sparkleNoise = SAMPLE_TEXTURE2D(_SparkleMap, sampler_SparkleMap, sparkleUV).r;
        half spec = pow(NdotH, 64.0);
        half sparkleThreshold = smoothstep(1.0 - _SparkleSize, 1.0, sparkleNoise);
        finalSpecular = _SparkleColor.rgb * spec * sparkleThreshold;
    #elif defined(_SPECULARMODE_DOUBLE_TONE)
        half innerSpec = pow(abs(NdotH), (1.0 - _SpecularInnerSize) * 256.0);
        innerSpec = smoothstep(0.5 - _SpecularDoubleToneSoftness, 0.5 + _SpecularDoubleToneSoftness, innerSpec);
        
        half outerSpec = pow(abs(NdotH), (1.0 - _SpecularOuterSize) * 128.0);
        outerSpec = smoothstep(0.5 - _SpecularDoubleToneSoftness, 0.5 + _SpecularDoubleToneSoftness, outerSpec);
        
        finalSpecular = lerp(_SpecularOuterColor.rgb, _SpecularInnerColor.rgb, innerSpec) * outerSpec;
    #elif defined(_SPECULARMODE_MATCAP)
        half2 matcapUV = mul((float3x3)UNITY_MATRIX_V, normal).xy * 0.5 + 0.5;
        finalSpecular = SAMPLE_TEXTURE2D(_MatcapTex, sampler_MatcapTex, matcapUV).rgb * _MatcapIntensity;
    #elif defined(_SPECULARMODE_HAIR_FUR)
        half3 tangent = tangentWS.xyz;
        half3 H = normalize(lightDir + viewDir);
        
        // Primary Highlight
        half3 shiftedLight1 = normalize(lightDir + tangent * _HairPrimaryShift);
        half3 H1 = normalize(shiftedLight1 + viewDir);
        half spec1 = pow(saturate(dot(normal, H1)), _HairPrimaryExponent);
        
        // Secondary Highlight
        half3 shiftedLight2 = normalize(lightDir + tangent * _HairSecondaryShift);
        half3 H2 = normalize(shiftedLight2 + viewDir);
        half spec2 = pow(saturate(dot(normal, H2)), _HairSecondaryExponent);
        
        finalSpecular = _HairPrimaryColor.rgb * spec1;
        finalSpecular += _HairSecondaryColor.rgb * spec2 * lightColor;
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
    #endif

    return finalRim * _RimIntensity * lightColor;
}


// Subsurface scattering (yüzey altı saçılımı) hesaplaması
half3 CalculateSubsurface(half3 normal, half3 viewDir, Light mainLight)
{
    #if defined(_ENABLESUBSURFACE_ON)
        half3 subsurfaceDir = mainLight.direction + normal * _SubsurfaceDistortion;
        half subsurfaceDot = pow(saturate(dot(viewDir, -subsurfaceDir)), _SubsurfacePower) * _SubsurfaceIntensity;
        return _SubsurfaceColor.rgb * subsurfaceDot * mainLight.color;
    #else
        return half3(0,0,0);
    #endif
}


#endif // GTOON_LIGHTING_INCLUDED
