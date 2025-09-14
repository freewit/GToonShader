using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Professional Asset Store Quality GUI Styles for Gorgonize Toon Shader
    /// </summary>
    public static class ToonShaderStyles
    {
        #region Color Palette - Professional Dark Theme
        
        // Ana Arka Plan - Koyu antrasit
        private static readonly Color MainBackground = new Color32(30, 30, 36, 255); // #1E1E24
        
        // İkincil Arka Plan - Soğuk gri panel
        private static readonly Color SecondaryBackground = new Color32(45, 48, 58, 255); // #2D303A
        
        // Ana Vurgu Rengi - Canlı mavi
        private static readonly Color AccentBlue = new Color32(0, 157, 255, 255); // #009DFF
        
        // Metin/İkonlar - Temiz beyaz
        private static readonly Color TextWhite = new Color32(234, 234, 234, 255); // #EAEAEA
        
        // Yardımcı renkler
        private static readonly Color AccentBlueDim = new Color32(0, 157, 255, 180); // Yarı şeffaf mavi
        private static readonly Color AccentBlueGlow = new Color32(0, 157, 255, 60); // Parlama efekti
        private static readonly Color BorderColor = new Color32(60, 60, 70, 255); // Çerçeve rengi
        private static readonly Color HoverColor = new Color32(0, 140, 220, 255); // Hover efekti
        
        #endregion
        
        #region Style Variables
        
        private static GUIStyle _headerStyle;
        private static GUIStyle _subHeaderStyle;
        private static GUIStyle _sectionStyle;
        private static GUIStyle _foldoutStyle;
        private static GUIStyle _versionStyle;
        private static GUIStyle _brandStyle;
        private static GUIStyle _featureToggleStyle;
        private static GUIStyle _infoBoxStyle;
        private static GUIStyle _warningBoxStyle;
        private static GUIStyle _buttonPrimaryStyle;
        private static GUIStyle _buttonSecondaryStyle;
        private static GUIStyle _propertyLabelStyle;
        private static GUIStyle _groupHeaderStyle;
        private static GUIStyle _footerStyle;
        
        private static Texture2D _headerBackgroundTexture;
        private static Texture2D _sectionBackgroundTexture;
        private static Texture2D _glowTexture;
        private static Texture2D _gradientTexture;
        
        private static bool _initialized = false;
        
        #endregion
        
        #region Public Properties
        
        public static GUIStyle HeaderStyle { get { if (!_initialized) Initialize(); return _headerStyle; } }
        public static GUIStyle SubHeaderStyle { get { if (!_initialized) Initialize(); return _subHeaderStyle; } }
        public static GUIStyle SectionStyle { get { if (!_initialized) Initialize(); return _sectionStyle; } }
        public static GUIStyle FoldoutStyle { get { if (!_initialized) Initialize(); return _foldoutStyle; } }
        public static GUIStyle VersionStyle { get { if (!_initialized) Initialize(); return _versionStyle; } }
        public static GUIStyle BrandStyle { get { if (!_initialized) Initialize(); return _brandStyle; } }
        public static GUIStyle FeatureToggleStyle { get { if (!_initialized) Initialize(); return _featureToggleStyle; } }
        public static GUIStyle InfoBoxStyle { get { if (!_initialized) Initialize(); return _infoBoxStyle; } }
        public static GUIStyle WarningBoxStyle { get { if (!_initialized) Initialize(); return _warningBoxStyle; } }
        public static GUIStyle ButtonPrimaryStyle { get { if (!_initialized) Initialize(); return _buttonPrimaryStyle; } }
        public static GUIStyle ButtonSecondaryStyle { get { if (!_initialized) Initialize(); return _buttonSecondaryStyle; } }
        public static GUIStyle PropertyLabelStyle { get { if (!_initialized) Initialize(); return _propertyLabelStyle; } }
        public static GUIStyle GroupHeaderStyle { get { if (!_initialized) Initialize(); return _groupHeaderStyle; } }
        public static GUIStyle FooterStyle { get { if (!_initialized) Initialize(); return _footerStyle; } }
        
        #endregion
        
        #region Initialization
        
        public static void Initialize()
        {
            if (_initialized) return;
            
            CreateTextures();
            CreateStyles();
            
            _initialized = true;
        }
        
        public static void ForceReinitialize()
        {
            _initialized = false;
            
            // Clean up old textures
            if (_headerBackgroundTexture) Object.DestroyImmediate(_headerBackgroundTexture);
            if (_sectionBackgroundTexture) Object.DestroyImmediate(_sectionBackgroundTexture);
            if (_glowTexture) Object.DestroyImmediate(_glowTexture);
            if (_gradientTexture) Object.DestroyImmediate(_gradientTexture);
            
            Initialize();
        }
        
        private static void CreateTextures()
        {
            // Header background with gradient
            _headerBackgroundTexture = CreateGradientTexture(64, MainBackground, SecondaryBackground, true);
            
            // Section background
            _sectionBackgroundTexture = CreateSolidTexture(SecondaryBackground);
            
            // Glow effect texture
            _glowTexture = CreateGlowTexture(AccentBlueGlow);
            
            // Gradient accent texture
            _gradientTexture = CreateGradientTexture(32, AccentBlueDim, AccentBlue, false);
        }
        
        private static void CreateStyles()
        {
            // Brand/Logo Style - Ana başlık
            _brandStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = TextWhite },
                padding = new RectOffset(10, 10, 15, 5)
            };
            
            // Header Style - Shader adı
            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { 
                    textColor = AccentBlue,
                },
                padding = new RectOffset(10, 10, 5, 5) 
            };
            
            // Version Style
            _versionStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.7f) },
                padding = new RectOffset(5, 5, 2, 8)
            };
            
            // Sub Header Style - Section başlıkları
            _subHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = AccentBlue },
                padding = new RectOffset(8, 5, 5, 5)
            };
            
            // Professional Foldout Style
            _foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { 
                    textColor = TextWhite,
                    background = _sectionBackgroundTexture
                },
                onNormal = { 
                    textColor = AccentBlue,
                    background = _sectionBackgroundTexture
                },
                hover = { 
                    textColor = HoverColor,
                    background = _sectionBackgroundTexture
                },
                padding = new RectOffset(20, 10, 8, 8),
                margin = new RectOffset(2, 2, 2, 2),
                border = new RectOffset(1, 1, 1, 1)
            };
            
            // Section Container Style
            _sectionStyle = new GUIStyle()
            {
                normal = { background = _sectionBackgroundTexture },
                padding = new RectOffset(15, 15, 10, 12),
                margin = new RectOffset(5, 5, 2, 8),
                border = new RectOffset(1, 1, 1, 1)
            };
            
            // Group Header Style
            _groupHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { 
                    textColor = AccentBlue,
                    background = CreateSolidTexture(new Color32(0, 157, 255, 25))
                },
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(0, 0, 5, 2)
            };
            
            // Property Label Style
            _propertyLabelStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = TextWhite },
                fontSize = 11,
                padding = new RectOffset(0, 0, 2, 2)
            };
            
            // Feature Toggle Style
            _featureToggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                normal = { textColor = TextWhite },
                onNormal = { textColor = AccentBlue },
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(20, 5, 3, 3)
            };
            
            // Info Box Style
            _infoBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                normal = { 
                    background = CreateSolidTexture(new Color32(0, 157, 255, 20)),
                    textColor = TextWhite
                },
                fontSize = 10,
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(5, 5, 5, 5),
                border = new RectOffset(1, 1, 1, 1)
            };
            
            // Warning Box Style
            _warningBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                normal = { 
                    background = CreateSolidTexture(new Color32(255, 180, 0, 30)),
                    textColor = TextWhite
                },
                fontSize = 10,
                padding = new RectOffset(10, 10, 8, 8),
                margin = new RectOffset(5, 5, 5, 5),
                border = new RectOffset(1, 1, 1, 1)
            };
            
            // Primary Button Style
            _buttonPrimaryStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { 
                    background = CreateSolidTexture(AccentBlue),
                    textColor = Color.white
                },
                hover = { 
                    background = CreateSolidTexture(HoverColor),
                    textColor = Color.white
                },
                active = {
                    background = CreateSolidTexture(AccentBlue),
                    textColor = new Color(1f, 1f, 1f, 0.8f)
                },
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(15, 15, 8, 8),
                border = new RectOffset(0,0,0,0),
                margin = new RectOffset(2,2,2,2)
            };
            
            // Secondary Button Style
            _buttonSecondaryStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { 
                    background = CreateSolidTexture(new Color32(55, 58, 68, 255)),
                    textColor = TextWhite
                },
                hover = { 
                    background = CreateSolidTexture(new Color32(70, 75, 85, 255)),
                    textColor = AccentBlue
                },
                active = {
                    background = CreateSolidTexture(new Color32(55, 58, 68, 255)),
                    textColor = AccentBlue
                },
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(15, 15, 8, 8),
                border = new RectOffset(0,0,0,0),
                margin = new RectOffset(2,2,2,2)
            };
            
            // Footer Style
            _footerStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.6f) },
                fontSize = 9,
                padding = new RectOffset(5, 5, 5, 5)
            };
        }
        
        #endregion
        
        #region Texture Creation Helpers
        
        private static Texture2D CreateSolidTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }
        
        private static Texture2D CreateGradientTexture(int height, Color topColor, Color bottomColor, bool vertical = true)
        {
            var texture = vertical ? new Texture2D(1, height) : new Texture2D(height, 1);
            
            for (int i = 0; i < height; i++)
            {
                float t = (float)i / (height - 1);
                Color color = Color.Lerp(topColor, bottomColor, t);
                
                if (vertical)
                    texture.SetPixel(0, i, color);
                else
                    texture.SetPixel(i, 0, color);
            }
            
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }
        
        private static Texture2D CreateGlowTexture(Color glowColor)
        {
            int size = 16;
            var texture = new Texture2D(size, size);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float normalizedDistance = distance / (size * 0.5f);
                    float alpha = Mathf.Clamp01(1f - normalizedDistance);
                    alpha = Mathf.Pow(alpha, 2f); // Smooth falloff
                    
                    Color color = new Color(glowColor.r, glowColor.g, glowColor.b, glowColor.a * alpha);
                    texture.SetPixel(x, y, color);
                }
            }
            
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }
        
        #endregion
        
        #region Public Drawing Methods
        
        public static void DrawProfessionalHeader(string brandName, string shaderName, string version)
        {
            EditorGUILayout.Space(10);
            
            // Brand name
            GUILayout.Label("🎨 " + brandName, BrandStyle);
            
            // Shader name with accent
            GUILayout.Label(shaderName, HeaderStyle);
            
            // Version
            GUILayout.Label(version + " 🚀", VersionStyle);
            
            DrawAccentSeparator();
        }
        
        public static void DrawAccentSeparator()
        {
            EditorGUILayout.Space(8);
            
            var rect = EditorGUILayout.GetControlRect(false, 3);
            rect.x += 20;
            rect.width -= 40;
            
            // Animated accent line
            float time = (float)EditorApplication.timeSinceStartup;
            float pulse = (Mathf.Sin(time * 2f) + 1f) * 0.5f;
            Color animatedColor = Color.Lerp(AccentBlueDim, AccentBlue, pulse);
            
            EditorGUI.DrawRect(rect, animatedColor);
            
            EditorGUILayout.Space(8);
        }
        
        public static bool DrawProfessionalFoldout(string title, bool isExpanded, string icon = "")
        {
            EditorGUILayout.Space(5);
            
            var rect = EditorGUILayout.GetControlRect(false, 30);
            
            // Background with rounded effect simulation
            var bgRect = new Rect(rect.x - 8, rect.y - 2, rect.width + 16, rect.height + 4);
            EditorGUI.DrawRect(bgRect, SecondaryBackground);
            
            // Left accent border
            var accentRect = new Rect(bgRect.x, bgRect.y, 4, bgRect.height);
            EditorGUI.DrawRect(accentRect, isExpanded ? AccentBlue : AccentBlueDim);
            
            // Hover glow effect
            if (bgRect.Contains(Event.current.mousePosition))
            {
                var glowRect = new Rect(bgRect.x + 4, bgRect.y, bgRect.width - 4, bgRect.height);
                EditorGUI.DrawRect(glowRect, AccentBlueGlow);
            }
            
            // Icon and title
            var contentRect = new Rect(rect.x + 8, rect.y, rect.width - 8, rect.height);
            string displayTitle = string.IsNullOrEmpty(icon) ? title : icon + "  " + title;
            
            // Custom foldout drawing
            var foldoutRect = new Rect(contentRect.x, contentRect.y + 5, 15, 15);
            bool newState = EditorGUI.Foldout(foldoutRect, isExpanded, "", false);
            
            // Title
            var titleRect = new Rect(contentRect.x + 20, contentRect.y, contentRect.width - 20, contentRect.height);
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = isExpanded ? AccentBlue : TextWhite }
            };
            EditorGUI.LabelField(titleRect, displayTitle, titleStyle);
            
            // Click detection for the entire area
            if (Event.current.type == EventType.MouseDown && bgRect.Contains(Event.current.mousePosition))
            {
                newState = !isExpanded;
                Event.current.Use();
            }
            
            EditorGUILayout.Space(3);
            
            return newState;
        }
        
        public static void DrawPropertyGroup(string groupName, System.Action drawContent, bool showBackground = true)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                EditorGUILayout.Space(8);
                
                // Group header
                var headerRect = EditorGUILayout.GetControlRect(false, 22);
                EditorGUI.DrawRect(headerRect, new Color(0f/255f, 157f/255f, 255f/255f, 15f/255f));
                
                var labelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 11,
                    normal = { textColor = AccentBlue },
                    padding = new RectOffset(8, 8, 4, 4)
                };
                
                EditorGUI.LabelField(headerRect, groupName.ToUpper(), labelStyle);
                EditorGUILayout.Space(2);
            }
            
            if (showBackground)
            {
                EditorGUILayout.BeginVertical(SectionStyle);
            }
            
            drawContent?.Invoke();
            
            if (showBackground)
            {
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(5);
        }
        
        public static void DrawFeatureToggle(MaterialProperty property, string label, string description = "", string icon = "")
        {
            EditorGUILayout.BeginHorizontal();
            
            // Toggle
            bool isEnabled = property.floatValue > 0.5f;
            bool newValue = EditorGUILayout.Toggle(isEnabled, GUILayout.Width(18));
            property.floatValue = newValue ? 1f : 0f;
            
            // Icon and label
            string displayLabel = string.IsNullOrEmpty(icon) ? label : $"{icon} {label}";
            
            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = newValue ? AccentBlue : new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.7f) },
                fontStyle = newValue ? FontStyle.Bold : FontStyle.Normal,
                fontSize = 12
            };
            
            EditorGUILayout.LabelField(displayLabel, labelStyle);
            
            EditorGUILayout.EndHorizontal();
            
            // Description
            if (!string.IsNullOrEmpty(description) && newValue)
            {
                EditorGUI.indentLevel++;
                var descStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.8f) }
                };
                EditorGUILayout.LabelField($"└ {description}", descStyle);
                EditorGUI.indentLevel--;
            }
        }
        
        public static void DrawInfoBox(string message, MessageType type = MessageType.Info)
        {
            string icon = type switch
            {
                MessageType.Info => "💡",
                MessageType.Warning => "⚠️",
                MessageType.Error => "❌",
                _ => "ℹ️"
            };
            
            var style = type == MessageType.Warning ? WarningBoxStyle : InfoBoxStyle;
            EditorGUILayout.LabelField($"{icon} {message}", style);
        }
        
        public static void DrawProfessionalFooter()
        {
            EditorGUILayout.Space(20);
            
            // Separator line
            var separatorRect = EditorGUILayout.GetControlRect(false, 1);
            separatorRect.x += 20;
            separatorRect.width -= 40;
            EditorGUI.DrawRect(separatorRect, AccentBlueDim);
            
            EditorGUILayout.Space(15);
            
            // Action buttons
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("📚 Documentation", ButtonSecondaryStyle, GUILayout.Width(140), GUILayout.Height(28)))
                {
                    Application.OpenURL("https://gorgonize.com/docs/toon-shader");
                }
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("💬 Support", ButtonPrimaryStyle, GUILayout.Width(120), GUILayout.Height(28)))
                {
                    Application.OpenURL("https://gorgonize.com/support");
                }
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("⭐ Review", ButtonPrimaryStyle, GUILayout.Width(100), GUILayout.Height(28)))
                {
                    Application.OpenURL("https://assetstore.unity.com/packages/your-package-url");
                }
                
                GUILayout.FlexibleSpace();
            }
            
            EditorGUILayout.Space(15);
            
            // Credits
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Made with ❤️ by Gorgonize Games", FooterStyle);
                GUILayout.FlexibleSpace();
            }
            
            EditorGUILayout.Space(10);
        }
        
        #endregion
    }
}

