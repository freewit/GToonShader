using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Toon Shader için özel ramp dokuları oluşturan ve yöneten, profesyonel tasarıma sahip bir Unity Editor penceresi.
    /// </summary>
    public class RampCreatorEditor : EditorWindow
    {
        // Sabitler
        private const string RampsFolderPath = "Assets/GorgonizeGames/ToonShader/Presets/Ramps";
        private const int TextureHeight = 1;

        // GUI Değişkenleri
        private Gradient currentGradient;
        private int textureWidth = 256;
        private string newRampName = "New_Ramp";
        private List<RampPreset> rampPresets = new List<RampPreset>();
        private Vector2 mainScrollPosition;
        
        // GUI Stilleri
        private static GUIStyle deleteButtonStyle;
        private static GUIStyle previewBoxStyle;
        private static GUIStyle presetLabelStyle;

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
            GetWindow<RampCreatorEditor>("Ramp Creator").minSize = new Vector2(480, 520);
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

            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            // Sadeleştirilmiş Başlık
            GUILayout.Label("Ramp Texture Creator", ToonShaderStyles.HeaderStyle);
            ToonShaderStyles.DrawAccentSeparator();
            
            DrawCreatorSection();
            DrawPresetManagerSection();
            
            EditorGUILayout.Space(20);
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawCreatorSection()
        {
            ToonShaderStyles.DrawPropertyGroup("Yeni Ramp Oluştur", () =>
            {
                EditorGUI.BeginChangeCheck();
                currentGradient = EditorGUILayout.GradientField(new GUIContent("🎨 Geçiş (Gradient)"), currentGradient);
                if (EditorGUI.EndChangeCheck()) Repaint();

                textureWidth = EditorGUILayout.IntSlider("📏 Doku Genişliği", textureWidth, 64, 1024);
                
                EditorGUILayout.Space(5);
                
                if (currentGradient != null)
                {
                    GUILayout.Label("Önizleme", EditorStyles.centeredGreyMiniLabel);
                    Rect previewRect = GUILayoutUtility.GetRect(100, 30, GUILayout.ExpandWidth(true));
                    Texture2D previewTexture = GenerateRampTexture(textureWidth, currentGradient);
                    if (previewTexture != null)
                    {
                        EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
                        DestroyImmediate(previewTexture);
                    }
                }

                EditorGUILayout.Space(10);
                newRampName = EditorGUILayout.TextField("📝 Yeni Ramp Adı", newRampName);
                EditorGUILayout.Space(5);
                if (GUILayout.Button("💾 Yeni Ramp Olarak Kaydet", ToonShaderStyles.ButtonPrimaryStyle, GUILayout.Height(35)))
                {
                    SaveNewRamp();
                }
            });
        }

        private void DrawPresetManagerSection()
        {
            ToonShaderStyles.DrawPropertyGroup("Kayıtlı Ramp'lar", () =>
            {
                if (rampPresets.Count == 0)
                {
                    EditorGUILayout.LabelField("Hiç kayıtlı ramp bulunamadı.", EditorStyles.centeredGreyMiniLabel);
                }
                
                int columns = Mathf.FloorToInt(position.width / 160f);
                int rows = Mathf.CeilToInt((float)rampPresets.Count / columns);

                for (int i = 0; i < rows; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < columns; j++)
                    {
                        int index = i * columns + j;
                        if (index < rampPresets.Count)
                        {
                            DrawPreset(rampPresets[index]);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        private void DrawPreset(RampPreset preset)
        {
            EditorGUILayout.BeginVertical(previewBoxStyle, GUILayout.Width(150));

            // Görsel Önizleme
            if (preset.texture != null)
            {
                Rect previewRect = GUILayoutUtility.GetRect(130, 70);
                EditorGUI.DrawPreviewTexture(previewRect, preset.texture, null, ScaleMode.ScaleAndCrop);
            }
            
            // İsim ve Yeniden Adlandırma
            if (preset.isRenaming)
            {
                preset.tempName = EditorGUILayout.TextField(preset.tempName, GUILayout.Width(130));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("✅", ToonShaderStyles.ButtonSecondaryStyle)) { RenamePreset(preset); }
                if (GUILayout.Button("❌", ToonShaderStyles.ButtonSecondaryStyle)) { preset.isRenaming = false; }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(preset.name, presetLabelStyle, GUILayout.Width(130));
            }
            
            // Butonlar
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📥 Yükle", ToonShaderStyles.ButtonSecondaryStyle)) { LoadRampFromTexture(preset.texture); }
            if (!preset.isRenaming && GUILayout.Button("✏️", ToonShaderStyles.ButtonSecondaryStyle, GUILayout.Width(30))) { preset.tempName = preset.name; preset.isRenaming = true; }
            if (GUILayout.Button("🗑️", deleteButtonStyle, GUILayout.Width(30))) { DeletePreset(preset); }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
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
                EditorUtility.DisplayDialog("Hata", "Lütfen geçerli bir ramp adı girin.", "Tamam");
                return;
            }

            if (!Directory.Exists(RampsFolderPath)) Directory.CreateDirectory(RampsFolderPath);

            string cleanName = string.Join("_", newRampName.Split(Path.GetInvalidFileNameChars()));
            string path = Path.Combine(RampsFolderPath, $"{cleanName}.png");

            if (File.Exists(path) && !EditorUtility.DisplayDialog("Uyarı", $"'{cleanName}.png' adında bir dosya zaten mevcut. Üzerine yazılsın mı?", "Evet", "Hayır"))
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
                    Debug.Log($"Ramp dokusu '{path}' konumuna kaydedildi ve import ayarları otomatik olarak yapıldı.");
                }
                else
                {
                    Debug.LogError($"'{path}' için TextureImporter bulunamadı. Lütfen import ayarlarını manuel olarak yapın.");
                    AssetDatabase.Refresh();
                }
                
                LoadRampPresets();
            }
        }

        private void DeletePreset(RampPreset preset)
        {
            if (EditorUtility.DisplayDialog("Preset'i Sil", $"'{preset.name}' adlı preseti kalıcı olarak silmek istediğinizden emin misiniz?", "Evet, Sil", "İptal"))
            {
                AssetDatabase.DeleteAsset(preset.path);
                Debug.Log($"Preset '{preset.name}' silindi.");
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
                Debug.Log($"Preset '{preset.name}' adı '{newName}' olarak değiştirildi.");
                LoadRampPresets();
            }
            else
            {
                EditorUtility.DisplayDialog("Yeniden Adlandırma Hatası", validationError, "Tamam");
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
            Debug.Log($"'{texture.name}' preseti yüklendi.");
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
        }
    }
}

