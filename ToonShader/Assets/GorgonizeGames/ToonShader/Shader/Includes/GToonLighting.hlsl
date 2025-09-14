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

// Toon aydınlatma hesaplaması - Artık sadece 0-1 arasında bir aydınlatma yoğunluğu döndürüyor
half CalculateToonLightIntensity(half3 lightDir, half3 normal, half shadowAtten)
{
    half NdotL = dot(normal, lightDir);
    half lightIntensity = NdotL * shadowAtten;
    
    #if defined(_LIGHTINGMODE_SINGLE_CELL)
        half halfSoftness = _TransitionSoftness / 2.0;
        lightIntensity = smoothstep(_ShadowThreshold - halfSoftness, _ShadowThreshold + halfSoftness, lightIntensity);
    #elif defined(_LIGHTINGMODE_BANDED)
        // Ara Ton Eşiği'nin her zaman Ana Gölge Eşiği'nden büyük veya ona eşit olmasını sağla
        half clampedMidtoneThreshold = max(_ShadowThreshold, _MidtoneThreshold);
        
        half totalIntensity = 0;
        // Toplam adım sayısı = ara ton sayısı + 1 (ana gölge için)
        // _BandCount 1-4 arasındadır, bu da 1-4 ara ton demektir.
        // Bu yüzden toplamda _BandCount + 1 adım (seviye) vardır.
        float totalSteps = floor(_BandCount) + 1.0; 
        half halfSoftness = _BandSoftness / 2.0;

        // Her bir bant için eşik değerini hesapla ve topla
        for (float i = 0.0; i < totalSteps; i += 1.0)
        {
            // Eşikleri Ana Gölge ve Ara Ton Bitişi arasında eşit olarak dağıt
            half threshold = lerp(_ShadowThreshold, clampedMidtoneThreshold, i / (totalSteps - 1.0));
            totalIntensity += smoothstep(threshold - halfSoftness, threshold + halfSoftness, lightIntensity);
        }
        
        // Ortalamayı alarak nihai aydınlatma değerini bul
        lightIntensity = totalIntensity / totalSteps;

    #elif defined(_LIGHTINGMODE_RAMP)
        lightIntensity = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(lightIntensity, 0.5)).r;
    #else // Fallback
        half halfSoftness = _TransitionSoftness / 2.0;
        lightIntensity = smoothstep(_ShadowThreshold - halfSoftness, _ShadowThreshold + halfSoftness, lightIntensity);
    #endif

    return saturate(lightIntensity);
}

// Specular (parlama) hesaplaması
half3 CalculateSpecular(half3 normal, half3 lightDir, half3 viewDir, half3 lightColor, half shadowAttenuation)
{
    #if defined(_ENABLEHIGHLIGHTS_ON)
        half3 halfVector = normalize(lightDir + viewDir);
        half NdotH = saturate(dot(normal, halfVector));
        half spec = pow(NdotH, (1.0 - _SpecularSize) * 128.0);
        
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
        rim = pow(rim + _RimOffset, _RimPower) * _RimIntensity;
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

