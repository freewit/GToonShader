using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AdvancedToonShaderGUI : ShaderGUI
{
    // Material Properties
    private MaterialProperty baseColor;
    private MaterialProperty baseMap;
    private MaterialProperty lightingMode;
    private MaterialProperty shadowSteps;
    private MaterialProperty shadowSmoothness;
    private MaterialProperty shadowRamp;
    private MaterialProperty shadowColor;
    private MaterialProperty shadowIntensity;
    private MaterialProperty shadowOffset;
    private MaterialProperty shadowDepthBias;
    private MaterialProperty shadowNormalBias;
    private MaterialProperty shadowSlopeBias;
    private MaterialProperty shadowDistanceFade;
    private MaterialProperty usePancaking;
    private MaterialProperty useAdaptiveBias;
    private MaterialProperty occlusionStrength;
    
    private MaterialProperty specularColor;
    private MaterialProperty specularSize;
    private MaterialProperty specularSmoothness;
    private MaterialProperty specularSteps;
    
    private MaterialProperty rimColor;
    private MaterialProperty rimPower;
    private MaterialProperty rimIntensity;
    private MaterialProperty rimOffset;
    
    private MaterialProperty normalMap;
    private MaterialProperty normalStrength;
    private MaterialProperty emissionMap;
    private MaterialProperty emissionColor;
    private MaterialProperty emissionIntensity;
    
    private MaterialProperty detailMap;
    private MaterialProperty detailNormalMap;
    private MaterialProperty detailStrength;
    
    private MaterialProperty subsurfaceColor;
    private MaterialProperty subsurfaceIntensity;
    private MaterialProperty subsurfaceDistortion;
    private MaterialProperty subsurfacePower;
    
    private MaterialProperty enableOutline;
    private MaterialProperty outlineColor;
    private MaterialProperty outlineWidth;
    
    private MaterialProperty enableWind;
    private MaterialProperty windSpeed;
    private MaterialProperty windStrength;
    private MaterialProperty windDirection;
    
    private MaterialProperty receiveShadows;
    private MaterialProperty enableAdditionalLights;
    private MaterialProperty lightmapInfluence;
    
    // GUI State
    private static bool showLighting = true;
    private static bool showAdvancedShadowBias = false;
    private static bool showHighlights = false;
    private static bool showRim = false;
    private static bool showAdvanced = false;
    private static bool showSubsurface = false;
    private static bool showOutline = false;
    private static bool showWind = false;
    private static bool showPerformance = false;
    
    // Styles
    private static GUIStyle headerStyle;
    private static GUIStyle versionStyle;
    private static GUIStyle sectionStyle;
    private static GUIStyle foldoutStyle;
    private static GUIStyle errorStyle;
    private static GUIStyle successStyle;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        InitializeStyles();
        FindProperties(properties);
        
        DrawHeader();
        EditorGUILayout.Space(5);
        
        DrawBaseProperties(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawLightingSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawAdvancedShadowBiasSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawHighlightsSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawRimSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawAdvancedSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawSubsurfaceSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawOutlineSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawWindSection(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawPerformanceSection(materialEditor);
        EditorGUILayout.Space(10);
        
        DrawTroubleshootingTips();
        EditorGUILayout.Space(5);
        
        DrawFooter();
    }
    
    void FindProperties(MaterialProperty[] props)
    {
        baseColor = FindProperty("_BaseColor", props);
        baseMap = FindProperty("_BaseMap", props);
        lightingMode = FindProperty("_LightingMode", props);
        shadowSteps = FindProperty("_ShadowSteps", props);
        shadowSmoothness = FindProperty("_ShadowSmoothness", props);
        shadowRamp = FindProperty("_ShadowRamp", props);
        shadowColor = FindProperty("_ShadowColor", props);
        shadowIntensity = FindProperty("_ShadowIntensity", props);
        shadowOffset = FindProperty("_ShadowOffset", props);
        shadowDepthBias = FindProperty("_ShadowDepthBias", props);
        shadowNormalBias = FindProperty("_ShadowNormalBias", props);
        shadowSlopeBias = FindProperty("_ShadowSlopeBias", props);
        shadowDistanceFade = FindProperty("_ShadowDistanceFade", props);
        usePancaking = FindProperty("_UsePancaking", props);
        useAdaptiveBias = FindProperty("_UseAdaptiveBias", props);
        occlusionStrength = FindProperty("_OcclusionStrength", props);
        
        specularColor = FindProperty("_SpecularColor", props);
        specularSize = FindProperty("_SpecularSize", props);
        specularSmoothness = FindProperty("_SpecularSmoothness", props);
        specularSteps = FindProperty("_SpecularSteps", props);
        
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
    
    void InitializeStyles()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.9f, 0.7f, 0.3f, 1f) }
            };
        }
        
        if (versionStyle == null)
        {
            versionStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 1f) },
                fontStyle = FontStyle.Italic
            };
        }
        
        if (sectionStyle == null)
        {
            sectionStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(0, 0, 2, 2)
            };
        }
        
        if (foldoutStyle == null)
        {
            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
        }
        
        if (errorStyle == null)
        {
            errorStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.red },
                fontSize = 11,
                wordWrap = true
            };
        }
        
        if (successStyle == null)
        {
            successStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.green },
                fontSize = 11,
                wordWrap = true
            };
        }
    }
    
    void DrawHeader()
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("⚡ Ultimate Toon Shader - Shadow Fix", headerStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField("Advanced Shadow Acne & Z-Fighting Solution v2.0", versionStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawBaseProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        EditorGUILayout.LabelField("🎨 Base Properties", EditorStyles.boldLabel);
        
        materialEditor.ColorProperty(baseColor, "Base Color");
        materialEditor.TextureProperty(baseMap, "Base Texture");
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawLightingSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showLighting = EditorGUILayout.Foldout(showLighting, "🌟 Lighting System", foldoutStyle);
        if (showLighting)
        {
            EditorGUI.BeginChangeCheck();
            int lightingModeValue = (int)lightingMode.floatValue;
            string[] lightingOptions = { "Stepped", "Smooth", "Ramp" };
            lightingModeValue = EditorGUILayout.Popup("Lighting Mode", lightingModeValue, lightingOptions);
            
            if (EditorGUI.EndChangeCheck())
            {
                lightingMode.floatValue = lightingModeValue;
                SetLightingKeywords(materialEditor.target as Material, lightingModeValue);
            }
            
            if (lightingModeValue == 0 || lightingModeValue == 1) // Stepped or Smooth
            {
                materialEditor.RangeProperty(shadowSteps, "Shadow Steps");
                materialEditor.RangeProperty(shadowSmoothness, "Shadow Smoothness");
            }
            else if (lightingModeValue == 2) // Ramp
            {
                materialEditor.TextureProperty(shadowRamp, "Shadow Ramp", false);
            }
            
            materialEditor.ColorProperty(shadowColor, "Shadow Color");
            materialEditor.RangeProperty(shadowIntensity, "Shadow Intensity");
            materialEditor.RangeProperty(shadowOffset, "Shadow Offset");
            materialEditor.RangeProperty(occlusionStrength, "Occlusion Strength");
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawAdvancedShadowBiasSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showAdvancedShadowBias = EditorGUILayout.Foldout(showAdvancedShadowBias, "🛠️ Advanced Shadow Fix (Anti Z-Fighting)", foldoutStyle);
        if (showAdvancedShadowBias)
        {
            EditorGUILayout.HelpBox("Bu gelişmiş ayarlar Z-fighting ve shadow acne problemlerini çözer. Değerleri kademeli olarak artırın.", MessageType.Info);
            
            // Advanced toggles
            EditorGUI.BeginChangeCheck();
            bool adaptiveBias = useAdaptiveBias.floatValue > 0.5f;
            bool pancaking = usePancaking.floatValue > 0.5f;
            
            adaptiveBias = EditorGUILayout.Toggle(new GUIContent("Use Adaptive Bias", "Yüzey açısına göre otomatik bias ayarı"), adaptiveBias);
            pancaking = EditorGUILayout.Toggle(new GUIContent("Use Shadow Pancaking", "Arka yüzlerdeki shadow acne'yi önler"), pancaking);
            
            if (EditorGUI.EndChangeCheck())
            {
                useAdaptiveBias.floatValue = adaptiveBias ? 1.0f : 0.0f;
                usePancaking.floatValue = pancaking ? 1.0f : 0.0f;
                SetAdvancedShadowKeywords(materialEditor.target as Material, adaptiveBias, pancaking);
            }
            
            EditorGUILayout.Space(5);
            
            // Bias parameters
            materialEditor.RangeProperty(shadowDepthBias, "Shadow Depth Bias");
            EditorGUILayout.LabelField("• Düz yüzeylerdeki shadow acne'yi düzeltir", EditorStyles.miniLabel);
            
            materialEditor.RangeProperty(shadowNormalBias, "Shadow Normal Bias");
            EditorGUILayout.LabelField("• Kavisli yüzeylerdeki shadow acne'yi düzeltir", EditorStyles.miniLabel);
            
            materialEditor.RangeProperty(shadowSlopeBias, "Shadow Slope Bias");
            EditorGUILayout.LabelField("• Eğimli yüzeylerdeki problemleri çözer", EditorStyles.miniLabel);
            
            materialEditor.RangeProperty(shadowDistanceFade, "Shadow Distance Fade");
            EditorGUILayout.LabelField("• Uzak nesnelerde gölgeleri yumuşatır", EditorStyles.miniLabel);
            
            EditorGUILayout.Space(10);
            
            // Smart preset buttons
            EditorGUILayout.LabelField("🎯 Akıllı Presetler:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Minimal Fix"))
            {
                ApplyShadowPreset(0.1f, 0.5f, 0.2f, 0.9f, false, false);
                SetAdvancedShadowKeywords(materialEditor.target as Material, false, false);
            }
            
            if (GUILayout.Button("Standard Fix"))
            {
                ApplyShadowPreset(0.5f, 1.0f, 0.5f, 0.8f, true, false);
                SetAdvancedShadowKeywords(materialEditor.target as Material, true, false);
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Aggressive Fix"))
            {
                ApplyShadowPreset(1.0f, 2.0f, 1.0f, 0.7f, true, true);
                SetAdvancedShadowKeywords(materialEditor.target as Material, true, true);
            }
            
            if (GUILayout.Button("Maximum Fix"))
            {
                ApplyShadowPreset(2.0f, 3.0f, 1.5f, 0.6f, true, true);
                SetAdvancedShadowKeywords(materialEditor.target as Material, true, true);
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Reset"))
            {
                ApplyShadowPreset(0.5f, 2.0f, 1.0f, 0.8f, true, true);
                SetAdvancedShadowKeywords(materialEditor.target as Material, true, true);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Performance impact indicator
            float totalBias = shadowDepthBias.floatValue + shadowNormalBias.floatValue + shadowSlopeBias.floatValue;
            if (totalBias < 1.0f)
            {
                EditorGUILayout.LabelField("✅ Performans Etkisi: Düşük", successStyle);
            }
            else if (totalBias < 3.0f)
            {
                EditorGUILayout.LabelField("⚠️ Performans Etkisi: Orta", EditorStyles.label);
            }
            else
            {
                EditorGUILayout.LabelField("⚠️ Performans Etkisi: Yüksek", errorStyle);
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void ApplyShadowPreset(float depthBias, float normalBias, float slopeBias, float distanceFade, bool adaptive, bool pancaking)
    {
        shadowDepthBias.floatValue = depthBias;
        shadowNormalBias.floatValue = normalBias;
        shadowSlopeBias.floatValue = slopeBias;
        shadowDistanceFade.floatValue = distanceFade;
        useAdaptiveBias.floatValue = adaptive ? 1.0f : 0.0f;
        usePancaking.floatValue = pancaking ? 1.0f : 0.0f;
    }
    
    void DrawHighlightsSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showHighlights = EditorGUILayout.Foldout(showHighlights, "✨ Highlight System", foldoutStyle);
        if (showHighlights)
        {
            materialEditor.ColorProperty(specularColor, "Specular Color");
            materialEditor.RangeProperty(specularSize, "Specular Size");
            materialEditor.RangeProperty(specularSmoothness, "Specular Smoothness");
            materialEditor.RangeProperty(specularSteps, "Specular Steps");
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawRimSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showRim = EditorGUILayout.Foldout(showRim, "🌅 Rim Lighting", foldoutStyle);
        if (showRim)
        {
            materialEditor.ColorProperty(rimColor, "Rim Color");
            materialEditor.RangeProperty(rimPower, "Rim Power");
            materialEditor.RangeProperty(rimIntensity, "Rim Intensity");
            materialEditor.RangeProperty(rimOffset, "Rim Offset");
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawAdvancedSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "🔧 Advanced Features", foldoutStyle);
        if (showAdvanced)
        {
            EditorGUILayout.LabelField("Normal Mapping", EditorStyles.miniBoldLabel);
            materialEditor.TextureProperty(normalMap, "Normal Map");
            if (normalMap.textureValue != null)
                materialEditor.RangeProperty(normalStrength, "Normal Strength");
            
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Emission", EditorStyles.miniBoldLabel);
            materialEditor.TextureProperty(emissionMap, "Emission Map");
            materialEditor.ColorProperty(emissionColor, "Emission Color");
            materialEditor.RangeProperty(emissionIntensity, "Emission Intensity");
            
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Detail Textures", EditorStyles.miniBoldLabel);
            materialEditor.TextureProperty(detailMap, "Detail Albedo");
            materialEditor.TextureProperty(detailNormalMap, "Detail Normal");
            materialEditor.RangeProperty(detailStrength, "Detail Strength");
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawSubsurfaceSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showSubsurface = EditorGUILayout.Foldout(showSubsurface, "🔴 Subsurface Scattering", foldoutStyle);
        if (showSubsurface)
        {
            materialEditor.ColorProperty(subsurfaceColor, "Subsurface Color");
            materialEditor.RangeProperty(subsurfaceIntensity, "Intensity");
            materialEditor.RangeProperty(subsurfaceDistortion, "Distortion");
            materialEditor.RangeProperty(subsurfacePower, "Power");
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawOutlineSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showOutline = EditorGUILayout.Foldout(showOutline, "⭕ Outline", foldoutStyle);
        if (showOutline)
        {
            EditorGUI.BeginChangeCheck();
            bool outlineEnabled = enableOutline.floatValue > 0.5f;
            outlineEnabled = EditorGUILayout.Toggle("Enable Outline", outlineEnabled);
            
            if (EditorGUI.EndChangeCheck())
            {
                enableOutline.floatValue = outlineEnabled ? 1.0f : 0.0f;
                SetOutlineKeyword(materialEditor.target as Material, outlineEnabled);
            }
            
            if (outlineEnabled)
            {
                materialEditor.ColorProperty(outlineColor, "Outline Color");
                materialEditor.RangeProperty(outlineWidth, "Outline Width");
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawWindSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showWind = EditorGUILayout.Foldout(showWind, "🌪️ Wind Animation", foldoutStyle);
        if (showWind)
        {
            EditorGUI.BeginChangeCheck();
            bool windEnabled = enableWind.floatValue > 0.5f;
            windEnabled = EditorGUILayout.Toggle("Enable Wind", windEnabled);
            
            if (EditorGUI.EndChangeCheck())
            {
                enableWind.floatValue = windEnabled ? 1.0f : 0.0f;
                SetWindKeyword(materialEditor.target as Material, windEnabled);
            }
            
            if (windEnabled)
            {
                materialEditor.RangeProperty(windSpeed, "Wind Speed");
                materialEditor.RangeProperty(windStrength, "Wind Strength");
                materialEditor.VectorProperty(windDirection, "Wind Direction");
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawPerformanceSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        showPerformance = EditorGUILayout.Foldout(showPerformance, "⚡ Performance", foldoutStyle);
        if (showPerformance)
        {
            EditorGUI.BeginChangeCheck();
            bool shadowsEnabled = receiveShadows.floatValue > 0.5f;
            bool additionalLightsEnabled = enableAdditionalLights.floatValue > 0.5f;
            
            shadowsEnabled = EditorGUILayout.Toggle("Receive Shadows", shadowsEnabled);
            additionalLightsEnabled = EditorGUILayout.Toggle("Additional Lights", additionalLightsEnabled);
            
            if (EditorGUI.EndChangeCheck())
            {
                receiveShadows.floatValue = shadowsEnabled ? 1.0f : 0.0f;
                enableAdditionalLights.floatValue = additionalLightsEnabled ? 1.0f : 0.0f;
                SetPerformanceKeywords(materialEditor.target as Material, shadowsEnabled, additionalLightsEnabled);
            }
            
            materialEditor.RangeProperty(lightmapInfluence, "Lightmap Influence");
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawTroubleshootingTips()
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        EditorGUILayout.LabelField("🔧 Sorun Giderme Rehberi", EditorStyles.boldLabel);
        
        GUIStyle tipStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            wordWrap = true,
            normal = { textColor = new Color(0.6f, 0.6f, 0.6f, 1f) }
        };
        
        EditorGUILayout.LabelField("🔍 Shadow Acne (Gölge Lekesi):", EditorStyles.miniBoldLabel);
        EditorGUILayout.LabelField("• 'Standard Fix' preset'ini deneyin", tipStyle);
        EditorGUILayout.LabelField("• Shadow Depth Bias'ı 0.5-1.0 arasında ayarlayın", tipStyle);
        
        EditorGUILayout.Space(3);
        EditorGUILayout.LabelField("🔍 Z-Fighting (Yüzey Titreşimi):", EditorStyles.miniBoldLabel);
        EditorGUILayout.LabelField("• 'Aggressive Fix' preset'ini kullanın", tipStyle);
        EditorGUILayout.LabelField("• Use Adaptive Bias'ı aktif edin", tipStyle);
        
        EditorGUILayout.Space(3);
        EditorGUILayout.LabelField("🔍 Light Leaking (Işık Sızması):", EditorStyles.miniBoldLabel);
        EditorGUILayout.LabelField("• Bias değerlerini düşürün", tipStyle);
        EditorGUILayout.LabelField("• Shadow Distance Fade'i artırın", tipStyle);
        
        EditorGUILayout.Space(3);
        EditorGUILayout.LabelField("🔍 Performance Sorunları:", EditorStyles.miniBoldLabel);
        EditorGUILayout.LabelField("• 'Minimal Fix' preset'ini kullanın", tipStyle);
        EditorGUILayout.LabelField("• Use Shadow Pancaking'i kapatın", tipStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawFooter()
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        EditorGUILayout.LabelField("💡 Pro Tips:", EditorStyles.miniBoldLabel);
        
        GUIStyle tipStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            wordWrap = true,
            normal = { textColor = new Color(0.6f, 0.6f, 0.6f, 1f) }
        };
        
        EditorGUILayout.LabelField("• Her sahne için farklı ayarlar gerekebilir", tipStyle);
        EditorGUILayout.LabelField("• Önce 'Standard Fix' ile başlayın", tipStyle);
        EditorGUILayout.LabelField("• URP Asset'inde Shadow Distance'ı 50-100 arası tutun", tipStyle);
        EditorGUILayout.LabelField("• Light'ın Shadow Bias ayarlarını da kontrol edin", tipStyle);
        EditorGUILayout.LabelField("• Mobil için daha düşük bias değerleri kullanın", tipStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    // Keyword management methods
    void SetLightingKeywords(Material material, int mode)
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
    
    void SetAdvancedShadowKeywords(Material material, bool adaptiveBias, bool pancaking)
    {
        if (adaptiveBias)
            material.EnableKeyword("_USEADAPTIVEBIAS_ON");
        else
            material.DisableKeyword("_USEADAPTIVEBIAS_ON");
            
        if (pancaking)
            material.EnableKeyword("_USEPANCAKING_ON");
        else
            material.DisableKeyword("_USEPANCAKING_ON");
    }
    
    void SetOutlineKeyword(Material material, bool enabled)
    {
        if (enabled)
            material.EnableKeyword("_ENABLEOUTLINE_ON");
        else
            material.DisableKeyword("_ENABLEOUTLINE_ON");
    }
    
    void SetWindKeyword(Material material, bool enabled)
    {
        if (enabled)
            material.EnableKeyword("_ENABLEWIND_ON");
        else
            material.DisableKeyword("_ENABLEWIND_ON");
    }
    
    void SetPerformanceKeywords(Material material, bool shadows, bool additionalLights)
    {
        if (shadows)
            material.EnableKeyword("_RECEIVESHADOWS_ON");
        else
            material.DisableKeyword("_RECEIVESHADOWS_ON");
            
        if (additionalLights)
            material.EnableKeyword("_ENABLEADDITIONALLIGHTS_ON");
        else
            material.DisableKeyword("_ENABLEADDITIONALLIGHTS_ON");
    }
}