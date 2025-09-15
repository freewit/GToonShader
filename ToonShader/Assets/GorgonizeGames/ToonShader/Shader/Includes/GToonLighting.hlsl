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

// Specular (parlama) hesaplaması
half3 CalculateSpecular(half3 normal, half3 lightDir, half3 viewDir, half3 lightColor, half shadowAttenuation)
{
    #if defined(_ENABLEHIGHLIGHTS_ON)
        half3 halfVector = normalize(lightDir + viewDir);
        half NdotH = saturate(dot(normal, halfVector));
        // DÜZELTME: 'pow' fonksiyonunun tabanının negatif olmasını engellemek için abs() kullanıldı.
        // NdotH zaten 'saturate' ile 0-1 aralığına sıkıştırılmış olsa da, derleyici uyarısını gidermek için bu en güvenli yoldur.
        half spec = pow(abs(NdotH), (1.0 - _SpecularSize) * 128.0);
        
        spec = floor(spec * _SpecularSteps) / _SpecularSteps;
        spec = smoothstep(0, _SpecularSmoothness, spec);
        
        return _SpecularColor.rgb * lightColor * spec * shadowAttenuation;
    #else
        return half3(0,0,0);
    #endif
}

// Rim light (kenar aydınlatması) hesaplaması
half3 CalculateRimLighting(half3 normal, half3 viewDir, half3 lightColor)
{
    #if defined(_ENABLERIM_ON)
        half rim = 1.0 - saturate(dot(viewDir, normal));
        // DÜZELTME: rim + _RimOffset ifadesi negatif olabileceğinden, 'pow' fonksiyonuna girmeden önce 'saturate' ile 0'dan küçük olması engellendi.
        rim = pow(saturate(rim + _RimOffset), _RimPower) * _RimIntensity;
        return _RimColor.rgb * rim * lightColor;
    #else
        return half3(0,0,0);
    #endif
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
