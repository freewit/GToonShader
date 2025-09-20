#ifndef GTOON_INPUT_INCLUDED
#define GTOON_INPUT_INCLUDED

// Bu dosya, tüm shader özellikleri (CBUFFER) ve doku tanımlamalarını içerir.
// Kod tekrarını önler ve merkezi bir kontrol sağlar.

// Textures
TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
TEXTURE2D(_ShadowRamp);         SAMPLER(sampler_ShadowRamp);
TEXTURE2D(_CustomRamp);         SAMPLER(sampler_CustomRamp);
TEXTURE2D(_NormalMap);          SAMPLER(sampler_NormalMap);
TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);
TEXTURE2D(_DetailMap);          SAMPLER(sampler_DetailMap);
TEXTURE2D(_DetailNormalMap);    SAMPLER(sampler_DetailNormalMap);
TEXTURE2D(_SparkleMap);         SAMPLER(sampler_SparkleMap);
TEXTURE2D(_SoftSpecularMask);   SAMPLER(sampler_SoftSpecularMask);
TEXTURE2D(_AnisotropicFlowMap); SAMPLER(sampler_AnisotropicFlowMap);
TEXTURE2D(_MatcapTex);          SAMPLER(sampler_MatcapTex);
TEXTURE2D(_RimTexture);         SAMPLER(sampler_RimTexture);

CBUFFER_START(UnityPerMaterial)
    // Doku Tiling/Offset
    float4 _BaseMap_ST;
    float4 _DetailMap_ST;
    float4 _RimTexture_ST;
    
    // Temel Özellikler
    half4 _BaseColor;
    half _Metallic;
    half _Smoothness;
    half _EnvironmentReflections;
    
    // Aydınlatma & Gölge
    half _LightingMode;
    half _TintShadowOnBase;
    half _ShadowThreshold;
    half _TransitionSoftness;
    half4 _ShadowColor;
    half _ShadowContrast;
    half _PrimaryThreshold;
    half _SecondaryThreshold;
    half4 _PrimaryShadowColor;
    half4 _SecondaryShadowColor;
    half _OcclusionStrength;

    // Banded Mode
    half _BandCount;
    half _MidtoneThreshold;
    half _BandSoftness;
    half _BandDistribution;

    // Ramp Modes
    half _RampIntensity;
    
    // Specular
    half _SpecularMode;
    half4 _SpecularColor;
    half _SpecularSize;
    half _SpecularSmoothness;
    half _SpecularSteps;
    half _SteppedFalloff;
    half _SoftSpecularGlossiness;
    half _SoftSpecularStrength;
    half _AnisotropicDirection;
    half _AnisotropicSharpness;
    half _AnisotropicIntensity;
    half _AnisotropicOffset;
    half _SparkleDensity;
    half4 _SparkleColor;
    half _SparkleSize;
    half _SparkleAnimSpeed;
    half4 _SpecularInnerColor;
    half4 _SpecularOuterColor;
    half _SpecularInnerSize;
    half _SpecularOuterSize;
    half _SpecularDoubleToneSoftness;
    half _MatcapIntensity;
    half _MatcapBlendWithLighting;
    half4 _HairPrimaryColor;
    half4 _HairSecondaryColor;
    half _HairPrimaryShift;
    half _HairSecondaryShift;
    half _HairPrimaryExponent;
    half _HairSecondaryExponent;

    // Rim
    half _RimMode;
    half4 _RimColor;
    half _RimPower;
    half _RimIntensity;
    half _RimOffset;
    half _RimThreshold;
    half _RimSoftness;
    half _RimLightInfluence;
    half _RimScrollSpeed;
    
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
    half _OutlineMode;
    half _OutlineAdaptiveMinWidth;
    half _OutlineAdaptiveMaxWidth;
    half4 _OutlineColorB;
    half _OutlineAnimationSpeed;
    
    // Rüzgar
    half _WindSpeed;
    half _WindStrength;
    float4 _WindDirection;
    
    // Performans
    half _LightmapInfluence;
CBUFFER_END

#endif // GTOON_INPUT_INCLUDED

