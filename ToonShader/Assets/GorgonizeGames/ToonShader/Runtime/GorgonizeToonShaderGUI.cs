using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class ToonShaderPreset
{
    public string presetName;
    public Color baseColor;
    public string baseMapPath;
    public int shadowMode;
    public Color shadowColor;
    public float shadowThreshold;
    public Color bandedShadowColor;
    public float bandedShadowThreshold;
    public float bandCount;
    public float bandSmooth;
    public string rampTexturePath;
}

public class GorgonizeToonShaderGUI : ShaderGUI
{
    // Property variables
    private MaterialProperty baseColor;
    private MaterialProperty baseMap;
    private MaterialProperty shadowMode;
    
    // Simple Shadow
    private MaterialProperty shadowColor;
    private MaterialProperty shadowThreshold;
    
    // Banded Shadow
    private MaterialProperty bandedShadowColor;
    private MaterialProperty bandedShadowThreshold;
    private MaterialProperty bandCount;
    private MaterialProperty bandSmooth;
    
    // Ramp Shadow
    private MaterialProperty rampTexture;
    
    // GUI Styles - static to persist between calls
    private static GUIStyle headerStyle;
    private static GUIStyle versionStyle;
    private static GUIStyle boxStyle;
    
    // Preset system - static to persist between calls
    private static string newPresetName = "";
    private static List<string> availablePresets = new List<string>();
    private static int selectedPresetIndex = 0;
    private const string PRESET_FOLDER = "Assets/GorgonizeGames/ToonShader/Presets/Custom/Material";
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Initialize styles
        InitializeStyles();
        
        // Find properties
        FindProperties(properties);
        
        // Header
        DrawHeader();
        
        EditorGUILayout.Space(10);
        
        // Preset System - EN ÜSTE TAŞINDI
        DrawPresetSystem(materialEditor);
        
        EditorGUILayout.Space(5);
        
        // Base Properties
        DrawBaseProperties(materialEditor);
        
        EditorGUILayout.Space(5);
        
        // Shadow Mode Section
        DrawShadowModeSection(materialEditor);
        
        EditorGUILayout.Space(10);
        
        // Footer info
        DrawFooter();
    }
    
    void FindProperties(MaterialProperty[] props)
    {
        baseColor = FindProperty("_BaseColor", props);
        baseMap = FindProperty("_BaseMap", props);
        shadowMode = FindProperty("_ShadowMode", props);
        
        shadowColor = FindProperty("_ShadowColor", props);
        shadowThreshold = FindProperty("_ShadowThreshold", props);
        
        bandedShadowColor = FindProperty("_BandedShadowColor", props);
        bandedShadowThreshold = FindProperty("_BandedShadowThreshold", props);
        bandCount = FindProperty("_BandCount", props);
        bandSmooth = FindProperty("_BandSmooth", props);
        
        rampTexture = FindProperty("_RampTexture", props);
    }
    
    void InitializeStyles()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.8f, 0.8f, 1f, 1f) }
            };
        }
        
        if (versionStyle == null)
        {
            versionStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f, 1f) },
                fontStyle = FontStyle.Italic
            };
        }
        
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };
        }
    }
    
    void DrawHeader()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        // Shader name with icon
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUIContent headerContent = new GUIContent("Gorgonize Toon Shader");
        EditorGUILayout.LabelField(headerContent, headerStyle);
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        // Version
        EditorGUILayout.LabelField("V0.1 Alpha", versionStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawBaseProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        EditorGUILayout.LabelField("Base Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);
        
        materialEditor.ColorProperty(baseColor, "Base Color");
        materialEditor.TextureProperty(baseMap, "Base Texture");
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawShadowModeSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        EditorGUILayout.LabelField("Shadow Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);
        
        // Shadow Mode dropdown
        EditorGUI.BeginChangeCheck();
        float shadowModeValue = shadowMode.floatValue;
        
        string[] options = { "Simple", "Banded", "Ramp" };
        int selectedMode = Mathf.RoundToInt(shadowModeValue);
        selectedMode = EditorGUILayout.Popup("Shadow Mode", selectedMode, options);
        
        if (EditorGUI.EndChangeCheck())
        {
            shadowMode.floatValue = selectedMode;
            SetKeyword(materialEditor.target as Material, selectedMode);
        }
        
        EditorGUILayout.Space(5);
        
        // Mode-specific properties
        switch (selectedMode)
        {
            case 0: // Simple
                DrawSimpleShadowProperties(materialEditor);
                break;
            case 1: // Banded
                DrawBandedShadowProperties(materialEditor);
                break;
            case 2: // Ramp
                DrawRampShadowProperties(materialEditor);
                break;
        }
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawSimpleShadowProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.LabelField("Simple Shadow", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        
        materialEditor.ColorProperty(shadowColor, "Shadow Color");
        materialEditor.RangeProperty(shadowThreshold, "Shadow Threshold");
        
        EditorGUI.indentLevel--;
    }
    
    void DrawBandedShadowProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.LabelField("Banded Shadow", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        
        materialEditor.ColorProperty(bandedShadowColor, "Shadow Color");
        materialEditor.RangeProperty(bandedShadowThreshold, "Shadow Threshold");
        materialEditor.RangeProperty(bandCount, "Band Count");
        materialEditor.RangeProperty(bandSmooth, "Band Smoothness");
        
        EditorGUI.indentLevel--;
    }
    
    void DrawRampShadowProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.LabelField("Ramp Shadow", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        
        materialEditor.TextureProperty(rampTexture, "Ramp Texture", false);
        
        if (rampTexture.textureValue == null)
        {
            EditorGUILayout.HelpBox("Assign a gradient texture for shadow ramping.", MessageType.Info);
        }
        
        EditorGUI.indentLevel--;
    }
    
    void DrawPresetSystem(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        EditorGUILayout.LabelField("Preset Management", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);
        
        // Save Preset Section
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Preset:", GUILayout.Width(80));
        newPresetName = EditorGUILayout.TextField(newPresetName);
        
        GUI.enabled = !string.IsNullOrEmpty(newPresetName);
        if (GUILayout.Button("Save", GUILayout.Width(60)))
        {
            SavePreset(materialEditor.target as Material, newPresetName);
            newPresetName = "";
            RefreshPresetList();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(3);
        
        // Load Preset Section
        RefreshPresetList();
        
        if (availablePresets.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Load Preset:", GUILayout.Width(80));
            
            selectedPresetIndex = EditorGUILayout.Popup(selectedPresetIndex, availablePresets.ToArray());
            
            if (GUILayout.Button("Load", GUILayout.Width(60)))
            {
                LoadPreset(materialEditor.target as Material, availablePresets[selectedPresetIndex]);
            }
            
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Delete Preset", 
                    "Are you sure you want to delete '" + availablePresets[selectedPresetIndex] + "'?", 
                    "Delete", "Cancel"))
                {
                    DeletePreset(availablePresets[selectedPresetIndex]);
                    RefreshPresetList();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("No presets found. Create your first preset above!", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
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
        {
            selectedPresetIndex = 0;
        }
    }
    
    void SavePreset(Material material, string presetName)
    {
        if (!Directory.Exists(PRESET_FOLDER))
        {
            Directory.CreateDirectory(PRESET_FOLDER);
        }
        
        ToonShaderPreset preset = new ToonShaderPreset
        {
            presetName = presetName,
            baseColor = material.GetColor("_BaseColor"),
            baseMapPath = material.GetTexture("_BaseMap") ? AssetDatabase.GetAssetPath(material.GetTexture("_BaseMap")) : "",
            shadowMode = (int)material.GetFloat("_ShadowMode"),
            shadowColor = material.GetColor("_ShadowColor"),
            shadowThreshold = material.GetFloat("_ShadowThreshold"),
            bandedShadowColor = material.GetColor("_BandedShadowColor"),
            bandedShadowThreshold = material.GetFloat("_BandedShadowThreshold"),
            bandCount = material.GetFloat("_BandCount"),
            bandSmooth = material.GetFloat("_BandSmooth"),
            rampTexturePath = material.GetTexture("_RampTexture") ? AssetDatabase.GetAssetPath(material.GetTexture("_RampTexture")) : ""
        };
        
        string json = JsonUtility.ToJson(preset, true);
        string filePath = Path.Combine(PRESET_FOLDER, presetName + ".json");
        File.WriteAllText(filePath, json);
        
        AssetDatabase.Refresh();
        Debug.Log("Preset '" + presetName + "' saved successfully!");
    }
    
    void LoadPreset(Material material, string presetName)
    {
        string filePath = Path.Combine(PRESET_FOLDER, presetName + ".json");
        
        if (!File.Exists(filePath))
        {
            Debug.LogError("Preset file not found: " + filePath);
            return;
        }
        
        try
        {
            string json = File.ReadAllText(filePath);
            ToonShaderPreset preset = JsonUtility.FromJson<ToonShaderPreset>(json);
            
            material.SetColor("_BaseColor", preset.baseColor);
            
            if (!string.IsNullOrEmpty(preset.baseMapPath))
            {
                Texture2D baseMapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(preset.baseMapPath);
                if (baseMapTexture != null) material.SetTexture("_BaseMap", baseMapTexture);
            }
            
            if (!string.IsNullOrEmpty(preset.rampTexturePath))
            {
                Texture2D rampTextureAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(preset.rampTexturePath);
                if (rampTextureAsset != null) material.SetTexture("_RampTexture", rampTextureAsset);
            }
            
            material.SetFloat("_ShadowMode", preset.shadowMode);
            material.SetColor("_ShadowColor", preset.shadowColor);
            material.SetFloat("_ShadowThreshold", preset.shadowThreshold);
            material.SetColor("_BandedShadowColor", preset.bandedShadowColor);
            material.SetFloat("_BandedShadowThreshold", preset.bandedShadowThreshold);
            material.SetFloat("_BandCount", preset.bandCount);
            material.SetFloat("_BandSmooth", preset.bandSmooth);
            
            SetKeyword(material, preset.shadowMode);
            
            Debug.Log("Preset '" + presetName + "' loaded successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load preset: " + e.Message);
        }
    }
    
    void DeletePreset(string presetName)
    {
        string filePath = Path.Combine(PRESET_FOLDER, presetName + ".json");
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            AssetDatabase.Refresh();
            Debug.Log("Preset '" + presetName + "' deleted successfully!");
        }
    }
    
    void DrawFooter()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        EditorGUILayout.LabelField("Tips:", EditorStyles.miniBoldLabel);
        
        GUIStyle tipStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            wordWrap = true,
            normal = { textColor = new Color(0.6f, 0.6f, 0.6f, 1f) }
        };
        
        EditorGUILayout.LabelField("• Simple: Basic cell shading with hard edges", tipStyle);
        EditorGUILayout.LabelField("• Banded: Multiple shadow steps with smoothing", tipStyle);
        EditorGUILayout.LabelField("• Ramp: Use gradient textures for artistic control", tipStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    void SetKeyword(Material material, int mode)
    {
        material.DisableKeyword("_SHADOWMODE_SIMPLE");
        material.DisableKeyword("_SHADOWMODE_BANDED");
        material.DisableKeyword("_SHADOWMODE_RAMP");
        
        switch (mode)
        {
            case 0:
                material.EnableKeyword("_SHADOWMODE_SIMPLE");
                break;
            case 1:
                material.EnableKeyword("_SHADOWMODE_BANDED");
                break;
            case 2:
                material.EnableKeyword("_SHADOWMODE_RAMP");
                break;
        }
    }
}