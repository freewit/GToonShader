using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Gorgonize.ToonShader.Editor
{
    public class FeatureCatalogEditor : EditorWindow
    {
        private static MaterialEditor currentMaterialEditor;
        private static Material currentMaterial;
        private Vector2 scrollPosition;
        private string searchText = "";

        private class Feature
        {
            public string Keyword;
            public string ToggleProperty;
            public string DisplayName;
            public string Icon;
            public string Description;
        }

        private readonly List<Feature> allFeatures = new List<Feature>
        {
            new Feature { Keyword = "GTOON_FEATURE_ADVANCEDLIGHTING", ToggleProperty = "_FeatureAdvancedLightingToggle", DisplayName = "Advanced Lighting", Icon = "🌑", Description = "Multiple lighting modes like Dual Cell, Banded, and Ramp-based." },
            new Feature { Keyword = "GTOON_FEATURE_SPECULARSYSTEM", ToggleProperty = "_FeatureSpecularSystemToggle", DisplayName = "Specular System", Icon = "✨", Description = "Stylized highlights: Stepped, Soft, Anisotropic, Sparkle, Matcap, and Hair." },
            new Feature { Keyword = "GTOON_FEATURE_ADVANCEDRIMLIGHTING", ToggleProperty = "_FeatureAdvancedRimLightingToggle", DisplayName = "Rim Lighting", Icon = "🌟", Description = "Glowing edge effects with Standard, Stepped, Textured, and Fresnel modes." },
            new Feature { Keyword = "GTOON_FEATURE_SMARTOUTLINESYSTEM", ToggleProperty = "_FeatureSmartOutlineSystemToggle", DisplayName = "Outline System", Icon = "📝", Description = "Classic toon outlines with controls for color, width, and distance scaling." },
            new Feature { Keyword = "GTOON_FEATURE_SUBSURFACESCATTERING", ToggleProperty = "_FeatureSubsurfaceScatteringToggle", DisplayName = "Subsurface Scattering", Icon = "🌸", Description = "Simulates light passing through translucent materials like skin or wax." },
            new Feature { Keyword = "GTOON_FEATURE_ADVANCEDSURFACEDETAILS", ToggleProperty = "_FeatureAdvancedSurfaceDetailsToggle", DisplayName = "Surface Details", Icon = "⚙️", Description = "Adds support for Normal Maps, Parallax, Detail Textures, and Emission." },
            new Feature { Keyword = "GTOON_FEATURE_ADVANCEDWINDSYSTEM", ToggleProperty = "_FeatureAdvancedWindSystemToggle", DisplayName = "Wind System", Icon = "🌬️", Description = "Procedural vertex animation to simulate wind on objects like foliage." },
            new Feature { Keyword = "GTOON_FEATURE_RENDERINGANDPERFORMANCE", ToggleProperty = "_FeatureRenderingAndPerformanceToggle", DisplayName = "Performance", Icon = "⚡", Description = "Options for rendering, culling, and performance-related settings." },
            new Feature { Keyword = "GTOON_FEATURE_HELPANDRESOURCES", ToggleProperty = "_FeatureHelpAndResourcesToggle", DisplayName = "Help & Resources", Icon = "📚", Description = "Quick start guides, presets, and links to documentation." }
        };

        public static void ShowWindow(MaterialEditor materialEditor)
        {
            currentMaterialEditor = materialEditor;
            currentMaterial = materialEditor.target as Material;

            var window = GetWindow<FeatureCatalogEditor>("Feature Catalog");
            window.minSize = new Vector2(350, 450);
            window.maxSize = new Vector2(350, 450);
        }
        
        private void OnFocus()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (currentMaterial == null)
            {
                EditorGUILayout.HelpBox("No material selected.", MessageType.Warning);
                return;
            }
            
            ToonShaderStyles.Initialize();
            
            // Arka planı çiz
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), ToonShaderStyles.GetSolidTexture(ToonShaderStyles.DarkBackground), ScaleMode.StretchToFill);

            DrawHeader();
            
            DrawSearchBar();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUIStyle.none);

            var filteredFeatures = allFeatures.Where(f => f.DisplayName.ToLower().Contains(searchText.ToLower())).ToList();

            foreach (var feature in filteredFeatures)
            {
                DrawFeature(feature);
            }

            EditorGUILayout.EndScrollView();
            
            DrawFooter();
        }

        private void DrawHeader()
        {
            GUILayout.Label("FEATURES", ToonShaderStyles.CatalogHeaderStyle);
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal(ToonShaderStyles.SearchBoxStyle);
            GUILayout.Label("🔍", GUILayout.Width(20));
            searchText = EditorGUILayout.TextField(searchText, ToonShaderStyles.SearchTextStyle);
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawFooter()
        {
             if (GUILayout.Button("Close", ToonShaderStyles.ButtonPrimaryStyle, GUILayout.Height(35)))
            {
                this.Close();
            }
        }

        private void DrawFeature(Feature feature)
        {
            MaterialProperty featureProp = MaterialEditor.GetMaterialProperty(new[] { currentMaterial }, feature.ToggleProperty);
            bool isEnabled = featureProp != null && featureProp.floatValue > 0.5f;

            EditorGUILayout.BeginHorizontal(ToonShaderStyles.FeatureBoxStyle);
            
            GUILayout.Label($"{feature.Icon} {feature.DisplayName}", ToonShaderStyles.FeatureLabelStyle, GUILayout.MinWidth(150));
            
            GUILayout.FlexibleSpace();
            
            EditorGUI.BeginChangeCheck();
            bool toggleState = EditorGUILayout.Toggle(isEnabled, ToonShaderStyles.FeatureToggleStyle, GUILayout.Width(40));
            if(EditorGUI.EndChangeCheck())
            {
                SetFeatureEnabled(feature, toggleState);
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);
        }

        private void SetFeatureEnabled(Feature feature, bool enabled)
        {
            MaterialProperty featureProp = MaterialEditor.GetMaterialProperty(new[] { currentMaterial }, feature.ToggleProperty);
            if (featureProp != null)
            {
                featureProp.floatValue = enabled ? 1.0f : 0.0f;

                if (enabled)
                    currentMaterial.EnableKeyword(feature.Keyword);
                else
                    currentMaterial.DisableKeyword(feature.Keyword);

                EditorUtility.SetDirty(currentMaterial);
                if (currentMaterialEditor != null)
                {
                    currentMaterialEditor.Repaint();
                }
            }
        }
    }
}

