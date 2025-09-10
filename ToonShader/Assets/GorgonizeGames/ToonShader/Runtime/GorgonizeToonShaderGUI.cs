using UnityEngine;
using UnityEditor;

public class GorgonizeToonShaderGUI : ShaderGUI
{
    // Property variables
    MaterialProperty baseColor;
    MaterialProperty baseMap;
    MaterialProperty shadowMode;
    
    // Simple Shadow
    MaterialProperty shadowColor;
    MaterialProperty shadowThreshold;
    
    // Banded Shadow
    MaterialProperty bandedShadowColor;
    MaterialProperty bandedShadowThreshold;
    MaterialProperty bandCount;
    MaterialProperty bandSmooth;
    
    // Ramp Shadow
    MaterialProperty rampTexture;
    
    // GUI Styles
    private static GUIStyle headerStyle;
    private static GUIStyle versionStyle;
    private static GUIStyle boxStyle;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Initialize styles
        InitializeStyles();
        
        // Find properties
        FindProperties(properties);
        
        // Header
        DrawHeader();
        
        EditorGUILayout.Space(10);
        
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
        
        // Icon (using built-in Unity icon)
        GUIContent headerContent = new GUIContent("🎨 Gorgonize Toon Shader");
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
        
        // Section header
        EditorGUILayout.LabelField("📋 Base Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);
        
        // Base Color
        materialEditor.ColorProperty(baseColor, "Base Color");
        
        // Base Texture
        materialEditor.TextureProperty(baseMap, "Base Texture");
        
        EditorGUILayout.EndVertical();
    }
    
    void DrawShadowModeSection(MaterialEditor materialEditor)
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        // Section header
        EditorGUILayout.LabelField("🌑 Shadow Settings", EditorStyles.boldLabel);
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
        EditorGUILayout.LabelField("🔸 Simple Shadow", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        
        materialEditor.ColorProperty(shadowColor, "Shadow Color");
        materialEditor.RangeProperty(shadowThreshold, "Shadow Threshold");
        
        EditorGUI.indentLevel--;
    }
    
    void DrawBandedShadowProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.LabelField("🔶 Banded Shadow", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        
        materialEditor.ColorProperty(bandedShadowColor, "Shadow Color");
        materialEditor.RangeProperty(bandedShadowThreshold, "Shadow Threshold");
        materialEditor.RangeProperty(bandCount, "Band Count");
        materialEditor.RangeProperty(bandSmooth, "Band Smoothness");
        
        EditorGUI.indentLevel--;
    }
    
    void DrawRampShadowProperties(MaterialEditor materialEditor)
    {
        EditorGUILayout.LabelField("🌈 Ramp Shadow", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        
        materialEditor.TextureProperty(rampTexture, "Ramp Texture", false);
        
        // Ramp texture info
        if (rampTexture.textureValue == null)
        {
            EditorGUILayout.HelpBox("Assign a gradient texture for shadow ramping.", MessageType.Info);
        }
        
        EditorGUI.indentLevel--;
    }
    
    void DrawFooter()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        
        EditorGUILayout.LabelField("💡 Tips:", EditorStyles.miniBoldLabel);
        
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
        // Clear all keywords
        material.DisableKeyword("_SHADOWMODE_SIMPLE");
        material.DisableKeyword("_SHADOWMODE_BANDED");
        material.DisableKeyword("_SHADOWMODE_RAMP");
        
        // Set the correct keyword
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