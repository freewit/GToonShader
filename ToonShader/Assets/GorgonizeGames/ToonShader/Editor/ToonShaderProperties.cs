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
        public MaterialProperty shadowSteps;
        public MaterialProperty shadowSmoothness;
        public MaterialProperty shadowRamp;
        public MaterialProperty shadowColor;
        public MaterialProperty shadowIntensity;
        public MaterialProperty shadowOffset;
        public MaterialProperty occlusionStrength;
        
        // Highlight Properties
        public MaterialProperty enableHighlights;
        public MaterialProperty specularColor;
        public MaterialProperty specularSize;
        public MaterialProperty specularSmoothness;
        public MaterialProperty specularSteps;
        
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
                        
                    case "_TintShadowOnBase":
                        tintShadowOnBase = prop;
                        break;
                        
                    case "_ShadowSteps":
                        shadowSteps = prop;
                        break;
                        
                    case "_ShadowSmoothness":
                        shadowSmoothness = prop;
                        break;
                        
                    case "_ShadowRamp":
                        shadowRamp = prop;
                        break;
                        
                    case "_ShadowColor":
                        shadowColor = prop;
                        break;
                        
                    case "_ShadowIntensity":
                        shadowIntensity = prop;
                        break;
                        
                    case "_ShadowOffset":
                        shadowOffset = prop;
                        break;
                        
                    case "_OcclusionStrength":
                        occlusionStrength = prop;
                        break;
                    
                    // Highlight Properties
                    case "_EnableHighlights":
                        enableHighlights = prop;
                        break;
                        
                    case "_SpecularColor":
                        specularColor = prop;
                        break;
                        
                    case "_SpecularSize":
                    case "_Glossiness":
                        specularSize = prop;
                        break;
                        
                    case "_SpecularSmoothness":
                        specularSmoothness = prop;
                        break;
                        
                    case "_SpecularSteps":
                        specularSteps = prop;
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
                    case "_BumpMap":
                    case "_NormalMap":
                        normalMap = prop;
                        break;
                        
                    case "_BumpScale":
                    case "_NormalStrength":
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
                        
                    case "_DetailAlbedoMap":
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
        /// Verilen property'nin aktif olup olmadığını kontrol eder
        /// </summary>
        public bool IsFeatureEnabled(MaterialProperty property)
        {
            return property != null && property.floatValue > 0.5f;
        }

        /// <summary>
        /// Feature'ı aktif/pasif yapar
        /// </summary>
        public void SetFeatureEnabled(MaterialProperty property, bool enabled)
        {
            if (property != null)
            {
                property.floatValue = enabled ? 1.0f : 0.0f;
            }
        }

        /// <summary>
        /// Shader keyword'lerini günceller
        /// </summary>
        public void UpdateKeywords(Material material)
        {
            // Highlights
            SetKeyword(material, "_ENABLE_HIGHLIGHTS", IsFeatureEnabled(enableHighlights));
            
            // Rim Lighting
            SetKeyword(material, "_ENABLE_RIM", IsFeatureEnabled(enableRim));
            
            // Subsurface Scattering
            SetKeyword(material, "_ENABLE_SUBSURFACE", IsFeatureEnabled(enableSubsurface));
            
            // Outline
            SetKeyword(material, "_ENABLE_OUTLINE", IsFeatureEnabled(enableOutline));
            
            // Wind Animation
            SetKeyword(material, "_ENABLE_WIND", IsFeatureEnabled(enableWind));
            
            // Normal Map
            SetKeyword(material, "_NORMALMAP", normalMap?.textureValue != null);
            
            // Emission
            bool hasEmission = emissionMap?.textureValue != null || 
                              (emissionColor != null && emissionColor.colorValue.maxColorComponent > 0);
            SetKeyword(material, "_EMISSION", hasEmission);
            
            // Detail Maps
            SetKeyword(material, "_DETAIL_MULX2", detailMap?.textureValue != null);
            
            // Additional Lights
            SetKeyword(material, "_ADDITIONAL_LIGHTS", IsFeatureEnabled(enableAdditionalLights));
            
            // Receive Shadows
            SetKeyword(material, "_RECEIVE_SHADOWS_OFF", !IsFeatureEnabled(receiveShadows));
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