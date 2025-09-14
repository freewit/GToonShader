using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// A professionally designed Unity Editor window for creating and managing custom ramp textures for the Toon Shader.
    /// </summary>
    public class RampCreatorEditor : EditorWindow
    {
        // Constants
        private const string RampsFolderPath = "Assets/GorgonizeGames/ToonShader/Presets/Ramps";
        private const int TextureHeight = 1;
        private const string GuideURL = "https://www.gorgonize.com/docs/gtoon-shader/ramp-creator-guide"; // You can add your own guide link here

        // GUI Variables
        private Gradient currentGradient;
        private int textureWidth = 256;
        private string newRampName = "New_Ramp";
        private List<RampPreset> rampPresets = new List<RampPreset>();
        private Vector2 mainScrollPosition;
        private Vector2 presetScrollPosition;
        
        // GUI Styles
        private static GUIStyle deleteButtonStyle;
        private static GUIStyle previewBoxStyle;
        private static GUIStyle presetLabelStyle;
        private static GUIStyle verticalSeparatorStyle;

        private class RampPreset
        {
            public string name;
            public string path;
            public Texture2D texture;
            public bool isRenaming = false;
            public string tempName = "";
        }

        [MenuItem("Gorgonize Game Tools/Ramp Texture Creator")]
        public static void ShowWindow()
        {
            GetWindow<RampCreatorEditor>("Ramp Creator").minSize = new Vector2(600, 400);
        }

        private void OnEnable()
        {
            if (currentGradient == null)
            {
                currentGradient = new Gradient();
                currentGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0.0f), new GradientColorKey(Color.white, 1.0f) };
                currentGradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
            }
            LoadRampPresets();
        }

        private void OnGUI()
        {
            ToonShaderStyles.Initialize();
            InitializeLocalStyles();

            // Simplified Header
            GUILayout.Label("Ramp Texture Creator", ToonShaderStyles.HeaderStyle);
            ToonShaderStyles.DrawAccentSeparator();

            EditorGUILayout.BeginHorizontal();
            
            // Left Panel: Saved Ramps List
            DrawPresetManagerSection();

            // Vertical Separator
            GUILayout.Box("", verticalSeparatorStyle, GUILayout.Width(2), GUILayout.ExpandHeight(true));

            // Right Panel: Ramp Creator
            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition, GUILayout.ExpandHeight(true));
            DrawCreatorSection();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawCreatorSection()
        {
            EditorGUILayout.BeginVertical();
            ToonShaderStyles.DrawPropertyGroup("Create New Ramp", () =>
            {
                EditorGUI.BeginChangeCheck();
                currentGradient = EditorGUILayout.GradientField(new GUIContent("Gradient"), currentGradient);
                if (EditorGUI.EndChangeCheck()) Repaint();

                textureWidth = EditorGUILayout.IntSlider("Texture Width", textureWidth, 64, 1024);
                
                EditorGUILayout.Space(5);
                
                if (currentGradient != null)
                {
                    GUILayout.Label("Preview", EditorStyles.centeredGreyMiniLabel);
                    Rect previewRect = GUILayoutUtility.GetRect(100, 30, GUILayout.ExpandWidth(true));
                    Texture2D previewTexture = GenerateRampTexture(textureWidth, currentGradient);
                    if (previewTexture != null)
                    {
                        EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
                        DestroyImmediate(previewTexture);
                    }
                }

                EditorGUILayout.Space(10);
                newRampName = EditorGUILayout.TextField("Ramp Name", newRampName);
                EditorGUILayout.Space(5);
                if (GUILayout.Button("SAVE", ToonShaderStyles.ButtonPrimaryStyle, GUILayout.Height(35)))
                {
                    SaveNewRamp();
                }
            });
            
            // Help Section
            ToonShaderStyles.DrawPropertyGroup("Help & Tips", () =>
            {
                ToonShaderStyles.DrawInfoBox("You can create custom lighting ramps using the tools above and use them in your materials.");
                if (GUILayout.Button("Guide", ToonShaderStyles.ButtonSecondaryStyle))
                {
                    Application.OpenURL(GuideURL);
                }
            });

            EditorGUILayout.EndVertical();
        }

        private void DrawPresetManagerSection()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(220), GUILayout.ExpandHeight(true));
            
            GUILayout.Label("Saved Ramps", ToonShaderStyles.SubHeaderStyle);
            
            presetScrollPosition = EditorGUILayout.BeginScrollView(presetScrollPosition, "box");
            
            if (rampPresets.Count == 0)
            {
                EditorGUILayout.LabelField("No saved ramps found.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var preset in rampPresets.ToList())
                {
                    DrawPresetListItem(preset);
                }
            }
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawPresetListItem(RampPreset preset)
        {
            EditorGUILayout.BeginVertical(previewBoxStyle);

            // Visual Preview
            if (preset.texture != null)
            {
                Rect previewRect = GUILayoutUtility.GetRect(0, 30, GUILayout.ExpandWidth(true));
                EditorGUI.DrawPreviewTexture(previewRect, preset.texture, null, ScaleMode.StretchToFill);
            }
            
            EditorGUILayout.Space(5);

            // Name and Renaming
            if (preset.isRenaming)
            {
                preset.tempName = EditorGUILayout.TextField(preset.tempName);
            }
            else
            {
                EditorGUILayout.LabelField(preset.name, presetLabelStyle);
            }
            
            // Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit", ToonShaderStyles.ButtonSecondaryStyle)) { LoadRampFromTexture(preset.texture); }
            
            if (preset.isRenaming)
            {
                if (GUILayout.Button("Save", ToonShaderStyles.ButtonSecondaryStyle)) { RenamePreset(preset); }
                if (GUILayout.Button("Cancel", ToonShaderStyles.ButtonSecondaryStyle)) { preset.isRenaming = false; }
            }
            else
            {
                if (GUILayout.Button("Rename", ToonShaderStyles.ButtonSecondaryStyle)) { preset.tempName = preset.name; preset.isRenaming = true; }
            }
            
            if (GUILayout.Button("Delete", deleteButtonStyle)) { DeletePreset(preset); }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        private void LoadRampPresets()
        {
            rampPresets.Clear();
            if (!Directory.Exists(RampsFolderPath))
            {
                Directory.CreateDirectory(RampsFolderPath);
                AssetDatabase.Refresh();
            }

            string[] guids = AssetDatabase.FindAssets("t:texture2D", new[] { RampsFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null && texture.height == TextureHeight)
                {
                    rampPresets.Add(new RampPreset { name = Path.GetFileNameWithoutExtension(path), path = path, texture = texture });
                }
            }
        }
        
        private void SaveNewRamp()
        {
            if (string.IsNullOrWhiteSpace(newRampName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a valid ramp name.", "OK");
                return;
            }

            if (!Directory.Exists(RampsFolderPath)) Directory.CreateDirectory(RampsFolderPath);

            string cleanName = string.Join("_", newRampName.Split(Path.GetInvalidFileNameChars()));
            string path = Path.Combine(RampsFolderPath, $"{cleanName}.png");

            if (File.Exists(path) && !EditorUtility.DisplayDialog("Warning", $"A file named '{cleanName}.png' already exists. Overwrite?", "Yes", "No"))
            {
                return;
            }
            
            Texture2D rampTexture = GenerateRampTexture(textureWidth, currentGradient);
            if (rampTexture != null)
            {
                byte[] pngData = rampTexture.EncodeToPNG();
                DestroyImmediate(rampTexture);
                File.WriteAllBytes(path, pngData);
                
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                if (textureImporter != null)
                {
                    textureImporter.textureType = TextureImporterType.Default;
                    textureImporter.sRGBTexture = false;
                    textureImporter.wrapModeU = TextureWrapMode.Clamp;
                    textureImporter.wrapModeV = TextureWrapMode.Clamp;
                    textureImporter.filterMode = FilterMode.Bilinear;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                    textureImporter.SaveAndReimport();
                    Debug.Log($"Ramp texture saved to '{path}' and import settings were applied automatically.");
                }
                else
                {
                    Debug.LogError($"TextureImporter for '{path}' not found. Please set the import settings manually.");
                    AssetDatabase.Refresh();
                }
                
                LoadRampPresets();
            }
        }

        private void DeletePreset(RampPreset preset)
        {
            if (EditorUtility.DisplayDialog("Delete Preset", $"Are you sure you want to permanently delete the preset '{preset.name}'?", "Yes, Delete", "Cancel"))
            {
                AssetDatabase.DeleteAsset(preset.path);
                Debug.Log($"Preset '{preset.name}' deleted.");
                LoadRampPresets();
            }
        }

        private void RenamePreset(RampPreset preset)
        {
            preset.isRenaming = false;
            string newName = preset.tempName;

            if (string.IsNullOrWhiteSpace(newName) || newName == preset.name) return;

            string newPath = Path.Combine(Path.GetDirectoryName(preset.path), $"{newName}.png");
            string validationError = AssetDatabase.ValidateMoveAsset(preset.path, newPath);

            if (string.IsNullOrEmpty(validationError))
            {
                AssetDatabase.RenameAsset(preset.path, newName);
                Debug.Log($"Preset '{preset.name}' was renamed to '{newName}'.");
                LoadRampPresets();
            }
            else
            {
                EditorUtility.DisplayDialog("Rename Error", validationError, "OK");
            }
        }

        private void LoadRampFromTexture(Texture2D texture)
        {
            if (texture == null) return;
            
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            Color[] pixels = texture.GetPixels();
            var colorKeys = new List<GradientColorKey>();
            var alphaKeys = new List<GradientAlphaKey>();
            
            const int sampleCount = 8;
            for(int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / (sampleCount - 1);
                int pixelIndex = Mathf.FloorToInt(time * (pixels.Length - 1));
                Color color = pixels[pixelIndex];
                
                colorKeys.Add(new GradientColorKey(color, time));
                alphaKeys.Add(new GradientAlphaKey(color.a, time));
            }
            
            currentGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            newRampName = texture.name;
            Repaint();
            Debug.Log($"Preset '{texture.name}' loaded.");
        }

        private Texture2D GenerateRampTexture(int width, Gradient gradient)
        {
            if (gradient == null) return null;

            Texture2D rampTexture = new Texture2D(width, TextureHeight, TextureFormat.RGBA32, false, true);
            rampTexture.filterMode = FilterMode.Bilinear;
            rampTexture.wrapMode = TextureWrapMode.Clamp;
            
            for (int i = 0; i < width; i++)
            {
                Color color = gradient.Evaluate((float)i / (width - 1));
                rampTexture.SetPixel(i, 0, color);
            }
            
            rampTexture.Apply(false);
            return rampTexture;
        }

        private static void InitializeLocalStyles()
        {
            if (deleteButtonStyle == null)
            {
                deleteButtonStyle = new GUIStyle(ToonShaderStyles.ButtonSecondaryStyle)
                {
                    normal = { textColor = new Color(0.9f, 0.4f, 0.4f) },
                    hover = { textColor = new Color(1f, 0.5f, 0.5f) },
                    active = { textColor = new Color(0.9f, 0.4f, 0.4f) }
                };
            }
            if (previewBoxStyle == null)
            {
                previewBoxStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(5, 5, 5, 5)
                };
            }
            if (presetLabelStyle == null)
            {
                presetLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
            }
            if (verticalSeparatorStyle == null)
            {
                verticalSeparatorStyle = new GUIStyle();
                verticalSeparatorStyle.normal.background = EditorGUIUtility.whiteTexture;
                verticalSeparatorStyle.margin = new RectOffset(4, 4, 4, 4);
            }
        }
    }
}

