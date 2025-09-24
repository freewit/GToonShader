using UnityEngine;
using UnityEditor;

namespace Gorgonize.ToonShader.Editor
{
    public static class ToonShaderStyles
    {
        // Renk Paleti
        public static readonly Color DarkBackground = new Color32(21, 23, 27, 255);
        private static readonly Color MediumBackground = new Color32(38, 41, 48, 255);
        private static readonly Color LightBackground = new Color32(57, 62, 70, 255);
        public static readonly Color AccentBlue = new Color32(0, 173, 239, 255);
        private static readonly Color TextWhite = new Color32(238, 238, 238, 255);

        // Stil Değişkenleri
        public static GUIStyle HeaderStyle { get; private set; }
        public static GUIStyle SubHeaderStyle { get; private set; }
        public static GUIStyle SectionStyle { get; private set; }
        public static GUIStyle FoldoutStyle { get; private set; }
        public static GUIStyle VersionStyle { get; private set; }
        public static GUIStyle BrandStyle { get; private set; }
        public static GUIStyle InfoBoxStyle { get; private set; }
        public static GUIStyle ButtonPrimaryStyle { get; private set; }
        public static GUIStyle ButtonSecondaryStyle { get; private set; }
        public static GUIStyle PropertyLabelStyle { get; private set; }
        public static GUIStyle GroupHeaderStyle { get; private set; }
        public static GUIStyle FooterStyle { get; private set; }
        public static GUIStyle CatalogHeaderStyle { get; private set; }
        public static GUIStyle SearchBoxStyle { get; private set; }
        public static GUIStyle SearchTextStyle { get; private set; }
        public static GUIStyle FeatureBoxStyle { get; private set; }
        public static GUIStyle FeatureLabelStyle { get; private set; }
        public static GUIStyle FeatureToggleStyle { get; private set; }

        private static bool _initialized = false;
        private static Texture2D _toggleOnTexture;
        private static Texture2D _toggleOffTexture;

        public static void Initialize()
        {
            if (_initialized) return;
            
            CreateTextures();
            CreateStyles();
            
            _initialized = true;
        }

        public static Texture2D GetSolidTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }
        
        private static void CreateTextures()
        {
            _toggleOnTexture = CreateToggleTexture(AccentBlue);
            _toggleOffTexture = CreateToggleTexture(LightBackground);
        }

        private static void CreateStyles()
        {
            BrandStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleCenter, normal = { textColor = TextWhite }, padding = new RectOffset(10, 10, 15, 5) };
            HeaderStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter, normal = { textColor = AccentBlue }, padding = new RectOffset(10, 10, 5, 5) };
            VersionStyle = new GUIStyle(EditorStyles.miniLabel) { fontSize = 10, alignment = TextAnchor.MiddleCenter, normal = { textColor = new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.7f) }, padding = new RectOffset(5, 5, 2, 8) };
            SubHeaderStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14, alignment = TextAnchor.MiddleLeft, normal = { textColor = AccentBlue }, padding = new RectOffset(8, 5, 5, 5) };
            FoldoutStyle = new GUIStyle(EditorStyles.foldout) { fontSize = 13, fontStyle = FontStyle.Bold, normal = { textColor = TextWhite }, onNormal = { textColor = AccentBlue }, hover = { textColor = AccentBlue }, padding = new RectOffset(20, 10, 8, 8), margin = new RectOffset(2, 2, 2, 2), border = new RectOffset(1, 1, 1, 1) };
            SectionStyle = new GUIStyle { padding = new RectOffset(15, 15, 10, 12), margin = new RectOffset(5, 5, 2, 8) };
            GroupHeaderStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 11, fontStyle = FontStyle.Bold, normal = { textColor = AccentBlue }, padding = new RectOffset(8, 8, 4, 4), margin = new RectOffset(0, 0, 5, 2) };
            PropertyLabelStyle = new GUIStyle(EditorStyles.label) { normal = { textColor = TextWhite }, fontSize = 11, padding = new RectOffset(0, 0, 2, 2) };
            InfoBoxStyle = new GUIStyle(EditorStyles.helpBox) { normal = { textColor = TextWhite }, fontSize = 10, padding = new RectOffset(10, 10, 8, 8), margin = new RectOffset(5, 5, 5, 5) };
            ButtonPrimaryStyle = new GUIStyle(GUI.skin.button) { normal = { background = GetSolidTexture(AccentBlue), textColor = Color.white }, hover = { background = GetSolidTexture(new Color(AccentBlue.r, AccentBlue.g, AccentBlue.b, 0.8f)), textColor = Color.white }, active = { background = GetSolidTexture(AccentBlue) }, fontSize = 11, fontStyle = FontStyle.Bold, padding = new RectOffset(15, 15, 8, 8), border = new RectOffset(0,0,0,0), margin = new RectOffset(2,2,2,2) };
            ButtonSecondaryStyle = new GUIStyle(GUI.skin.button) { normal = { background = GetSolidTexture(LightBackground), textColor = TextWhite }, hover = { background = GetSolidTexture(MediumBackground), textColor = AccentBlue }, active = { background = GetSolidTexture(LightBackground) }, fontSize = 11, fontStyle = FontStyle.Bold, padding = new RectOffset(15, 15, 8, 8), border = new RectOffset(0,0,0,0), margin = new RectOffset(2,2,2,2) };
            FooterStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.6f) }, fontSize = 9, padding = new RectOffset(5, 5, 5, 5) };
            
            CatalogHeaderStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleLeft, normal = { textColor = TextWhite }, padding = new RectOffset(10, 10, 10, 10) };
            SearchBoxStyle = new GUIStyle(EditorStyles.toolbar) { normal = { background = GetSolidTexture(MediumBackground) }, fixedHeight = 30, padding = new RectOffset(10, 10, 5, 5), margin = new RectOffset(10, 10, 0, 10) };
            SearchTextStyle = new GUIStyle(EditorStyles.textField) { normal = { textColor = TextWhite, background = GetSolidTexture(MediumBackground) }, fontStyle = FontStyle.Bold };
            FeatureBoxStyle = new GUIStyle { normal = { background = GetSolidTexture(MediumBackground) }, padding = new RectOffset(10, 10, 8, 8), margin = new RectOffset(10, 10, 3, 3) };
            FeatureLabelStyle = new GUIStyle(EditorStyles.label) { normal = { textColor = TextWhite }, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            
            FeatureToggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                normal = { background = _toggleOffTexture },
                onNormal = { background = _toggleOnTexture },
                active = { background = _toggleOnTexture },
                onActive = { background = _toggleOnTexture },
                hover = { background = _toggleOffTexture },
                onHover = { background = _toggleOnTexture },
                fixedWidth = 36,
                fixedHeight = 20,
            };
            FeatureToggleStyle.overflow = new RectOffset(-12, 0, -3, 0); // Center the knob
        }
        
        private static Texture2D CreateToggleTexture(Color bgColor)
        {
            int width = 36;
            int height = 20;
            int knobSize = 16;
            
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(0,0,0,0); // Transparent background
            }
            
            // Draw rounded rect for background
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                     if(new Rect(0,0,width,height).Contains(new Vector2(x,y)))
                     {
                         if(Vector2.Distance(new Vector2(x,y), new Vector2(height/2, height/2)) < height/2 ||
                            Vector2.Distance(new Vector2(x,y), new Vector2(width-height/2, height/2)) < height/2 ||
                            new Rect(height/2, 0, width-height, height).Contains(new Vector2(x,y)))
                         {
                              pixels[y * width + x] = bgColor;
                         }
                     }
                }
            }

            // Draw knob
            int knobX = bgColor == AccentBlue ? width - knobSize - 2 : 2;
            int knobY = (height - knobSize) / 2;
            for (int y = 0; y < knobSize; y++)
            {
                for (int x = 0; x < knobSize; x++)
                {
                    if(Vector2.Distance(new Vector2(x,y), new Vector2(knobSize/2, knobSize/2)) < knobSize/2)
                    {
                         pixels[(knobY + y) * width + (knobX + x)] = Color.white;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }

        public static void DrawProfessionalHeader(string brandName, string shaderName, string version) { GUILayout.Label(brandName, BrandStyle); GUILayout.Label(shaderName, HeaderStyle); GUILayout.Label(version, VersionStyle); DrawAccentSeparator(); }
        public static void DrawAccentSeparator() { EditorGUILayout.Space(8); var rect = EditorGUILayout.GetControlRect(false, 3); rect.x += 20; rect.width -= 40; EditorGUI.DrawRect(rect, new Color(AccentBlue.r, AccentBlue.g, AccentBlue.b, 0.5f)); EditorGUILayout.Space(8); }
        public static void DrawPropertyGroup(string groupName, System.Action drawContent, bool showBackground = true) { if (!string.IsNullOrEmpty(groupName)) { EditorGUILayout.Space(8); var headerRect = EditorGUILayout.GetControlRect(false, 22); EditorGUI.DrawRect(headerRect, new Color(AccentBlue.r, AccentBlue.g, AccentBlue.b, 0.1f)); EditorGUI.LabelField(headerRect, groupName.ToUpper(), new GUIStyle(EditorStyles.boldLabel) { fontSize = 11, normal = { textColor = AccentBlue }, padding = new RectOffset(8, 8, 4, 4) }); EditorGUILayout.Space(2); } if (showBackground) EditorGUILayout.BeginVertical(SectionStyle); drawContent?.Invoke(); if (showBackground) EditorGUILayout.EndVertical(); EditorGUILayout.Space(5); }
        public static void DrawFeatureToggle(MaterialProperty property, string label, string description = "", string icon = "") { EditorGUILayout.BeginHorizontal(); bool isEnabled = property.floatValue > 0.5f; bool newValue = EditorGUILayout.Toggle(isEnabled, GUILayout.Width(18)); property.floatValue = newValue ? 1f : 0f; string displayLabel = string.IsNullOrEmpty(icon) ? label : $"{icon} {label}"; var labelStyle = new GUIStyle(EditorStyles.label) { normal = { textColor = newValue ? AccentBlue : new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.7f) }, fontStyle = newValue ? FontStyle.Bold : FontStyle.Normal, fontSize = 12 }; EditorGUILayout.LabelField(displayLabel, labelStyle); EditorGUILayout.EndHorizontal(); if (!string.IsNullOrEmpty(description) && newValue) { EditorGUI.indentLevel++; var descStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(TextWhite.r, TextWhite.g, TextWhite.b, 0.8f) } }; EditorGUILayout.LabelField($"└ {description}", descStyle); EditorGUI.indentLevel--; } }
        public static void DrawInfoBox(string message, MessageType type = MessageType.Info) { EditorGUILayout.LabelField($"{(type == MessageType.Warning ? "⚠️" : "💡")} {message}", InfoBoxStyle); }
        public static void DrawProfessionalFooter() { EditorGUILayout.Space(20); var separatorRect = EditorGUILayout.GetControlRect(false, 1); separatorRect.x += 20; separatorRect.width -= 40; EditorGUI.DrawRect(separatorRect, new Color(AccentBlue.r, AccentBlue.g, AccentBlue.b, 0.5f)); EditorGUILayout.Space(15); using (new EditorGUILayout.HorizontalScope()) { GUILayout.FlexibleSpace(); if (GUILayout.Button("📚 Docs", ButtonSecondaryStyle)) Application.OpenURL("https://gorgonize.com/docs/toon-shader"); GUILayout.Space(10); if (GUILayout.Button("💬 Support", ButtonPrimaryStyle)) Application.OpenURL("https://gorgonize.com/support"); GUILayout.Space(10); if (GUILayout.Button("⭐ Review", ButtonPrimaryStyle)) Application.OpenURL("https://assetstore.unity.com/packages/slug/your-package-id"); GUILayout.FlexibleSpace(); } EditorGUILayout.Space(15); using (new EditorGUILayout.HorizontalScope()) { GUILayout.FlexibleSpace(); GUILayout.Label("Made with ❤️ by Gorgonize Games", FooterStyle); GUILayout.FlexibleSpace(); } EditorGUILayout.Space(10); }
    }
}
