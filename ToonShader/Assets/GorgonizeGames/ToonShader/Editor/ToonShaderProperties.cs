using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Gorgonize Toon Shader için tüm materyal özelliklerini yönetir
    /// </summary>
    public class ToonShaderProperties
    {
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
        
        // Specular Properties (New and Updated)
        public MaterialProperty specularMode;
        
        // Mode 0: Stepped
        public MaterialProperty specularColor;
        public MaterialProperty specularSize;
        public MaterialProperty specularSmoothness;
        public MaterialProperty specularSteps;
        
        // Mode 1: Soft
        public MaterialProperty softSpecularGlossiness;
        public MaterialProperty softSpecularStrength;
        
        // Mode 2: Anisotropic
        public MaterialProperty anisotropicDirection;
        public MaterialProperty anisotropicSharpness;
        public MaterialProperty anisotropicIntensity;
        public MaterialProperty anisotropicOffset;
        
        // Mode 3: Sparkle
        public MaterialProperty sparkleMap;
        public MaterialProperty sparkleDensity;
        public MaterialProperty sparkleColor;
        
        // Mode 4: Double Tone
        public MaterialProperty specularInnerColor;
        public MaterialProperty specularOuterColor;
        public MaterialProperty specularInnerSize;
        public MaterialProperty specularOuterSize;
        public MaterialProperty specularDoubleToneSoftness;
        
        // Rim Lighting Properties
        public MaterialProperty enableRim;
        public MaterialProperty rimColor;
        public MaterialProperty rimPower;
        public MaterialProperty rimIntensity;
        public MaterialProperty rimOffset;
        
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
            foreach (var prop in properties)
            {
                switch (prop.name)
                {
                    // Base Properties
                    case "_BaseColor":
                    case "_Color":
                        baseColor = prop;
                        break;
                        
                    case "_BaseMap":
                    case "_MainTex":
                        baseMap = prop;
                        break;
                    
                    // Lighting/Shadow Properties
                    case "_LightingMode":
                        lightingMode = prop;
                        break;
                    
                    // Specular Properties
                    case "_SpecularMode":
                        specularMode = prop;
                        break;
                    case "_SpecularColor":
                        specularColor = prop;
                        break;
                    case "_SpecularSize":
                        specularSize = prop;
                        break;
                    case "_SpecularSmoothness":
                        specularSmoothness = prop;
                        break;
                    case "_SpecularSteps":
                        specularSteps = prop;
                        break;
                    case "_SoftSpecularGlossiness":
                        softSpecularGlossiness = prop;
                        break;
                    case "_SoftSpecularStrength":
                        softSpecularStrength = prop;
                        break;
                    case "_AnisotropicDirection":
                        anisotropicDirection = prop;
                        break;
                    case "_AnisotropicSharpness":
                        anisotropicSharpness = prop;
                        break;
                    case "_AnisotropicIntensity":
                        anisotropicIntensity = prop;
                        break;
                    case "_AnisotropicOffset":
                        anisotropicOffset = prop;
                        break;
                    case "_SparkleMap":
                        sparkleMap = prop;
                        break;
                    case "_SparkleDensity":
                        sparkleDensity = prop;
                        break;
                    case "_SparkleColor":
                        sparkleColor = prop;
                        break;
                    case "_SpecularInnerColor":
                        specularInnerColor = prop;
                        break;
                    case "_SpecularOuterColor":
                        specularOuterColor = prop;
                        break;
                    case "_SpecularInnerSize":
                        specularInnerSize = prop;
                        break;
                    case "_SpecularOuterSize":
                        specularOuterSize = prop;
                        break;
                    case "_SpecularDoubleToneSoftness":
                        specularDoubleToneSoftness = prop;
                        break;

                    case "_TintShadowOnBase":
                        tintShadowOnBase = prop;
                        break;
                        
                    case "_ShadowThreshold":
                        shadowThreshold = prop;
                        break;
                        
                    case "_TransitionSoftness":
                        transitionSoftness = prop;
                        break;
                        
                    case "_ShadowRamp":
                        shadowRamp = prop;
                        break;
                        
                    case "_ShadowColor":
                        shadowColor = prop;
                        break;
                        
                    case "_OcclusionStrength":
                        occlusionStrength = prop;
                        break;

                    // Banded Mode
                    case "_BandCount":
                        bandCount = prop;
                        break;
                    case "_MidtoneThreshold":
                        midtoneThreshold = prop;
                        break;
                    case "_BandSoftness":
                        bandSoftness = prop;
                        break;
                    
                    // Highlight Properties
                    case "_EnableHighlights":
                        enableHighlights = prop;
                        break;
                    
                    // Rim Lighting Properties
                    case "_EnableRim":
                        enableRim = prop;
                        break;
                        
                    case "_RimColor":
                        rimColor = prop;
                        break;
                        
                    case "_RimPower":
                        rimPower = prop;
                        break;
                        
                    case "_RimIntensity":
                        rimIntensity = prop;
                        break;
                        
                    case "_RimOffset":
                        rimOffset = prop;
                        break;
                    
                    // Advanced Feature Properties
                    case "_NormalMap":
                    case "_BumpMap":
                        normalMap = prop;
                        break;
                        
                    case "_NormalStrength":
                    case "_BumpScale":
                        normalStrength = prop;
                        break;
                        
                    case "_EmissionMap":
                        emissionMap = prop;
                        break;
                        
                    case "_EmissionColor":
                        emissionColor = prop;
                        break;
                        
                    case "_EmissionIntensity":
                        emissionIntensity = prop;
                        break;
                        
                    case "_DetailMap":
                        detailMap = prop;
                        break;
                        
                    case "_DetailNormalMap":
                        detailNormalMap = prop;
                        break;
                        
                    case "_DetailStrength":
                        detailStrength = prop;
                        break;
                    
                    // Subsurface Scattering Properties
                    case "_EnableSubsurface":
                        enableSubsurface = prop;
                        break;
                        
                    case "_SubsurfaceColor":
                        subsurfaceColor = prop;
                        break;
                        
                    case "_SubsurfaceIntensity":
                        subsurfaceIntensity = prop;
                        break;
                        
                    case "_SubsurfaceDistortion":
                        subsurfaceDistortion = prop;
                        break;
                        
                    case "_SubsurfacePower":
                        subsurfacePower = prop;
                        break;
                    
                    // Outline Properties
                    case "_EnableOutline":
                        enableOutline = prop;
                        break;
                        
                    case "_OutlineColor":
                        outlineColor = prop;
                        break;
                        
                    case "_OutlineWidth":
                        outlineWidth = prop;
                        break;
                    
                    // Wind Animation Properties
                    case "_EnableWind":
                        enableWind = prop;
                        break;
                        
                    case "_WindSpeed":
                        windSpeed = prop;
                        break;
                        
                    case "_WindStrength":
                        windStrength = prop;
                        break;
                        
                    case "_WindDirection":
                        windDirection = prop;
                        break;
                    
                    // Performance Properties
                    case "_ReceiveShadows":
                        receiveShadows = prop;
                        break;
                        
                    case "_EnableAdditionalLights":
                        enableAdditionalLights = prop;
                        break;
                        
                    case "_LightmapInfluence":
                        lightmapInfluence = prop;
                        break;
                }
            }
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
        /// Color property değerini güvenli şekilde alır
        /// </summary>
        public Color GetColorValue(MaterialProperty property)
        {
            if (property == null) return Color.white;
            
            if (property.type == MaterialProperty.PropType.Color)
            {
                return property.colorValue;
            }
            
            return Color.white;
        }

        /// <summary>
        /// Toggle property'yi güvenli şekilde set eder
        /// </summary>
        public void SetToggleValue(MaterialProperty property, bool value)
        {
            if (property == null) return;
            
            if (property.type == MaterialProperty.PropType.Float || 
                property.type == MaterialProperty.PropType.Range)
            {
                property.floatValue = value ? 1.0f : 0.0f;
            }
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
            if (IsPropertyValid(specularMode) && IsFeatureEnabled(enableHighlights))
            {
                var specMode = GetFloatValue(specularMode);
                SetKeyword(material, "_SPECULARMODE_STEPPED", specMode == 0f);
                SetKeyword(material, "_SPECULARMODE_SOFT", specMode == 1f);
                SetKeyword(material, "_SPECULARMODE_ANISOTROPIC", specMode == 2f);
                SetKeyword(material, "_SPECULARMODE_SPARKLE", specMode == 3f);
                SetKeyword(material, "_SPECULARMODE_DOUBLE_TONE", specMode == 4f);
            }
            else
            {
                SetKeyword(material, "_SPECULARMODE_STEPPED", false);
                SetKeyword(material, "_SPECULARMODE_SOFT", false);
                SetKeyword(material, "_SPECULARMODE_ANISOTROPIC", false);
                SetKeyword(material, "_SPECULARMODE_SPARKLE", false);
                SetKeyword(material, "_SPECULARMODE_DOUBLE_TONE", false);
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

        /// <summary>
        /// Tüm property'lerin listesini döndürür (debug için)
        /// </summary>
        public string[] GetAllPropertyNames()
        {
            var names = new System.Collections.Generic.List<string>();
            
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(MaterialProperty))
                {
                    var prop = field.GetValue(this) as MaterialProperty;
                    if (prop != null)
                    {
                        names.Add($"{field.Name}: {prop.name}");
                    }
                }
            }
            
            return names.ToArray();
        }
    }
}

