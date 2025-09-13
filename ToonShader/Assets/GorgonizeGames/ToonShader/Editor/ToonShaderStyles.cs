using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Gorgonize Toon Shader GUI için modern stil sistemi
    /// </summary>
    public static class ToonShaderStyles
    {
        // Textures
        public static Texture2D logoTexture;
        
        // Colors
        public static readonly Color primaryColor = new Color(0.2f, 0.6f, 1.0f, 1f);
        public static readonly Color secondaryColor = new Color(0.8f, 0.4f, 0.2f, 1f);
        public static readonly Color accentColor = new Color(0.4f, 0.8f, 0.6f, 1f);
        public static readonly Color backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        public static readonly Color sectionColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
        
        // Styles
        public static GUIStyle headerStyle;
        public static GUIStyle subHeaderStyle;
        public static GUIStyle sectionStyle;
        public static GUIStyle boxStyle;
        public static GUIStyle buttonStyle;
        public static GUIStyle toggleStyle;
        public static GUIStyle labelStyle;
        public static GUIStyle helpBoxStyle;
        public static GUIStyle foldoutStyle;
        
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (isInitialized) return;
            
            LoadTextures();
            InitializeStyles();
            
            isInitialized = true;
        }

        private static void LoadTextures()
        {
            // Logo yükleme
            string[] guids = AssetDatabase.FindAssets("GorgonizeLogo t:Texture2D");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            
            // Eğer logo bulunamazsa, Resources klasöründen dene
            if (logoTexture == null)
            {
                logoTexture = Resources.Load<Texture2D>("GorgonizeLogo");
            }
        }

        private static void InitializeStyles()
        {
            // Header Style - Ana başlık
            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };

            // Sub Header Style - Alt başlık
            subHeaderStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.8f, 0.9f, 0.8f) },
                padding = new RectOffset(5, 5, 2, 8)
            };

            // Section Style - Bölüm kutuları
            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(15, 15, 10, 15),
                margin = new RectOffset(5, 5, 5, 5),
                normal = { background = CreateColorTexture(sectionColor) }
            };

            // Box Style - Özel kutular
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(2, 2, 2, 2),
                normal = { background = CreateColorTexture(sectionColor) }
            };

            // Button Style - Butonlar
            buttonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(8, 8, 4, 4),
                normal = 
                { 
                    textColor = Color.white,
                    background = CreateColorTexture(primaryColor)
                }
            };

            // Toggle Style - Toggle butonları
            toggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) }
            };

            // Label Style - Etiketler
            labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f, 1f) },
                wordWrap = true
            };

            // Help Box Style - Yardım kutuları
            helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 10,
                fontStyle = FontStyle.Italic,
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(5, 5, 5, 5),
                normal = 
                { 
                    textColor = new Color(0.7f, 0.8f, 0.9f, 1f),
                    background = CreateColorTexture(new Color(0.3f, 0.5f, 0.7f, 0.15f))
                }
            };

            // Foldout Style - Açılır bölümler
            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) },
                onNormal = { textColor = primaryColor }
            };
        }

        // Yardımcı metodlar
        private static Texture2D CreateColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        // Özel çizim metodları
        public static void DrawSeparator(float thickness = 1f, Color? color = null)
        {
            var separatorColor = color ?? new Color(0.5f, 0.5f, 0.5f, 0.4f);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(thickness));
            EditorGUI.DrawRect(rect, separatorColor);
        }

        public static void DrawHeader(string title, string subtitle = "")
        {
            EditorGUILayout.Space(10);
            
            var headerRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(headerRect, new Color(0.2f, 0.3f, 0.5f, 0.2f));
            
            GUILayout.Space(15);
            
            // Logo'yu üstte ortala
            if (logoTexture != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logoTexture, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(10);
            }
            
            // Başlık ve alt başlığı ortala
            EditorGUILayout.BeginVertical();
            GUILayout.Label(title, headerStyle);
            
            if (!string.IsNullOrEmpty(subtitle))
            {
                GUILayout.Label(subtitle, subHeaderStyle);
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            EditorGUILayout.EndVertical();
        }

        public static bool DrawFoldoutHeader(string title, bool foldout, string icon = "")
        {
            var displayTitle = string.IsNullOrEmpty(icon) ? title : $"{icon} {title}";
            return EditorGUILayout.Foldout(foldout, displayTitle, foldoutStyle);
        }

        public static void DrawPropertyField(MaterialEditor editor, MaterialProperty property, 
            string label, string tooltip = "")
        {
            if (property == null) return;
            
            var content = new GUIContent(label, tooltip);
            editor.ShaderProperty(property, content);
        }

        public static void DrawInfoBox(string message, MessageType messageType = MessageType.Info)
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.HelpBox(message, messageType);
            EditorGUILayout.Space(3);
        }

        public static void DrawToggleGroup(string title, ref bool toggle, System.Action drawContent)
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            
            var toggleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = toggle ? accentColor : Color.gray }
            };
            
            toggle = EditorGUILayout.ToggleLeft(title, toggle, toggleStyle);
            
            if (toggle)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(5);
                drawContent?.Invoke();
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        // Cleanup
        public static void Cleanup()
        {
            isInitialized = false;
        }
    }
}