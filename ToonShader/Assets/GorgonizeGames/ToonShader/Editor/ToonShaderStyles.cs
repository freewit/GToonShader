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
        
        // Colors - Modern renk paleti
        public static readonly Color darkBackground = new Color(0x22/255f, 0x28/255f, 0x31/255f, 1f);     // #222831
        public static readonly Color mediumBackground = new Color(0x39/255f, 0x3E/255f, 0x46/255f, 1f);   // #393E46  
        public static readonly Color accentColor = new Color(0x00/255f, 0xAD/255f, 0xB5/255f, 1f);        // #00ADB5
        public static readonly Color lightText = new Color(0xEE/255f, 0xEE/255f, 0xEE/255f, 1f);          // #EEEEEE
        
        // Derived colors
        public static readonly Color sectionColor = new Color(0x39/255f, 0x3E/255f, 0x46/255f, 0.8f);
        public static readonly Color hoverColor = new Color(0x00/255f, 0xAD/255f, 0xB5/255f, 0.2f);
        public static readonly Color activeColor = new Color(0x00/255f, 0xAD/255f, 0xB5/255f, 0.6f);
        public static readonly Color borderColor = new Color(0x00/255f, 0xAD/255f, 0xB5/255f, 0.3f);
        
        // Legacy colors
        public static readonly Color primaryColor = accentColor;
        public static readonly Color secondaryColor = mediumBackground;
        public static readonly Color backgroundColor = darkBackground;
        
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

        public static void Initialize()
        {
            LoadTextures();
            InitializeStyles();
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
            
            // Resources klasöründen dene
            if (logoTexture == null)
            {
                logoTexture = Resources.Load<Texture2D>("GorgonizeLogo");
            }
        }

        private static void InitializeStyles()
        {
            // Header Style
            headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = lightText },
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };

            // Sub Header Style
            subHeaderStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(lightText.r, lightText.g, lightText.b, 0.7f) },
                padding = new RectOffset(5, 5, 2, 8)
            };

            // Section Style
            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(15, 15, 12, 15),
                margin = new RectOffset(5, 5, 5, 5),
                normal = { background = CreateGradientTexture(sectionColor, darkBackground) },
                border = new RectOffset(1, 1, 1, 1)
            };

            // Box Style
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(12, 12, 10, 10),
                margin = new RectOffset(2, 2, 2, 2),
                normal = { 
                    background = CreateColorTexture(mediumBackground),
                    textColor = lightText 
                },
                border = new RectOffset(1, 1, 1, 1)
            };

            // Button Style
            buttonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 6, 6),
                normal = { 
                    textColor = lightText,
                    background = CreateColorTexture(mediumBackground)
                },
                hover = { 
                    textColor = darkBackground,
                    background = CreateColorTexture(accentColor)
                },
                active = { 
                    textColor = darkBackground,
                    background = CreateColorTexture(new Color(accentColor.r * 0.8f, accentColor.g * 0.8f, accentColor.b * 0.8f, 1f))
                }
            };

            // Toggle Style
            toggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                normal = { textColor = lightText },
                onNormal = { textColor = accentColor }
            };

            // Label Style
            labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(lightText.r, lightText.g, lightText.b, 0.9f) },
                wordWrap = true
            };

            // Help Box Style
            helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                padding = new RectOffset(12, 12, 10, 10),
                margin = new RectOffset(5, 5, 5, 5),
                normal = { 
                    textColor = lightText,
                    background = CreateColorTexture(new Color(accentColor.r, accentColor.g, accentColor.b, 0.15f))
                },
                border = new RectOffset(1, 1, 1, 1)
            };

            // Foldout Style
            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = lightText },
                onNormal = { textColor = accentColor },
                hover = { textColor = new Color(accentColor.r * 1.2f, accentColor.g * 1.2f, accentColor.b * 1.2f, 1f) },
                onHover = { textColor = new Color(accentColor.r * 1.2f, accentColor.g * 1.2f, accentColor.b * 1.2f, 1f) },
                active = { textColor = accentColor },
                onActive = { textColor = accentColor },
                focused = { textColor = accentColor },
                onFocused = { textColor = accentColor }
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

        private static Texture2D CreateGradientTexture(Color topColor, Color bottomColor)
        {
            int height = 32;
            var texture = new Texture2D(1, height);
            
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / (height - 1);
                Color color = Color.Lerp(bottomColor, topColor, t);
                texture.SetPixel(0, y, color);
            }
            
            texture.Apply();
            return texture;
        }

        // Çizim metodları
        public static void DrawSeparator(float thickness = 2f, Color? color = null)
        {
            var separatorColor = color ?? accentColor;
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(thickness));
            EditorGUI.DrawRect(rect, separatorColor);
        }

        public static void DrawHeader(string title, string subtitle = "")
        {
            EditorGUILayout.Space(8);
            
            // Stilize header container
            var headerRect = EditorGUILayout.BeginVertical();
            
            // Gradient background
            EditorGUI.DrawRect(headerRect, new Color(darkBackground.r, darkBackground.g, darkBackground.b, 0.95f));
            
            // Top accent border
            var topBorderRect = new Rect(headerRect.x, headerRect.y, headerRect.width, 3);
            EditorGUI.DrawRect(topBorderRect, accentColor);
            
            GUILayout.Space(12);
            
            // Logo ve yazı container
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15); // Sol padding
            
            // Logo ile glow effect
            if (logoTexture != null)
            {
                // Logo background glow
                var logoRect = GUILayoutUtility.GetRect(40, 40);
                var glowRect = new Rect(logoRect.x - 4, logoRect.y - 4, logoRect.width + 8, logoRect.height + 8);
                EditorGUI.DrawRect(glowRect, new Color(accentColor.r, accentColor.g, accentColor.b, 0.15f));
                
                // Logo border
                var borderRect = new Rect(logoRect.x - 1, logoRect.y - 1, logoRect.width + 2, logoRect.height + 2);
                EditorGUI.DrawRect(borderRect, new Color(accentColor.r, accentColor.g, accentColor.b, 0.4f));
                
                // Logo
                GUI.DrawTexture(logoRect, logoTexture);
                
                GUILayout.Space(15);
            }
            
            // Yazılar container
            EditorGUILayout.BeginVertical();
            GUILayout.Space(6);
            
            // Ana başlık
            var headerLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = lightText }
            };
            
            // Alt başlık  
            var subHeaderLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Italic,
                normal = { textColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.9f) }
            };
            
            GUILayout.Label(title, headerLabelStyle);
            GUILayout.Space(2);
            GUILayout.Label(subtitle, subHeaderLabelStyle);
            
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace(); // Sağ padding
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(12);
            
            // Bottom gradient line
            var bottomGradientRect = EditorGUILayout.GetControlRect(GUILayout.Height(2));
            
            // Manuel gradient çizimi
            for (int i = 0; i < bottomGradientRect.width; i++)
            {
                float t = (float)i / bottomGradientRect.width;
                Color currentColor;
                
                if (t < 0.5f)
                    currentColor = Color.Lerp(Color.clear, accentColor, t * 2f);
                else
                    currentColor = Color.Lerp(accentColor, Color.clear, (t - 0.5f) * 2f);
                
                var pixelRect = new Rect(bottomGradientRect.x + i, bottomGradientRect.y, 1, 2);
                EditorGUI.DrawRect(pixelRect, currentColor);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8);
        }

        public static void DrawMinimalHeader(string title, string subtitle = "")
        {
            EditorGUILayout.Space(5);
            
            var separatorRect = EditorGUILayout.GetControlRect(GUILayout.Height(2));
            EditorGUI.DrawRect(separatorRect, accentColor);
            
            EditorGUILayout.Space(8);
            
            var minimalHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = accentColor }
            };
            
            var minimalSubStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 9,
                normal = { textColor = new Color(lightText.r, lightText.g, lightText.b, 0.6f) }
            };
            
            GUILayout.Label(title, minimalHeaderStyle);
            if (!string.IsNullOrEmpty(subtitle))
            {
                GUILayout.Label(subtitle, minimalSubStyle);
            }
            
            EditorGUILayout.Space(8);
            
            var separatorRect2 = EditorGUILayout.GetControlRect(GUILayout.Height(1));
            EditorGUI.DrawRect(separatorRect2, new Color(accentColor.r, accentColor.g, accentColor.b, 0.3f));
        }

        public static void DrawNoLogoHeader(string title, string subtitle = "")
        {
            EditorGUILayout.Space(8);
            
            var headerRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(headerRect, new Color(darkBackground.r, darkBackground.g, darkBackground.b, 0.7f));
            
            var borderRect = new Rect(headerRect.x, headerRect.y, headerRect.width, 3);
            EditorGUI.DrawRect(borderRect, accentColor);
            
            GUILayout.Space(15);
            
            var bigHeaderStyle = new GUIStyle(headerStyle)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            
            var bigSubHeaderStyle = new GUIStyle(subHeaderStyle)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter
            };
            
            GUILayout.Label(title, bigHeaderStyle);
            
            if (!string.IsNullOrEmpty(subtitle))
            {
                GUILayout.Space(3);
                GUILayout.Label(subtitle, bigSubHeaderStyle);
            }
            
            GUILayout.Space(15);
            EditorGUILayout.EndVertical();
        }

        public static bool DrawFoldoutHeader(string title, bool foldout, string icon = "")
        {
            var displayTitle = string.IsNullOrEmpty(icon) ? title : $"{icon} {title}";
            
            EditorGUILayout.BeginHorizontal();
            
            var rect = EditorGUILayout.GetControlRect();
            var backgroundRect = new Rect(rect.x - 5, rect.y - 2, rect.width + 10, rect.height + 4);
            
            if (Event.current.type == EventType.Repaint)
            {
                if (backgroundRect.Contains(Event.current.mousePosition))
                {
                    EditorGUI.DrawRect(backgroundRect, hoverColor);
                }
                else
                {
                    EditorGUI.DrawRect(backgroundRect, new Color(mediumBackground.r, mediumBackground.g, mediumBackground.b, 0.3f));
                }
                
                if (foldout)
                {
                    var accentRect = new Rect(backgroundRect.x, backgroundRect.y, 3, backgroundRect.height);
                    EditorGUI.DrawRect(accentRect, accentColor);
                }
            }
            
            var result = EditorGUI.Foldout(rect, foldout, displayTitle, foldoutStyle);
            
            EditorGUILayout.EndHorizontal();
            
            return result;
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
            EditorGUILayout.Space(5);
            
            Color bgColor;
            Color borderColor;
            Color textColor = lightText;
            string icon;
            
            switch (messageType)
            {
                case MessageType.Warning:
                    bgColor = new Color(1f, 0.8f, 0.2f, 0.15f);
                    borderColor = new Color(1f, 0.8f, 0.2f, 0.6f);
                    icon = "⚠️";
                    break;
                case MessageType.Error:
                    bgColor = new Color(1f, 0.3f, 0.3f, 0.15f);
                    borderColor = new Color(1f, 0.3f, 0.3f, 0.6f);
                    icon = "❌";
                    break;
                case MessageType.None:
                    bgColor = new Color(mediumBackground.r, mediumBackground.g, mediumBackground.b, 0.3f);
                    borderColor = new Color(lightText.r, lightText.g, lightText.b, 0.3f);
                    icon = "💡";
                    break;
                default:
                    bgColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.15f);
                    borderColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.6f);
                    icon = "ℹ️";
                    break;
            }
            
            var rect = EditorGUILayout.BeginVertical();
            
            EditorGUI.DrawRect(rect, bgColor);
            
            var topBorder = new Rect(rect.x, rect.y, rect.width, 2);
            EditorGUI.DrawRect(topBorder, borderColor);
            
            GUILayout.Space(10);
            
            var style = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                wordWrap = true,
                normal = { textColor = textColor },
                padding = new RectOffset(15, 15, 0, 0)
            };
            
            EditorGUILayout.LabelField($"{icon} {message}", style);
            
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
        }

        public static void DrawToggleGroup(string title, ref bool toggle, System.Action drawContent)
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            
            var toggleStyle_custom = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = toggle ? accentColor : Color.gray }
            };
            
            toggle = EditorGUILayout.ToggleLeft(title, toggle, toggleStyle_custom);
            
            if (toggle)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(5);
                drawContent?.Invoke();
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}