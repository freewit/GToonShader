#ifndef GTOON_INPUT_INCLUDED
#define GTOON_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

CBUFFER_START(UnityPerMaterial)
    // Base Properties
    float4 _BaseMap_ST;
    half4 _BaseColor;
    half _Cutoff;
    half _Smoothness;
    half _Metallic;
    half _OcclusionStrength;
    
    // Normal Map
    half _BumpScale;
    
    // Lighting Properties
    half _ShadowThreshold;
    half _TransitionSoftness;
    half _ShadowContrast;
    half4 _ShadowColor;
    
    // Dual Cell Lighting
    half _PrimaryThreshold;
    half _SecondaryThreshold;
    half4 _PrimaryShadowColor;
    half4 _SecondaryShadowColor;
    
    // Enhanced Banded Lighting
    half _BandCount;
    half _MidtoneThreshold;
    half _BandSoftness;
    half _BandDistribution;
    
    // Ramp Lighting
    half _RampIntensity;
    
    // Specular Properties - ALL ORIGINAL MODES PRESERVED
    half4 _SpecularColor;
    half _Glossiness;
    half _SpecularThreshold;
    half _SpecularSoftness;
    half _SpecularSize;
    half _SpecularSmoothness;
    half _SpecularSteps;
    half _SteppedFalloff;
    half _SoftSpecularGlossiness;
    half _SoftSpecularStrength;
    half _AnisotropicDirection;
    half _AnisotropicOffset;
    half _AnisotropicSharpness;
    half _AnisotropicIntensity;
    half _SparkleDensity;
    half _SparkleSize;
    half _SparkleAnimSpeed;
    half4 _SparkleColor;
    half4 _SpecularInnerColor;
    half4 _SpecularOuterColor;
    half _SpecularInnerSize;
    half _SpecularOuterSize;
    half _SpecularDoubleToneSoftness;
    half _MatcapIntensity;
    half4 _HairPrimaryColor;
    half4 _HairSecondaryColor;
    half _HairPrimaryShift;
    half _HairSecondaryShift;
    half _HairPrimaryExponent;
    half _HairSecondaryExponent;
    
    // Rim Lighting - ALL ORIGINAL MODES PRESERVED
    half4 _RimColor;
    half _RimThreshold;
    half _RimSoftness;
    half _RimPower;
    half _RimIntensity;
    half _RimOffset;
    half _RimLightInfluence;
    half _RimScrollSpeed;
    half _FresnelPower;
    half _FresnelBias;
    half4 _RimColorTop;
    half4 _RimColorBottom;
    half _RimGradientPower;
    
    // Subsurface Scattering
    half4 _SubsurfaceColor;
    half _SubsurfaceIntensity;
    half _SubsurfacePower;
    half _SubsurfaceDistortion;
    
    // Environment
    half _ReflectionIntensity;
    half _IndirectLightingMultiplier;
    
    // Detail Maps
    float4 _DetailMap_ST;
    half _DetailNormalScale;
    
    // Height Mapping
    half _HeightScale;
    
    // Emission
    half4 _EmissionColor;
    half _EmissionPulseSpeed;
    half _EmissionPulseAmount;
    
    // Wind
    float4 _WindDirection;
    half _WindSpeed;
    half _WindStrength;
    half _WindGustiness;
    
    // Outline
    half4 _OutlineColor;
    half _OutlineWidth;
    half _OutlineZOffset;
CBUFFER_END

// Texture declarations - ALL ORIGINAL TEXTURES PRESERVED
TEXTURE2D(_BaseMap);            SAMPLER(sampler_BaseMap);
TEXTURE2D(_BumpMap);            SAMPLER(sampler_BumpMap);
TEXTURE2D(_DetailMap);          SAMPLER(sampler_DetailMap);
TEXTURE2D(_DetailAlbedoMap);    SAMPLER(sampler_DetailAlbedoMap);
TEXTURE2D(_DetailNormalMap);    SAMPLER(sampler_DetailNormalMap);
TEXTURE2D(_DetailMask);         SAMPLER(sampler_DetailMask);
TEXTURE2D(_HeightMap);          SAMPLER(sampler_HeightMap);
TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);
TEXTURE2D(_ShadowRamp);         SAMPLER(sampler_ShadowRamp);
TEXTURE2D(_CustomRamp);         SAMPLER(sampler_CustomRamp);
TEXTURE2D(_AnisotropicFlowMap); SAMPLER(sampler_AnisotropicFlowMap);
TEXTURE2D(_SoftSpecularMask);   SAMPLER(sampler_SoftSpecularMask);
TEXTURE2D(_MatCapTexture);      SAMPLER(sampler_MatCapTexture);
TEXTURE2D(_RimGradient);        SAMPLER(sampler_RimGradient);

// Surface data input function for compatibility
half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
}

half3 SampleNormal(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_bumpMap), half scale = 1.0h)
{
#ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
    return UnpackNormalScale(n, scale);
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleEmission(float2 uv, half3 emissionColor, TEXTURE2D_PARAM(emissionMap, sampler_emissionMap))
{
#ifndef _EMISSION
    return 0;
#else
    return SAMPLE_TEXTURE2D(emissionMap, sampler_emissionMap, uv).rgb * emissionColor;
#endif
}

#ifdef UNITY_INSTANCING_ENABLED
    UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
        UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
        UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
        UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
    UNITY_INSTANCING_BUFFER_END(Props)

    #define _BaseColor          UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor)
    #define _Cutoff             UNITY_ACCESS_INSTANCED_PROP(Props, _Cutoff)
    #define _Smoothness         UNITY_ACCESS_INSTANCED_PROP(Props, _Smoothness)
    #define _Metallic           UNITY_ACCESS_INSTANCED_PROP(Props, _Metallic)
#endif

#endif