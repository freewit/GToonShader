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
        public MaterialProperty shadowSteps;
        public MaterialProperty shadowSmoothness;
        public MaterialProperty shadowRamp;
        public MaterialProperty shadowColor;
        public MaterialProperty shadowIntensity;
        public MaterialProperty shadowOffset;
        public MaterialProperty occlusionStrength;
        
        // Advanced Shadow
        public MaterialProperty shadowDepthBias;
        public MaterialProperty shadowNormalBias;
        public MaterialProperty shadowSlopeBias;
        public MaterialProperty shadowDistanceFade;
        public MaterialProperty usePancaking;
        public MaterialProperty useAdaptiveBias;

        // Highlights
        public MaterialProperty specularColor;
        public MaterialProperty specularSize;
        public MaterialProperty specularSmoothness;
        public MaterialProperty specularSteps;
        
        // Rim
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
            // HATA DÜZELTMESİ: 'ShaderGUI.FindProperty' metodunu kullanmak yerine,
            // koruma seviyesi hatasını önlemek için özellikleri manuel olarak buluyoruz.
            foreach (var prop in props)
            {
                switch (prop.name)
                {
                    case "_BaseColor": baseColor = prop; break;
                    case "_BaseMap": baseMap = prop; break;
                    case "_LightingMode": lightingMode = prop; break;
                    case "_ShadowSteps": shadowSteps = prop; break;
                    case "_ShadowSmoothness": shadowSmoothness = prop; break;
                    case "_ShadowRamp": shadowRamp = prop; break;
                    case "_ShadowColor": shadowColor = prop; break;
                    case "_ShadowIntensity": shadowIntensity = prop; break;
                    case "_ShadowOffset": shadowOffset = prop; break;
                    case "_OcclusionStrength": occlusionStrength = prop; break;
                    case "_ShadowDepthBias": shadowDepthBias = prop; break;
                    case "_ShadowNormalBias": shadowNormalBias = prop; break;
                    case "_ShadowSlopeBias": shadowSlopeBias = prop; break;
                    case "_ShadowDistanceFade": shadowDistanceFade = prop; break;
                    case "_UsePancaking": usePancaking = prop; break;
                    case "_UseAdaptiveBias": useAdaptiveBias = prop; break;
                    case "_SpecularColor": specularColor = prop; break;
                    case "_SpecularSize": specularSize = prop; break;
                    case "_SpecularSmoothness": specularSmoothness = prop; break;
                    case "_SpecularSteps": specularSteps = prop; break;
                    case "_RimColor": rimColor = prop; break;
                    case "_RimPower": rimPower = prop; break;
                    case "_RimIntensity": rimIntensity = prop; break;
                    case "_RimOffset": rimOffset = prop; break;
                    case "_NormalMap": normalMap = prop; break;
                    case "_NormalStrength": normalStrength = prop; break;
                    case "_EmissionMap": emissionMap = prop; break;
                    case "_EmissionColor": emissionColor = prop; break;
                    case "_EmissionIntensity": emissionIntensity = prop; break;
                    case "_DetailMap": detailMap = prop; break;
                    case "_DetailNormalMap": detailNormalMap = prop; break;
                    case "_DetailStrength": detailStrength = prop; break;
                    case "_SubsurfaceColor": subsurfaceColor = prop; break;
                    case "_SubsurfaceIntensity": subsurfaceIntensity = prop; break;
                    case "_SubsurfaceDistortion": subsurfaceDistortion = prop; break;
                    case "_SubsurfacePower": subsurfacePower = prop; break;
                    case "_EnableOutline": enableOutline = prop; break;
                    case "_OutlineColor": outlineColor = prop; break;
                    case "_OutlineWidth": outlineWidth = prop; break;
                    case "_EnableWind": enableWind = prop; break;
                    case "_WindSpeed": windSpeed = prop; break;
                    case "_WindStrength": windStrength = prop; break;
                    case "_WindDirection": windDirection = prop; break;
                    case "_ReceiveShadows": receiveShadows = prop; break;
                    case "_EnableAdditionalLights": enableAdditionalLights = prop; break;
                    case "_LightmapInfluence": lightmapInfluence = prop; break;
                }
            }
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
        public static GUIStyle foldoutStyle;
        public static GUIStyle errorStyle;
        public static GUIStyle successStyle;
        public static Texture2D logoTexture; // Logo için Texture2D alanı
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (isInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel) 
            { 
                fontSize = 20, 
                alignment = TextAnchor.MiddleCenter, 
                normal = { textColor = new Color(0.9f, 0.7f, 0.3f, 1f) },
                wordWrap = true // Yazının kaybolmasını engellemek için
            };
            versionStyle = new GUIStyle(EditorStyles.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter, normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 1f) }, fontStyle = FontStyle.Italic };
            sectionStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(8, 8, 4, 4), margin = new RectOffset(0, 0, 2, 2) };
            foldoutStyle = new GUIStyle(EditorStyles.foldout) { fontSize = 13, fontStyle = FontStyle.Bold };
            errorStyle = new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red }, fontSize = 11, wordWrap = true };
            successStyle = new GUIStyle(EditorStyles.label) { normal = { textColor = Color.green }, fontSize = 11, wordWrap = true };
            
            // Logoyu Resources klasöründen yükle
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
            material.DisableKeyword("_LIGHTINGMODE_STEPPED");
            material.DisableKeyword("_LIGHTINGMODE_SMOOTH");
            material.DisableKeyword("_LIGHTINGMODE_RAMP");
            
            switch (mode)
            {
                case 0: material.EnableKeyword("_LIGHTINGMODE_STEPPED"); break;
                case 1: material.EnableKeyword("_LIGHTINGMODE_SMOOTH"); break;
                case 2: material.EnableKeyword("_LIGHTINGMODE_RAMP"); break;
            }
        }
    
        public static void SetAdvancedShadowKeywords(Material material, bool adaptiveBias, bool pancaking)
        {
            SetKeyword(material, "_USEADAPTIVEBIAS_ON", adaptiveBias);
            SetKeyword(material, "_USEPANCAKING_ON", pancaking);
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


