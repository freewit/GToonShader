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
        
        // Lighting/Shadow Properties
        public MaterialProperty lightingMode;
        public MaterialProperty tintShadowOnBase;
        public MaterialProperty shadowThreshold;
        public MaterialProperty transitionSoftness;
        public MaterialProperty shadowRamp;
        public MaterialProperty shadowColor;
        public MaterialProperty occlusionStrength;
        
        // Banded Mode Properties
        public MaterialProperty bandCount;
        public MaterialProperty midtoneThreshold;
        public MaterialProperty bandSoftness;

        // Highlight Properties
        public MaterialProperty enableHighlights;
        public MaterialProperty specularMode;
        public MaterialProperty specularColor;
        public MaterialProperty specularSize;
        public MaterialProperty specularSmoothness;
        public MaterialProperty specularSteps;
        public MaterialProperty softSpecularGlossiness;
        public MaterialProperty softSpecularStrength;
        public MaterialProperty anisotropicDirection;
        public MaterialProperty anisotropicSharpness;
        public MaterialProperty anisotropicIntensity;
        public MaterialProperty anisotropicOffset;
        public MaterialProperty sparkleMap;
        public MaterialProperty sparkleDensity;
        public MaterialProperty sparkleColor;
        public MaterialProperty specularInnerColor;
        public MaterialProperty specularOuterColor;
        public MaterialProperty specularInnerSize;
        public MaterialProperty specularOuterSize;
        public MaterialProperty specularDoubleToneSoftness;

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
        
        // Advanced Feature Properties
        public MaterialProperty normalMap;
        public MaterialProperty normalStrength;
        public MaterialProperty emissionMap;
        public MaterialProperty emissionColor;
        public MaterialProperty emissionIntensity;
        public MaterialProperty detailMap;
        public MaterialProperty detailNormalMap;
        public MaterialProperty detailStrength;
        
        // Subsurface Scattering Properties
        public MaterialProperty enableSubsurface;
        public MaterialProperty subsurfaceColor;
        public MaterialProperty subsurfaceIntensity;
        public MaterialProperty subsurfaceDistortion;
        public MaterialProperty subsurfacePower;
        
        // Outline Properties
        public MaterialProperty enableOutline;
        public MaterialProperty outlineColor;
        public MaterialProperty outlineWidth;
        public MaterialProperty outlineNoiseEnabled;
        public MaterialProperty outlineNoiseScale;
        public MaterialProperty outlineNoiseStrength;
        public MaterialProperty outlineNoiseSpeed;
        
        // Wind Animation Properties
        public MaterialProperty enableWind;
        public MaterialProperty windSpeed;
        public MaterialProperty windStrength;
        public MaterialProperty windDirection;
        
        // Performance Properties
        public MaterialProperty receiveShadows;
        public MaterialProperty enableAdditionalLights;
        public MaterialProperty lightmapInfluence;

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
            lightingMode = FindProperty("_LightingMode", properties);
            tintShadowOnBase = FindProperty("_TintShadowOnBase", properties);
            shadowThreshold = FindProperty("_ShadowThreshold", properties);
            transitionSoftness = FindProperty("_TransitionSoftness", properties);
            shadowRamp = FindProperty("_ShadowRamp", properties);
            shadowColor = FindProperty("_ShadowColor", properties);
            occlusionStrength = FindProperty("_OcclusionStrength", properties);
            bandCount = FindProperty("_BandCount", properties);
            midtoneThreshold = FindProperty("_MidtoneThreshold", properties);
            bandSoftness = FindProperty("_BandSoftness", properties);
            enableHighlights = FindProperty("_EnableHighlights", properties);
            specularMode = FindProperty("_SpecularMode", properties);
            specularColor = FindProperty("_SpecularColor", properties);
            specularSize = FindProperty("_SpecularSize", properties);
            specularSmoothness = FindProperty("_SpecularSmoothness", properties);
            specularSteps = FindProperty("_SpecularSteps", properties);
            softSpecularGlossiness = FindProperty("_SoftSpecularGlossiness", properties);
            softSpecularStrength = FindProperty("_SoftSpecularStrength", properties);
            anisotropicDirection = FindProperty("_AnisotropicDirection", properties);
            anisotropicSharpness = FindProperty("_AnisotropicSharpness", properties);
            anisotropicIntensity = FindProperty("_AnisotropicIntensity", properties);
            anisotropicOffset = FindProperty("_AnisotropicOffset", properties);
            sparkleMap = FindProperty("_SparkleMap", properties);
            sparkleDensity = FindProperty("_SparkleDensity", properties);
            sparkleColor = FindProperty("_SparkleColor", properties);
            specularInnerColor = FindProperty("_SpecularInnerColor", properties);
            specularOuterColor = FindProperty("_SpecularOuterColor", properties);
            specularInnerSize = FindProperty("_SpecularInnerSize", properties);
            specularOuterSize = FindProperty("_SpecularOuterSize", properties);
            specularDoubleToneSoftness = FindProperty("_SpecularDoubleToneSoftness", properties);
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
            normalMap = FindProperty("_NormalMap", properties);
            normalStrength = FindProperty("_NormalStrength", properties);
            emissionMap = FindProperty("_EmissionMap", properties);
            emissionColor = FindProperty("_EmissionColor", properties);
            emissionIntensity = FindProperty("_EmissionIntensity", properties);
            detailMap = FindProperty("_DetailMap", properties);
            detailNormalMap = FindProperty("_DetailNormalMap", properties);
            detailStrength = FindProperty("_DetailStrength", properties);
            enableSubsurface = FindProperty("_EnableSubsurface", properties);
            subsurfaceColor = FindProperty("_SubsurfaceColor", properties);
            subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", properties);
            subsurfaceDistortion = FindProperty("_SubsurfaceDistortion", properties);
            subsurfacePower = FindProperty("_SubsurfacePower", properties);
            enableOutline = FindProperty("_EnableOutline", properties);
            outlineColor = FindProperty("_OutlineColor", properties);
            outlineWidth = FindProperty("_OutlineWidth", properties);
            outlineNoiseEnabled = FindProperty("_OutlineNoiseEnabled", properties);
            outlineNoiseScale = FindProperty("_OutlineNoiseScale", properties);
            outlineNoiseStrength = FindProperty("_OutlineNoiseStrength", properties);
            outlineNoiseSpeed = FindProperty("_OutlineNoiseSpeed", properties);
            enableWind = FindProperty("_EnableWind", properties);
            windSpeed = FindProperty("_WindSpeed", properties);
            windStrength = FindProperty("_WindStrength", properties);
            windDirection = FindProperty("_WindDirection", properties);
            receiveShadows = FindProperty("_ReceiveShadows", properties);
            enableAdditionalLights = FindProperty("_EnableAdditionalLights", properties);
            lightmapInfluence = FindProperty("_LightmapInfluence", properties);
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
                SetKeyword(material, "_LIGHTINGMODE_BANDED", mode == 1f);
                SetKeyword(material, "_LIGHTINGMODE_RAMP", mode == 2f);
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
            }
            
            // Rim Mode
            if (IsPropertyValid(rimMode))
            {
                var mode = GetFloatValue(rimMode);
                SetKeyword(material, "_RIMMODE_STANDARD", mode == 0f);
                SetKeyword(material, "_RIMMODE_STEPPED", mode == 1f);
                SetKeyword(material, "_RIMMODE_LIGHTBASED", mode == 2f);
                SetKeyword(material, "_RIMMODE_TEXTURED", mode == 3f);
            }

            // Tinting
            SetKeyword(material, "_TINT_SHADOW_ON_BASE", IsFeatureEnabled(tintShadowOnBase));

            // Highlights
            SetKeyword(material, "_ENABLEHIGHLIGHTS_ON", IsFeatureEnabled(enableHighlights));
            
            // Rim Lighting
            SetKeyword(material, "_ENABLERIM_ON", IsFeatureEnabled(enableRim));
            
            // Subsurface Scattering
            SetKeyword(material, "_ENABLESUBSURFACE_ON", IsFeatureEnabled(enableSubsurface));
            
            // Outline
            SetKeyword(material, "_ENABLEOUTLINE_ON", IsFeatureEnabled(enableOutline));
            SetKeyword(material, "_OUTLINE_NOISE_ON", IsFeatureEnabled(outlineNoiseEnabled));
            
            // Wind Animation
            SetKeyword(material, "_ENABLEWIND_ON", IsFeatureEnabled(enableWind));
            
            // Normal Map
            SetKeyword(material, "_NORMALMAP", normalMap?.textureValue != null);
            
            // Emission
            bool hasEmission = emissionMap?.textureValue != null || 
                              (emissionColor != null && emissionColor.colorValue.maxColorComponent > 0);
            SetKeyword(material, "_EMISSION", hasEmission);
            
            // Detail Maps
            SetKeyword(material, "_DETAIL", detailMap?.textureValue != null);
            
            // Additional Lights
            SetKeyword(material, "_ENABLEADDITIONALLIGHTS_ON", IsFeatureEnabled(enableAdditionalLights));
            
            // Receive Shadows
            SetKeyword(material, "_RECEIVESHADOWS_ON", IsFeatureEnabled(receiveShadows));
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

