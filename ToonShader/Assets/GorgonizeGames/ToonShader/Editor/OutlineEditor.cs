using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Sadece materyalde hangi outline özelliklerinin görüneceğini seçmek için kullanılan editör penceresi.
    /// </summary>
    public class OutlineEditor : EditorWindow
    {
        private static Material currentMaterial;
        private static OutlineEditor windowInstance;

        // Geçici durumları tutan değişkenler
        private bool _outlineNoiseEnabled;
        
        // Kaydırma çubuğu pozisyonu için
        private Vector2 scrollPosition;
        
        // Bu editör penceresini, belirli bir materyalle ilişkilendirerek açar.
        public static void ShowWindow(Material material)
        {
            // Eğer zaten bir pencere açıksa, onu kullan; değilse yeni bir tane oluştur.
            windowInstance = GetWindow<OutlineEditor>(true, "Advanced Outline Editor", true);
            windowInstance.minSize = new Vector2(400, 220);
            windowInstance.maxSize = new Vector2(400, 500); // Yüksekliği artırılabilir yaptık
            currentMaterial = material;
            windowInstance.LoadSettingsFromMaterial();
        }

        // Materyalden mevcut ayarları yükler.
        private void LoadSettingsFromMaterial()
        {
            if (currentMaterial != null)
            {
                _outlineNoiseEnabled = currentMaterial.GetFloat("_OutlineNoiseEnabled") > 0.5f;
            }
        }
        
        private void OnGUI()
        {
            if (currentMaterial == null)
            {
                EditorGUILayout.HelpBox("No material is currently being edited. Please close this window.", MessageType.Warning);
                if (GUILayout.Button("Close")) this.Close();
                return;
            }
            
            // Profesyonel stilleri ve arayüzü başlat
            ToonShaderStyles.Initialize();
            ToonShaderStyles.DrawProfessionalHeader("Outline", "Feature Selector", "");
            
            // Ayarların çizildiği bölüm (kaydırılabilir)
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawSettings();
            EditorGUILayout.EndScrollView();

            // Apply ve Close butonları
            DrawActionButtons();
        }

        private void DrawSettings()
        {
            // Stil kullanmadan, daha güvenilir bir gruplama yapısı
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Available Outline Features", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // Outline Noise özelliğini etkinleştirme/devre dışı bırakma
            _outlineNoiseEnabled = EditorGUILayout.ToggleLeft(" Enable Outline Noise", _outlineNoiseEnabled);
            
            // Açıklama kutusu
            EditorGUILayout.HelpBox("Adds a wobbly, hand-drawn look to the outlines. Its properties will appear in the main inspector when enabled.", MessageType.Info);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }

        private void DrawActionButtons()
        {
            // Butonların arasında boşluk bırakmak için esnek alan
            GUILayout.FlexibleSpace();
            
            // "Apply" butonu, yapılan seçimleri materyale uygular.
            if (GUILayout.Button("Apply", ToonShaderStyles.ButtonPrimaryStyle, GUILayout.Height(35)))
            {
                ApplySettingsToMaterial();
                this.Close(); // Pencereyi kapat
            }
            
            // "Close" butonu, hiçbir değişiklik yapmadan pencereyi kapatır.
            if (GUILayout.Button("Close", ToonShaderStyles.ButtonSecondaryStyle))
            {
                this.Close();
            }
            EditorGUILayout.Space(5);
        }

        // Editördeki seçimleri materyalin özelliklerine ve anahtar kelimelerine uygular.
        private void ApplySettingsToMaterial()
        {
            if (currentMaterial == null) return;
            
            // Değişiklikleri kaydetmek için materyali işaretle
            Undo.RecordObject(currentMaterial, "Apply Outline Settings");

            // Outline Noise
            currentMaterial.SetFloat("_OutlineNoiseEnabled", _outlineNoiseEnabled ? 1.0f : 0.0f);
            SetKeyword(currentMaterial, "_OUTLINE_NOISE_ON", _outlineNoiseEnabled);
            
            // Değişikliklerin kaydedildiğinden emin ol
            EditorUtility.SetDirty(currentMaterial);
        }

        // Shader anahtar kelimesini (keyword) ayarlar.
        private void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        // Pencere kapandığında referansları temizle.
        private void OnDestroy()
        {
            currentMaterial = null;
            windowInstance = null;
        }
    }
}

