using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class UltimateToonPreset
{
    public string presetName;
    public Color baseColor;
    public string baseMapPath;
    public int lightingMode;
    public float shadowSteps;
    public float shadowSmoothness;
    public string shadowRampPath;
    public Color shadowColor;
    public float shadowIntensity;
    public float shadowOffset;
    public Color specularColor;
    public float specularSize;
    public float specularSmoothness;
    public float specularSteps;
    public Color rimColor;
    public float rimPower;
    public float rimIntensity;
    public float rimOffset;
    public float normalStrength;
    public Color emissionColor;
    public float emissionIntensity;
    public Color subsurfaceColor;
    public float subsurfaceIntensity;
    public bool enableOutline;
    public Color outlineColor;
    public float outlineWidth;
    public bool enableWind;
    public float windSpeed;
    public float windStrength;
}

public class UltimateToonShaderGUI : ShaderGUI
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
    private static bool showHighlights = false;
    private static bool showRim = false;
    private static bool showAdvanced = false;
    private static bool showSubsurface = false;
    private static bool showOutline = false;
    private static bool showWind = false;
    private static bool showPerformance = false;
    
    // Preset System
    private static string newPresetName = "";
    private static List<string> availablePresets = new List<string>();
    private static int selectedPresetIndex = 0;
    private const string PRESET_FOLDER = "Assets/GorgonizeGames/ToonShader/Presets";
    
    // Styles
    private static GUIStyle headerStyle;
    private static GUIStyle versionStyle;
    private static GUIStyle sectionStyle;
    private static GUIStyle foldoutStyle;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        InitializeStyles();
        FindProperties(properties);
        
        DrawHeader();
        EditorGUILayout.Space(5);
        
        DrawPresetSystem(materialEditor);
        EditorGUILayout.Space(5);
        
        DrawBaseProperties(materialEditor);
        EditorGUILayout.Space(3);
        
        DrawLightingSection(materialEditor);
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
    }
    
    void DrawHeader()
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("⚡ Ultimate Toon Shader", headerStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.LabelField("Professional Toon Rendering System v1.0", versionStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawPresetSystem(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(sectionStyle);
        
        EditorGUILayout.LabelField("🎛️ Preset Management", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save:", GUILayout.Width(40));
        newPresetName = EditorGUILayout.TextField(newPresetName);
        
        GUI.enabled = !string.IsNullOrEmpty(newPresetName);
        if (GUILayout.Button("💾", GUILayout.Width(30)))
        {
            SavePreset(materialEditor.target as Material, newPresetName);
            newPresetName = "";
            RefreshPresetList();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        RefreshPresetList();
        if (availablePresets.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Load:", GUILayout.Width(40));
            selectedPresetIndex = EditorGUILayout.Popup(selectedPresetIndex, availablePresets.ToArray());
            
            if (GUILayout.Button("📂", GUILayout.Width(30)))
            {
                LoadPreset(materialEditor.target as Material, availablePresets[selectedPresetIndex]);
            }
            
            if (GUILayout.Button("🗑️", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Delete Preset", 
                    "Delete '" + availablePresets[selectedPresetIndex] + "'?", "Delete", "Cancel"))
                {
                    DeletePreset(availablePresets[selectedPresetIndex]);
                    RefreshPresetList();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
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
        
        EditorGUILayout.LabelField("• Use Stepped mode for classic cell shading", tipStyle);
        EditorGUILayout.LabelField("• Smooth mode blends between stepped and realistic", tipStyle);
        EditorGUILayout.LabelField("• Ramp mode gives full artistic control via textures", tipStyle);
        EditorGUILayout.LabelField("• Enable outline for comic book style", tipStyle);
        EditorGUILayout.LabelField("• Subsurface scattering enhances skin and translucent materials", tipStyle);
        
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
    
    // Preset System Implementation
    void RefreshPresetList()
    {
        availablePresets.Clear();
        
        if (!Directory.Exists(PRESET_FOLDER))
        {
            Directory.CreateDirectory(PRESET_FOLDER);
            return;
        }
        
        string[] files = Directory.GetFiles(PRESET_FOLDER, "*.json");
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            availablePresets.Add(fileName);
        }
        
        if (selectedPresetIndex >= availablePresets.Count)
            selectedPresetIndex = 0;
    }
    
    void SavePreset(Material material, string presetName)
    {
        // Implementation for saving presets
        // This would be a comprehensive preset save including all parameters
    }
    
    void LoadPreset(Material material, string presetName)
    {
        // Implementation for loading presets
        // This would restore all material parameters from saved preset
    }
    
    void DeletePreset(string presetName)
    {
        string filePath = Path.Combine(PRESET_FOLDER, presetName + ".json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            AssetDatabase.Refresh();
        }
    }
}