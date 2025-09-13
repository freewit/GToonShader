using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Toon Shader için tüm materyal özelliklerine referansları bulur ve tutar.
    /// </summary>
    public class ToonShaderProperties
    {
        // Base
        public MaterialProperty baseColor;
        public MaterialProperty baseMap;

        // Lighting
        public MaterialProperty lightingMode;
        public MaterialProperty tintShadowOnBase;
        public MaterialProperty shadowSteps;
        public MaterialProperty shadowSmoothness;
        public MaterialProperty shadowRamp;
        public MaterialProperty shadowColor;
        public MaterialProperty shadowIntensity;
        public MaterialProperty shadowOffset;
        public MaterialProperty occlusionStrength;
        
        // Highlights
        public MaterialProperty enableHighlights;
        public MaterialProperty specularColor;
        public MaterialProperty specularSize;
        public MaterialProperty specularSmoothness;
        public MaterialProperty specularSteps;
        
        // Rim
        public MaterialProperty enableRim;
        public MaterialProperty rimColor;
        public MaterialProperty rimPower;
        public MaterialProperty rimIntensity;
        public MaterialProperty rimOffset;
        
        // Advanced Features
        public MaterialProperty normalMap;
        public MaterialProperty normalStrength;
        public MaterialProperty emissionMap;
        public MaterialProperty emissionColor;
        public MaterialProperty emissionIntensity;
        public MaterialProperty detailMap;
        public MaterialProperty detailNormalMap;
        public MaterialProperty detailStrength;
        
        // Subsurface
        public MaterialProperty enableSubsurface;
        public MaterialProperty subsurfaceColor;
        public MaterialProperty subsurfaceIntensity;
        public MaterialProperty subsurfaceDistortion;
        public MaterialProperty subsurfacePower;
        
        // Outline
        public MaterialProperty enableOutline;
        public MaterialProperty outlineColor;
        public MaterialProperty outlineWidth;
        
        // Wind
        public MaterialProperty enableWind;
        public MaterialProperty windSpeed;
        public MaterialProperty windStrength;
        public MaterialProperty windDirection;
        
        // Performance
        public MaterialProperty receiveShadows;
        public MaterialProperty enableAdditionalLights;
        public MaterialProperty lightmapInfluence;

        public ToonShaderProperties(MaterialProperty[] props)
        {
            baseColor = FindProperty("_BaseColor", props);
            baseMap = FindProperty("_BaseMap", props);
            lightingMode = FindProperty("_LightingMode", props);
            tintShadowOnBase = FindProperty("_TintShadowOnBase", props);
            shadowSteps = FindProperty("_ShadowSteps", props);
            shadowSmoothness = FindProperty("_ShadowSmoothness", props);
            shadowRamp = FindProperty("_ShadowRamp", props);
            shadowColor = FindProperty("_ShadowColor", props);
            shadowIntensity = FindProperty("_ShadowIntensity", props);
            shadowOffset = FindProperty("_ShadowOffset", props);
            occlusionStrength = FindProperty("_OcclusionStrength", props);
            enableHighlights = FindProperty("_EnableHighlights", props);
            specularColor = FindProperty("_SpecularColor", props);
            specularSize = FindProperty("_SpecularSize", props);
            specularSmoothness = FindProperty("_SpecularSmoothness", props);
            specularSteps = FindProperty("_SpecularSteps", props);
            enableRim = FindProperty("_EnableRim", props);
            rimColor = FindProperty("_RimColor", props);
            rimPower = FindProperty("_RimPower", props);
            rimIntensity = FindProperty("_RimIntensity", props);
            rimOffset = FindProperty("_RimOffset", props);
            normalMap = FindProperty("_NormalMap", props);
            normalStrength = FindProperty("_NormalStrength", props);
            emissionMap = FindProperty("_EmissionMap", props);
            emissionColor = FindProperty("_EmissionColor", props);
            emissionIntensity = FindProperty("_EmissionIntensity", props);
            detailMap = FindProperty("_DetailMap", props);
            detailNormalMap = FindProperty("_DetailNormalMap", props);
            detailStrength = FindProperty("_DetailStrength", props);
            enableSubsurface = FindProperty("_EnableSubsurface", props);
            subsurfaceColor = FindProperty("_SubsurfaceColor", props);
            subsurfaceIntensity = FindProperty("_SubsurfaceIntensity", props);
            subsurfaceDistortion = FindProperty("_SubsurfaceDistortion", props);
            subsurfacePower = FindProperty("_SubsurfacePower", props);
            enableOutline = FindProperty("_EnableOutline", props);
            outlineColor = FindProperty("_OutlineColor", props);
            outlineWidth = FindProperty("_OutlineWidth", props);
            enableWind = FindProperty("_EnableWind", props);
            windSpeed = FindProperty("_WindSpeed", props);
            windStrength = FindProperty("_WindStrength", props);
            windDirection = FindProperty("_WindDirection", props);
            receiveShadows = FindProperty("_ReceiveShadows", props);
            enableAdditionalLights = FindProperty("_EnableAdditionalLights", props);
            lightmapInfluence = FindProperty("_LightmapInfluence", props);
        }

        private MaterialProperty FindProperty(string name, MaterialProperty[] props)
        {
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] != null && props[i].name == name)
                    return props[i];
            }
            return null;
        }
    }
    
    /// <summary>
    /// Toon Shader arayüzü için özel GUIStyle'ları yönetir.
    /// </summary>
    public static class ToonShaderStyles
    {
        public static GUIStyle headerStyle;
        public static GUIStyle versionStyle;
        public static GUIStyle sectionStyle;
        public static GUIStyle headerSectionStyle;
        public static GUIStyle foldoutStyle;
        public static GUIStyle errorStyle;
        public static GUIStyle successStyle;
        public static Texture2D logoTexture;
        private static bool isInitialized = false;

        // GUI elemanları için tek renkli bir doku oluşturan yardımcı metot
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.hideFlags = HideFlags.HideAndDontSave; // Editör arayüzü için önemli
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public static void Initialize()
        {
            if (isInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel) 
            { 
                fontSize = 20, 
                alignment = TextAnchor.MiddleCenter, 
                normal = { textColor = new Color(0.9f, 0.7f, 0.3f, 1f) },
                wordWrap = true
            };
            
            versionStyle = new GUIStyle(EditorStyles.label) 
            { 
                fontSize = 12, 
                alignment = TextAnchor.MiddleCenter, 
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 1f) }, 
                fontStyle = FontStyle.Italic 
            };

            // Geliştirilmiş, koyu tema ve daha profesyonel bir görünüme sahip bölüm stili
            sectionStyle = new GUIStyle(EditorStyles.helpBox) 
            { 
                padding = new RectOffset(12, 12, 12, 12), 
                margin = new RectOffset(4, 4, 6, 6) 
            };
            sectionStyle.normal.background = MakeTex(1, 1, new Color(0.18f, 0.19f, 0.21f, 1f));

            // Başlık için biraz daha farklı bir stil
            headerSectionStyle = new GUIStyle(GUI.skin.box) 
            { 
                padding = new RectOffset(8, 8, 8, 8), 
                margin = new RectOffset(0, 0, 0, 4) 
            };
            headerSectionStyle.normal.background = MakeTex(1, 1, new Color(0.15f, 0.15f, 0.15f, 1f));

            // Geliştirilmiş, daha okunaklı Foldout (açılır menü) stili
            foldoutStyle = new GUIStyle(EditorStyles.foldout) 
            { 
                fontSize = 13, 
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(15, 0, 1, 1) 
            };
            Color foldoutTextColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            foldoutStyle.normal.textColor = foldoutTextColor;
            foldoutStyle.onNormal.textColor = foldoutTextColor;
            foldoutStyle.hover.textColor = Color.white;
            foldoutStyle.onHover.textColor = Color.white;
            foldoutStyle.focused.textColor = Color.white;
            foldoutStyle.onFocused.textColor = Color.white;
            foldoutStyle.active.textColor = Color.white;
            foldoutStyle.onActive.textColor = Color.white;
            
            errorStyle = new GUIStyle(EditorStyles.label) 
            { 
                normal = { textColor = Color.red }, 
                fontSize = 11, 
                wordWrap = true 
            };
            
            successStyle = new GUIStyle(EditorStyles.label) 
            { 
                normal = { textColor = Color.green }, 
                fontSize = 11, 
                wordWrap = true 
            };
            
            logoTexture = Resources.Load<Texture2D>("GorgonizeLogo");

            isInitialized = true;
        }
    }
    
    /// <summary>
    /// Toon Shader için shader anahtar kelimelerinin ayarlanmasını yönetir.
    /// </summary>
    public static class ToonShaderKeywords
    {
        public static void SetLightingKeywords(Material material, int mode)
        {
            SetKeyword(material, "_LIGHTINGMODE_STEPPED", mode == 0);
            SetKeyword(material, "_LIGHTINGMODE_SMOOTH", mode == 1);
            SetKeyword(material, "_LIGHTINGMODE_RAMP", mode == 2);
        }
    
        public static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (enabled)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
    }
}
