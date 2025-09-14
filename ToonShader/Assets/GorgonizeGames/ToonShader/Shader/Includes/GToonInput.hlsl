#ifndef GTOON_INPUT_INCLUDED
#define GTOON_INPUT_INCLUDED

// Bu dosya, tüm shader özellikleri (CBUFFER) ve doku tanımlamalarını içerir.
// Kod tekrarını önler ve merkezi bir kontrol sağlar.

// Textures
TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
TEXTURE2D(_ShadowRamp);         SAMPLER(sampler_ShadowRamp);
TEXTURE2D(_NormalMap);          SAMPLER(sampler_NormalMap);
TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);
TEXTURE2D(_DetailMap);          SAMPLER(sampler_DetailMap);
TEXTURE2D(_DetailNormalMap);    SAMPLER(sampler_DetailNormalMap);

CBUFFER_START(UnityPerMaterial)
    // Doku Tiling/Offset
    float4 _BaseMap_ST;
    float4 _DetailMap_ST;
    
    // Temel Özellikler
    half4 _BaseColor;
    
    // Aydınlatma & Gölge
    half _LightingMode;
    half _TintShadowOnBase;
    half _ShadowSteps;
    half _ShadowSmoothness;
    half4 _ShadowColor;
    half _ShadowIntensity;
    half _ShadowOffset;
    half _OcclusionStrength;
    
    // Specular
    half4 _SpecularColor;
    half _SpecularSize;
    half _SpecularSmoothness;
    half _SpecularSteps;
    
    // Rim
    half4 _RimColor;
    half _RimPower;
    half _RimIntensity;
    half _RimOffset;
    
    // Gelişmiş Özellikler
    half _NormalStrength;
    half4 _EmissionColor;
    half _EmissionIntensity;
    half _DetailStrength;
    
    // Subsurface
    half4 _SubsurfaceColor;
    half _SubsurfaceIntensity;
    half _SubsurfaceDistortion;
    half _SubsurfacePower;
    
    // Outline
    half4 _OutlineColor;
    half _OutlineWidth;
    
    // Rüzgar
    half _WindSpeed;
    half _WindStrength;
    float4 _WindDirection;
    
    // Performans
    half _LightmapInfluence;
CBUFFER_END

// Not: Vertex ve Fragment shader'lar arasındaki veri yapıları (structs)
// artık ilgili pass'in HLSL dosyası içinde tanımlanmıştır (örn. GToonForwardPass.hlsl).
// Bu, her pass'in sadece ihtiyaç duyduğu veriyi tanımlamasını sağlar ve modülerliği artırır.

#endif // GTOON_INPUT_INCLUDED

