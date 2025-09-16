using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Prosedürel olarak farklı tiplerde gürültü dokuları oluşturmak ve yönetmek için bir editör penceresi.
    /// </summary>
    public class NoiseTextureEditor : EditorWindow
    {
        // Gürültü Tipleri
        private enum NoiseType { Perlin, Worley }
        private NoiseType noiseType = NoiseType.Perlin;

        // Sabitler
        private const string NoiseFolderPath = "Assets/GorgonizeGames/ToonShader/Presets/Noise";

        // Genel Ayarlar
        private int textureSize = 256;

        // Perlin Gürültü Ayarları
        private float perlinScale = 20f;
        private int perlinOctaves = 4;
        private float perlinPersistence = 0.5f;
        private float perlinLacunarity = 2.0f;
        private float perlinOffsetX = 0f;
        private float perlinOffsetY = 0f;
        
        // Worley Gürültü Ayarları
        private int worleyCellCount = 10;
        private float worleyFalloff = 1.0f;
        private int worleySeed = 0;
        private bool worleyInvert = false;

        // UI Değişkenleri
        private string newNoiseName = "New_Noise_Texture";
        private List<NoisePreset> noisePresets = new List<NoisePreset>();
        private Vector2 mainScrollPosition;
        private Vector2 presetScrollPosition;
        private Texture2D previewTexture;

        // Stil Değişkenleri
        private static GUIStyle deleteButtonStyle;
        private static GUIStyle previewBoxStyle;
        private static GUIStyle presetLabelStyle;
        private static GUIStyle selectTextStyle;
        
        private class NoisePreset
        {
            public string name;
            public string path;
            public Texture2D texture;
            public bool isRenaming = false;
            public string tempName = "";
        }

        [MenuItem("Gorgonize Game Tools/Noise Texture Editor")]
        public static void ShowWindow()
        {
            GetWindow<NoiseTextureEditor>("Noise Editor").minSize = new Vector2(650, 450);
        }

        private void OnEnable()
        {
            LoadNoisePresets();
            GeneratePreview();
            wantsMouseMove = true; // Hover efektleri için gerekli
        }

        private void OnDisable()
        {
            if (previewTexture != null)
            {
                DestroyImmediate(previewTexture);
            }
        }

        private void OnGUI()
        {
            ToonShaderStyles.Initialize();
            InitializeLocalStyles();

            GUILayout.Label("Noise Texture Editor", ToonShaderStyles.HeaderStyle);
            ToonShaderStyles.DrawAccentSeparator();

            EditorGUILayout.BeginHorizontal();

            // Sol Panel: Kayıtlı Gürültü Dokuları
            DrawPresetManagerSection();

            // Dikey Ayırıcı
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));

            // Sağ Panel: Gürültü Oluşturucu
            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition, GUILayout.ExpandHeight(true));
            DrawGeneratorSection();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGeneratorSection()
        {
            EditorGUILayout.BeginVertical();
            ToonShaderStyles.DrawPropertyGroup("Noise Generator", () =>
            {
                EditorGUI.BeginChangeCheck();
                noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", noiseType);

                // Seçilen gürültü tipine göre ayarları göster
                switch (noiseType)
                {
                    case NoiseType.Perlin:
                        DrawPerlinSettings();
                        break;
                    case NoiseType.Worley:
                        DrawWorleySettings();
                        break;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    GeneratePreview();
                }
            });

            ToonShaderStyles.DrawPropertyGroup("Texture Settings", () =>
            {
                textureSize = EditorGUILayout.IntPopup("Resolution", textureSize, new string[] { "128x128", "256x256", "512x512" }, new int[] { 128, 256, 512 });
                
                if (previewTexture != null)
                {
                    GUILayout.Label("Preview", EditorStyles.centeredGreyMiniLabel);
                    Rect previewRect = GUILayoutUtility.GetRect(256, 256);
                    EditorGUI.DrawPreviewTexture(previewRect, previewTexture, null, ScaleMode.ScaleToFit);
                }

                EditorGUILayout.Space(10);
                newNoiseName = EditorGUILayout.TextField("Texture Name", newNoiseName);
                
                if (GUILayout.Button("SAVE TEXTURE", ToonShaderStyles.ButtonPrimaryStyle, GUILayout.Height(35)))
                {
                    SaveNewNoiseTexture();
                }
            });

            EditorGUILayout.EndVertical();
        }

        private void DrawPerlinSettings()
        {
            perlinScale = EditorGUILayout.Slider("Scale", perlinScale, 1f, 100f);
            perlinOctaves = EditorGUILayout.IntSlider("Octaves", perlinOctaves, 1, 8);
            perlinPersistence = EditorGUILayout.Slider("Persistence", perlinPersistence, 0f, 1f);
            perlinLacunarity = EditorGUILayout.Slider("Lacunarity", perlinLacunarity, 1f, 4f);
            perlinOffsetX = EditorGUILayout.FloatField("Offset X", perlinOffsetX);
            perlinOffsetY = EditorGUILayout.FloatField("Offset Y", perlinOffsetY);

            if (GUILayout.Button("Randomize Offset"))
            {
                perlinOffsetX = Random.Range(0f, 9999f);
                perlinOffsetY = Random.Range(0f, 9999f);
                GeneratePreview();
            }
        }

        private void DrawWorleySettings()
        {
            worleyCellCount = EditorGUILayout.IntSlider("Cell Count", worleyCellCount, 2, 100);
            worleyFalloff = EditorGUILayout.Slider("Falloff / Sharpness", worleyFalloff, 0.1f, 10f);
            worleyInvert = EditorGUILayout.Toggle("Invert Colors", worleyInvert);

            EditorGUILayout.BeginHorizontal();
            worleySeed = EditorGUILayout.IntField("Random Seed", worleySeed);
            if (GUILayout.Button("Randomize"))
            {
                worleySeed = Random.Range(0, 99999);
                GeneratePreview();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawPresetManagerSection()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(250), GUILayout.ExpandHeight(true));
            GUILayout.Label("Saved Noise Textures", ToonShaderStyles.SubHeaderStyle);
            presetScrollPosition = EditorGUILayout.BeginScrollView(presetScrollPosition, "box");

            if (noisePresets.Count == 0)
            {
                EditorGUILayout.LabelField("No saved noise textures.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var preset in noisePresets.ToList())
                {
                    DrawPresetListItem(preset);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawPresetListItem(NoisePreset preset)
        {
            EditorGUILayout.BeginVertical(previewBoxStyle);

            Rect previewRect = new Rect();
            if (preset.texture != null)
            {
                previewRect = GUILayoutUtility.GetRect(0, 80, GUILayout.ExpandWidth(true));
                EditorGUI.DrawPreviewTexture(previewRect, preset.texture, null, ScaleMode.ScaleToFit);
            }
            
            EditorGUILayout.Space(5);

            if (preset.isRenaming)
            {
                preset.tempName = EditorGUILayout.TextField(preset.tempName);
            }
            else
            {
                EditorGUILayout.LabelField(preset.name, presetLabelStyle);
            }
            
            EditorGUILayout.BeginHorizontal();
            
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

            Rect containerRect = GUILayoutUtility.GetLastRect();
            bool isMouseOver = containerRect.Contains(Event.current.mousePosition);

            if (isMouseOver)
            {
                DrawOutline(containerRect, ToonShaderStyles.AccentBlue, 1.5f);
                EditorGUI.DrawRect(previewRect, new Color(0, 0, 0, 0.5f));
                GUI.Label(previewRect, "SELECT", selectTextStyle);

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    AssignNoiseToSelectedMaterial(preset.texture);
                    Event.current.Use();
                }
                
                Repaint();
            }
            
            EditorGUILayout.Space(5);
        }

        private void DrawOutline(Rect rect, Color color, float thickness)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void GeneratePreview()
        {
            if (previewTexture != null)
            {
                DestroyImmediate(previewTexture);
            }
            previewTexture = GenerateNoiseTexture(256, 256);
        }

        private Texture2D GenerateNoiseTexture(int width, int height)
        {
            switch (noiseType)
            {
                case NoiseType.Perlin:
                    return GeneratePerlinNoiseTexture(width, height);
                case NoiseType.Worley:
                    return GenerateWorleyNoiseTexture(width, height);
                default:
                    return new Texture2D(width, height);
            }
        }

        private Texture2D GeneratePerlinNoiseTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseValue = 0;
                    float maxAmplitude = 0;

                    for (int i = 0; i < perlinOctaves; i++)
                    {
                        float sampleX = (x + perlinOffsetX) / perlinScale * frequency;
                        float sampleY = (y + perlinOffsetY) / perlinScale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                        noiseValue += perlinValue * amplitude;
                        
                        maxAmplitude += amplitude;

                        amplitude *= perlinPersistence;
                        frequency *= perlinLacunarity;
                    }

                    float normalizedNoise = noiseValue / maxAmplitude;
                    Color color = new Color(normalizedNoise, normalizedNoise, normalizedNoise);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }

        private Texture2D GenerateWorleyNoiseTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            
            Random.InitState(worleySeed);

            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < worleyCellCount; i++)
            {
                points.Add(new Vector2(Random.Range(0, width), Random.Range(0, height)));
            }
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float minDistance = float.MaxValue;

                    foreach (Vector2 p in points)
                    {
                        minDistance = Mathf.Min(minDistance, Vector2.Distance(new Vector2(x, y), p));
                    }
                    
                    float avgCellRadius = width / Mathf.Sqrt(worleyCellCount);
                    float value = minDistance / (avgCellRadius * 1.5f);
                    value = Mathf.Clamp01(value);
                    
                    value = Mathf.Pow(value, worleyFalloff);

                    if (worleyInvert)
                    {
                        value = 1.0f - value;
                    }

                    Color color = new Color(value, value, value);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }
        
        private void SaveNewNoiseTexture()
        {
            if (string.IsNullOrWhiteSpace(newNoiseName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a valid texture name.", "OK");
                return;
            }

            if (!Directory.Exists(NoiseFolderPath)) Directory.CreateDirectory(NoiseFolderPath);

            string cleanName = string.Join("_", newNoiseName.Split(Path.GetInvalidFileNameChars()));
            string path = Path.Combine(NoiseFolderPath, $"{cleanName}.png");

            if (File.Exists(path) && !EditorUtility.DisplayDialog("Warning", $"A file named '{cleanName}.png' already exists. Overwrite?", "Yes", "No"))
            {
                return;
            }
            
            Texture2D noiseTexture = GenerateNoiseTexture(textureSize, textureSize);
            if (noiseTexture != null)
            {
                byte[] pngData = noiseTexture.EncodeToPNG();
                DestroyImmediate(noiseTexture);
                File.WriteAllBytes(path, pngData);
                
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

                if (textureImporter != null)
                {
                    textureImporter.textureType = TextureImporterType.Default;
                    textureImporter.sRGBTexture = false;
                    textureImporter.wrapMode = TextureWrapMode.Repeat;
                    textureImporter.mipmapEnabled = true;
                    textureImporter.isReadable = true;
                    textureImporter.SaveAndReimport();
                    Debug.Log($"Noise texture saved to '{path}'.");
                }
                
                LoadNoisePresets();
            }
        }

        private void LoadNoisePresets()
        {
            noisePresets.Clear();
            if (!Directory.Exists(NoiseFolderPath))
            {
                Directory.CreateDirectory(NoiseFolderPath);
                AssetDatabase.Refresh();
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:texture2D", new[] { NoiseFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    noisePresets.Add(new NoisePreset { name = Path.GetFileNameWithoutExtension(path), path = path, texture = texture });
                }
            }
        }
        
        private void DeletePreset(NoisePreset preset)
        {
            if (EditorUtility.DisplayDialog("Delete Preset", $"Are you sure you want to permanently delete '{preset.name}'?", "Yes, Delete", "Cancel"))
            {
                AssetDatabase.DeleteAsset(preset.path);
                LoadNoisePresets();
            }
        }

        private void RenamePreset(NoisePreset preset)
        {
            preset.isRenaming = false;
            string newName = preset.tempName;

            if (string.IsNullOrWhiteSpace(newName) || newName == preset.name) return;

            string validationError = AssetDatabase.ValidateMoveAsset(preset.path, Path.Combine(Path.GetDirectoryName(preset.path), $"{newName}.png"));
            if (string.IsNullOrEmpty(validationError))
            {
                AssetDatabase.RenameAsset(preset.path, newName);
                LoadNoisePresets();
            }
            else
            {
                EditorUtility.DisplayDialog("Rename Error", validationError, "OK");
            }
        }

        private void AssignNoiseToSelectedMaterial(Texture2D noiseTexture)
        {
            var selectedObject = Selection.activeObject;
            if (selectedObject == null || !(selectedObject is Material)) return;

            Material selectedMaterial = selectedObject as Material;
            if (selectedMaterial.shader == null || !selectedMaterial.shader.name.Contains("Gorgonize Toon Shader")) return;
            if (!selectedMaterial.HasProperty("_OutlineNoiseMap")) return;

            selectedMaterial.SetTexture("_OutlineNoiseMap", noiseTexture);
            Debug.Log($"Assigned noise '{noiseTexture.name}' to material '{selectedMaterial.name}'.");
        }

        private void InitializeLocalStyles()
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
            if (selectTextStyle == null)
            {
                selectTextStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white },
                    fontSize = 14
                };
            }
        }
    }
}

