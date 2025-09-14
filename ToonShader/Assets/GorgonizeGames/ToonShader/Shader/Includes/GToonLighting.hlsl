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

// Toon (cel-shading) aydınlatma hesaplaması
half3 CalculateToonLighting(half3 lightColor, half3 lightDir, half3 normal, half shadowAtten)
{
    half NdotL = dot(normal, lightDir);
    half lightIntensity = NdotL * shadowAtten + _ShadowOffset;
    
    #if defined(_LIGHTINGMODE_STEPPED)
    lightIntensity = floor(lightIntensity * _ShadowSteps) / _ShadowSteps;
    #elif defined(_LIGHTINGMODE_SMOOTH)
    lightIntensity = smoothstep(0, _ShadowSmoothness, lightIntensity);
    #elif defined(_LIGHTINGMODE_RAMP)
    lightIntensity = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, float2(lightIntensity, 0.5)).r;
    #endif
    
    lightIntensity = saturate(lightIntensity);
    
    half3 shadowTint = lerp(half3(1,1,1), _ShadowColor.rgb, _ShadowIntensity);
    
    #if defined(_TINT_SHADOW_ON_BASE)
    // Gölge rengini tüm objeye uygula ve aydınlık alanları aydınlat
    half3 finalColor = lerp(shadowTint, half3(1,1,1), lightIntensity);
    return finalColor * lightColor;
    #else
    // Sadece gölgede kalan alanları renklendir
    return lerp(shadowTint * lightColor, lightColor, lightIntensity);
    #endif
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
