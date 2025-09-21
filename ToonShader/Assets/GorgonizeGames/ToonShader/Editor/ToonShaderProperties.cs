using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Gorgonize Toon Shader için tüm materyal özelliklerini yönetir
    /// </summary>
    public class ToonShaderProperties
    {
        // Helper method to find properties
        private MaterialProperty FindProperty(string name, MaterialProperty[] props)
        {
            return System.Array.Find(props, p => p.name == name);
        }

        // Base Properties
        public MaterialProperty baseColor;
        public MaterialProperty baseMap;
        public MaterialProperty metallic;
        public MaterialProperty smoothness;
        public MaterialProperty enableSpecularHighlights;
        public MaterialProperty enableEnvironmentReflections;
        public MaterialProperty environmentReflections;

        // Lighting/Shadow Properties
        public MaterialProperty lightingMode;
        public MaterialProperty tintShadowOnBase;
        public MaterialProperty shadowThreshold;
        public MaterialProperty transitionSoftness;
        public MaterialProperty shadowContrast;
        public MaterialProperty shadowRamp;
        public MaterialProperty customRamp;
        public MaterialProperty shadowColor;
        public MaterialProperty occlusionStrength;
        public MaterialProperty lightmapInfluence;
        public MaterialProperty receiveShadows;

        // Dual Cell Mode
        public MaterialProperty primaryThreshold;
        public MaterialProperty secondaryThreshold;
        public MaterialProperty primaryShadowColor;
        public MaterialProperty secondaryShadowColor;
        
        // Banded Mode Properties
        public MaterialProperty bandCount;
        public MaterialProperty midtoneThreshold;
        public MaterialProperty bandSoftness;
        public MaterialProperty bandDistribution;

        // Ramp Mode
        public MaterialProperty rampIntensity;
        
        // Specular Properties
        public MaterialProperty specularMode;
        public MaterialProperty specularColor;
        public MaterialProperty specularSize;
        public MaterialProperty specularSmoothness;
        public MaterialProperty specularSteps;
        public MaterialProperty steppedFalloff;
        public MaterialProperty softSpecularGlossiness;
        public MaterialProperty softSpecularStrength;
        public MaterialProperty softSpecularMask;
        public MaterialProperty anisotropicDirection;
        public MaterialProperty anisotropicSharpness;
        public MaterialProperty anisotropicIntensity;
        public MaterialProperty anisotropicOffset;
        public MaterialProperty anisotropicFlowMap;
        public MaterialProperty sparkleMap;
        public MaterialProperty sparkleDensity;
        public MaterialProperty sparkleSize;
        public MaterialProperty sparkleAnimSpeed;
        public MaterialProperty sparkleColor;
        public MaterialProperty specularInnerColor;
        public MaterialProperty specularOuterColor;
        public MaterialProperty specularInnerSize;
        public MaterialProperty specularOuterSize;
        public MaterialProperty specularDoubleToneSoftness;
        public MaterialProperty matcapTex;
        public MaterialProperty matcapIntensity;
        public MaterialProperty matcapBlendWithLighting;
        public MaterialProperty hairPrimaryColor;
        public MaterialProperty hairSecondaryColor;
        public MaterialProperty hairPrimaryShift;
        public MaterialProperty hairSecondaryShift;
        public MaterialProperty hairPrimaryExponent;
        public MaterialProperty hairSecondaryExponent;

        // Rim Lighting Properties
        public MaterialProperty enableRim;
        public MaterialProperty rimMode;
        public MaterialProperty rimColor;
        public MaterialProperty rimPower;
        public MaterialProperty rimIntensity;
        public MaterialProperty rimOffset;
        public MaterialProperty rimThreshold;
        public MaterialProperty rimSoftness;
        public MaterialProperty rimLightInfluence;
        public MaterialProperty rimTexture;
        public MaterialProperty rimScrollSpeed;
        public MaterialProperty fresnelPower;
        public MaterialProperty fresnelBias;
        public MaterialProperty rimColorTop;
        public MaterialProperty rimColorBottom;
        public MaterialProperty rimGradientPower;

        // Advanced Feature Properties
        public MaterialProperty normalMap;
        public MaterialProperty normalStrength;
        public MaterialProperty enableParallax;
        public MaterialProperty heightMap;
        public MaterialProperty heightScale;
        public MaterialProperty emissionMap;
        public MaterialProperty emissionColor;
        public MaterialProperty emissionIntensity;
        public MaterialProperty enableEmissionPulse;
        public MaterialProperty pulseSpeed;
        public MaterialProperty detailMap;
        public MaterialProperty detailNormalMap;
        public MaterialProperty detailNormalScale;
        public MaterialProperty detailStrength;
        
        // Subsurface Scattering Properties
        public MaterialProperty enableSubsurface;
        public MaterialProperty subsurfaceMode;
        public MaterialProperty subsurfaceColor;
        public MaterialProperty subsurfaceIntensity;
        public MaterialProperty subsurfaceDistortion;
        public MaterialProperty subsurfacePower;
        public MaterialProperty subsurfaceMap;
        public MaterialProperty thicknessMap;

        // Outline Properties
        public MaterialProperty enableOutline;
        public MaterialProperty outlineColor;
        public MaterialProperty outlineColorB;
        public MaterialProperty outlineWidth;
        public MaterialProperty outlineMode;
        public MaterialProperty outlineDistanceScaling;
        public MaterialProperty outlineAdaptiveMinWidth;
        public MaterialProperty outlineAdaptiveMaxWidth;
        public MaterialProperty outlineAnimatedColor;
        public MaterialProperty outlineAnimationSpeed;

        // Wind Animation Properties
        public MaterialProperty enableWind;
        public MaterialProperty windMode;
        public MaterialProperty windSpeed;
        public MaterialProperty windStrength;
        public MaterialProperty windDirection;
        public MaterialProperty windTurbulence;
        public MaterialProperty windNoiseScale;
        public MaterialProperty windPhaseVariation;
        public MaterialProperty branchBending;
        public MaterialProperty windVertexColorMask;

        // Performance Properties
        public MaterialProperty enableAdditionalLights;

        public ToonShaderProperties(MaterialProperty[] properties)
        {
            FindAllProperties(properties);
        }

        /// <summary>
        /// Tüm material property'leri bulur ve atar
        /// </summary>
        private void FindAllProperties(MaterialProperty[] properties)
        {
            baseColor = FindProperty("_BaseColor", properties);
            baseMap = FindProperty("_BaseMap", properties);
            metallic = FindProperty("_Metallic", properties);
            smoothness = FindProperty("_Smoothness", properties);
            enableSpecularHighlights = FindProperty("_EnableSpecularHighlights", properties);
            enableEnvironmentReflections = FindProperty("_EnableEnvironmentReflections", properties);
            environmentReflections = FindProperty("_EnvironmentReflections", properties);
            
            lightingMode = FindProperty("_LightingMode", properties);
            tintShadowOnBase = FindProperty("_TintShadowOnBase", properties);
            shadowThreshold = FindProperty("_ShadowThreshold", properties);
            transitionSoftness = FindProperty("_TransitionSoftness", properties);
            shadowContrast = FindProperty("_ShadowContrast", properties);
            shadowRamp = FindProperty("_ShadowRamp", properties);
            customRamp = FindProperty("_CustomRamp", properties);
            shadowColor = FindProperty("_ShadowColor", properties);
            occlusionStrength = FindProperty("_OcclusionStrength", properties);
            lightmapInfluence = FindProperty("_LightmapInfluence", properties);
            receiveShadows = FindProperty("_ReceiveShadows", properties);

            primaryThreshold = FindProperty("_PrimaryThreshold", properties);
            secondaryThreshold = FindProperty("_SecondaryThreshold", properties);
            primaryShadowColor = FindProperty("_PrimaryShadowColor", properties);
            secondaryShadowColor = FindProperty("_SecondaryShadowColor", properties);
            
            bandCount = FindProperty("_BandCount", properties);
            midtoneThreshold = FindProperty("_MidtoneThreshold", properties);
            bandSoftness = FindProperty("_BandSoftness", properties);
            bandDistribution = FindProperty("_BandDistribution", properties);
            
            rampIntensity = FindProperty("_RampIntensity", properties);
            
            specularMode = FindProperty("_SpecularMode", properties);
            specularColor = FindProperty("_SpecularColor", properties);
            specularSize = FindProperty("_SpecularSize", properties);
            specularSmoothness = FindProperty("_SpecularSmoothness", properties);
            specularSteps = FindProperty("_SpecularSteps", properties);
            steppedFalloff = FindProperty("_SteppedFalloff", properties);
            softSpecularGlossiness = FindProperty("_SoftSpecularGlossiness", properties);
            softSpecularStrength = FindProperty("_SoftSpecularStrength", properties);
            softSpecularMask = FindProperty("_SoftSpecularMask", properties);
            anisotropicDirection = FindProperty("_AnisotropicDirection", properties);
            anisotropicSharpness = FindProperty("_AnisotropicSharpness", properties);
            anisotropicIntensity = FindProperty("_AnisotropicIntensity", properties);
            anisotropicOffset = FindProperty("_AnisotropicOffset", properties);
            anisotropicFlowMap = FindProperty("_AnisotropicFlowMap", properties);
            sparkleMap = FindProperty("_SparkleMap", properties);
            sparkleDensity = FindProperty("_SparkleDensity", properties);
            sparkleSize = FindProperty("_SparkleSize", properties);
            sparkleAnimSpeed = FindProperty("_SparkleAnimSpeed", properties);
            sparkleColor = FindProperty("_SparkleColor", properties);
            specularInnerColor = FindProperty("_SpecularInnerColor", properties);
            specularOuterColor = FindProperty("_SpecularOuterColor", properties);
            specularInnerSize = FindProperty("_SpecularInnerSize", properties);
            specularOuterSize = FindProperty("_SpecularOuterSize", properties);
            specularDoubleToneSoftness = FindProperty("_SpecularDoubleToneSoftness", properties);
            matcapTex = FindProperty("_MatcapTex", properties);
            matcapIntensity = FindProperty("_MatcapIntensity", properties);
            matcapBlendWithLighting = FindProperty("_MatcapBlendWithLighting", properties);
            hairPrimaryColor = FindProperty("_HairPrimaryColor", properties);
            hairSecondaryColor = FindProperty("_HairSecondaryColor", properties);
            hairPrimaryShift = FindProperty("_HairPrimaryShift", properties);
            hairSecondaryShift = FindProperty("_HairSecondaryShift", properties);
            hairPrimaryExponent = FindProperty("_HairPrimaryExponent", properties);
            hairSecondaryExponent = FindProperty("_HairSecondaryExponent", properties);

            enableRim = FindProperty("_EnableRim", properties);
            rimMode = FindProperty("_RimMode", properties);
            rimColor = FindProperty("_RimColor", properties);
            rimPower = FindProperty("_RimPower", properties);
            rimIntensity = FindProperty("_RimIntensity", properties);
            rimOffset = FindProperty("_RimOffset", properties);
            rimThreshold = FindProperty("_RimThreshold", properties);
            rimSoftness = FindProperty("_RimSoftness", properties);
            rimLightInfluence = FindProperty("_RimLightInfluence", properties);
            rimTexture = FindProperty("_RimTexture", properties);
            rimScrollSpeed = FindProperty("_RimScrollSpeed", properties);
            fresnelPower = FindProperty("_FresnelPower", properties);
            fresnelBias = FindProperty("_FresnelBias", properties);
            rimColorTop = FindProperty("_RimColorTop", properties);
            rimColorBottom = FindProperty("_RimColorBottom", properties);
            rimGradientPower = FindProperty("_RimGradientPower", properties);
            
            normalMap = FindProperty("_NormalMap", properties);
            normalStrength = FindProperty("_NormalStrength", properties);
            enableParallax = FindProperty("_EnableParallax", properties);
            heightMap = FindProperty("_HeightMap", properties);
            heightScale = FindProperty("_HeightScale", properties);
            emissionMap = FindProperty("_EmissionMap", properties);
            emissionColor = FindProperty("_EmissionColor", properties);
            emissionIntensity = FindProperty("_EmissionIntensity", properties);
            enableEmissionPulse = FindProperty("_EnableEmissionPulse", properties);
            pulseSpeed = FindProperty("_PulseSpeed", properties);
            detailMap = FindProperty("_DetailMap", properties);
            detailNormalMap = FindProperty("_DetailNormalMap", properties);
            detailNormalScale = FindProperty("_DetailNormalScale", properties);
            detailStrength = FindProperty("_DetailStrength", properties);
            
            enableSubsurface = FindProperty("_EnableSubsurface", properties);
            subsurfaceMode = FindProperty("_SubsurfaceMode", properties);
            subsurfaceColor = FindProperty("_SubsurfaceColor", properties);
            subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", properties);
            subsurfaceDistortion = FindProperty("_SubsurfaceDistortion", properties);
            subsurfacePower = FindProperty("_SubsurfacePower", properties);
            subsurfaceMap = FindProperty("_SubsurfaceMap", properties);
            thicknessMap = FindProperty("_ThicknessMap", properties);

            enableOutline = FindProperty("_EnableOutline", properties);
            outlineColor = FindProperty("_OutlineColor", properties);
            outlineColorB = FindProperty("_OutlineColorB", properties);
            outlineWidth = FindProperty("_OutlineWidth", properties);
            outlineMode = FindProperty("_OutlineMode", properties);
            outlineDistanceScaling = FindProperty("_OutlineDistanceScaling", properties);
            outlineAdaptiveMinWidth = FindProperty("_OutlineAdaptiveMinWidth", properties);
            outlineAdaptiveMaxWidth = FindProperty("_OutlineAdaptiveMaxWidth", properties);
            outlineAnimatedColor = FindProperty("_OutlineAnimatedColor", properties);
            outlineAnimationSpeed = FindProperty("_OutlineAnimationSpeed", properties);

            enableWind = FindProperty("_EnableWind", properties);
            windMode = FindProperty("_WindMode", properties);
            windSpeed = FindProperty("_WindSpeed", properties);
            windStrength = FindProperty("_WindStrength", properties);
            windDirection = FindProperty("_WindDirection", properties);
            windTurbulence = FindProperty("_WindTurbulence", properties);
            windNoiseScale = FindProperty("_WindNoiseScale", properties);
            windPhaseVariation = FindProperty("_WindPhaseVariation", properties);
            branchBending = FindProperty("_BranchBending", properties);
            windVertexColorMask = FindProperty("_WindVertexColorMask", properties);

            enableAdditionalLights = FindProperty("_EnableAdditionalLights", properties);
        }

        /// <summary>
        /// Feature'ın aktif olup olmadığını kontrol eder
        /// </summary>
        public bool IsFeatureEnabled(MaterialProperty property)
        {
            if (property == null) return false;
            
            if (property.type == MaterialProperty.PropType.Float || 
                property.type == MaterialProperty.PropType.Range)
            {
                return property.floatValue > 0.5f;
            }
            
            return false;
        }

        /// <summary>
        /// Float property değerini güvenli şekilde alır
        /// </summary>
        public float GetFloatValue(MaterialProperty property)
        {
            if (property == null) return 0.0f;
            
            if (property.type == MaterialProperty.PropType.Float || 
                property.type == MaterialProperty.PropType.Range)
            {
                return property.floatValue;
            }
            
            return 0.0f;
        }
        
        /// <summary>
        /// Shader keyword'lerini günceller
        /// </summary>
        public void UpdateKeywords(Material material)
        {
            // Lighting Mode
            if (IsPropertyValid(lightingMode))
            {
                var mode = GetFloatValue(lightingMode);
                SetKeyword(material, "_LIGHTINGMODE_SINGLE_CELL", mode == 0f);
                SetKeyword(material, "_LIGHTINGMODE_DUAL_CELL", mode == 1f);
                SetKeyword(material, "_LIGHTINGMODE_BANDED", mode == 2f);
                SetKeyword(material, "_LIGHTINGMODE_GRADIENT_RAMP", mode == 3f);
                SetKeyword(material, "_LIGHTINGMODE_CUSTOM_RAMP", mode == 4f);
            }
            
            // Subsurface Mode
            if (IsPropertyValid(subsurfaceMode))
            {
                var mode = GetFloatValue(subsurfaceMode);
                SetKeyword(material, "_SUBSURFACE_BASIC", mode == 0f);
                SetKeyword(material, "_SUBSURFACE_ADVANCED", mode == 1f);
            }

            // Specular Mode
            if (IsPropertyValid(specularMode))
            {
                var mode = GetFloatValue(specularMode);
                SetKeyword(material, "_SPECULARMODE_STEPPED", mode == 0f);
                SetKeyword(material, "_SPECULARMODE_SOFT", mode == 1f);
                SetKeyword(material, "_SPECULARMODE_ANISOTROPIC", mode == 2f);
                SetKeyword(material, "_SPECULARMODE_SPARKLE", mode == 3f);
                SetKeyword(material, "_SPECULARMODE_DOUBLE_TONE", mode == 4f);
                SetKeyword(material, "_SPECULARMODE_MATCAP", mode == 5f);
                SetKeyword(material, "_SPECULARMODE_HAIR", mode == 6f);
            }
            
            // Rim Mode
            if (IsPropertyValid(rimMode))
            {
                var mode = GetFloatValue(rimMode);
                SetKeyword(material, "_RIMMODE_STANDARD", mode == 0f);
                SetKeyword(material, "_RIMMODE_STEPPED", mode == 1f);
                SetKeyword(material, "_RIMMODE_LIGHTBASED", mode == 2f);
                SetKeyword(material, "_RIMMODE_TEXTURED", mode == 3f);
                SetKeyword(material, "_RIMMODE_FRESNEL_ENHANCED", mode == 4f);
                SetKeyword(material, "_RIMMODE_COLOR_GRADIENT", mode == 5f);
            }
            
            // Outline Mode
            if (IsPropertyValid(outlineMode))
            {
                var mode = GetFloatValue(outlineMode);
                SetKeyword(material, "_OUTLINEMODE_NORMAL", mode == 0f);
                SetKeyword(material, "_OUTLINEMODE_POSITION", mode == 1f);
                SetKeyword(material, "_OUTLINEMODE_UV", mode == 2f);
            }
            
             // Wind Mode
            if (IsPropertyValid(windMode))
            {
                var mode = GetFloatValue(windMode);
                SetKeyword(material, "_WINDMODE_BASIC", mode == 0f);
                SetKeyword(material, "_WINDMODE_ADVANCED", mode == 1f);
            }

            // Toggle Keywords
            SetKeyword(material, "_TINT_SHADOW_ON_BASE", IsFeatureEnabled(tintShadowOnBase));
            SetKeyword(material, "_ENABLESPECULARHIGHLIGHTS_ON", IsFeatureEnabled(enableSpecularHighlights));
            SetKeyword(material, "_ENVIRONMENTREFLECTIONS_ON", IsFeatureEnabled(enableEnvironmentReflections));
            SetKeyword(material, "_ENABLERIM_ON", IsFeatureEnabled(enableRim));
            SetKeyword(material, "_ENABLESUBSURFACE_ON", IsFeatureEnabled(enableSubsurface));
            SetKeyword(material, "_ENABLEOUTLINE_ON", IsFeatureEnabled(enableOutline));
            SetKeyword(material, "_OUTLINE_DISTANCE_SCALING_ON", IsFeatureEnabled(outlineDistanceScaling));
            SetKeyword(material, "_OUTLINE_ANIMATED_COLOR_ON", IsFeatureEnabled(outlineAnimatedColor));
            SetKeyword(material, "_ENABLEWIND_ON", IsFeatureEnabled(enableWind));
            SetKeyword(material, "_WIND_VERTEX_COLOR_MASK_ON", IsFeatureEnabled(windVertexColorMask));
            SetKeyword(material, "_RECEIVESHADOWS_ON", IsFeatureEnabled(receiveShadows));
            SetKeyword(material, "_ADDITIONAL_LIGHTS_ON", IsFeatureEnabled(enableAdditionalLights));
            SetKeyword(material, "_ENABLEPARALLAX_ON", IsFeatureEnabled(enableParallax));
            SetKeyword(material, "_ENABLEEMISSIONPULSE_ON", IsFeatureEnabled(enableEmissionPulse));

            // Texture-based Keywords
            SetKeyword(material, "_NORMALMAP", normalMap?.textureValue != null);
            SetKeyword(material, "_HEIGHTMAP", heightMap?.textureValue != null && IsFeatureEnabled(enableParallax));
            SetKeyword(material, "_DETAIL", detailMap?.textureValue != null || detailNormalMap?.textureValue != null);
            bool hasEmission = emissionMap?.textureValue != null || 
                              (emissionColor != null && emissionColor.colorValue.maxColorComponent > 0);
            SetKeyword(material, "_EMISSION", hasEmission);
            SetKeyword(material, "_MATCAP_BLEND_ON", IsFeatureEnabled(matcapBlendWithLighting));
            SetKeyword(material, "_ANISOTROPIC_FLOWMAP_ON", anisotropicFlowMap?.textureValue != null);
            SetKeyword(material, "_SOFT_SPECULAR_MASK_ON", softSpecularMask?.textureValue != null);
        }

        private void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        /// <summary>
        /// Property'nin geçerli olup olmadığını kontrol eder
        /// </summary>
        public bool IsPropertyValid(MaterialProperty property)
        {
            return property != null;
        }
    }
}

