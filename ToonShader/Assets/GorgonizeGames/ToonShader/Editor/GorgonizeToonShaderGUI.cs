using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Gorgonize Toon Shader için profesyonel material editor GUI
    /// </summary>
    public class GorgonizeToonShaderGUI : ShaderGUI
    {
        private ToonShaderProperties props;
        
        // Foldout durumları - Hepsi kapalı başlasın
        private static bool showBase = false;
        private static bool showShadows = false;
        private static bool showHighlights = false;
        private static bool showRim = false;
        private static bool showAdvanced = false;
        private static bool showSubsurface = false;
        private static bool showOutline = false;
        private static bool showWind = false;
        private static bool showPerformance = false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Her seferinde force initialize (cache problemi için)
            ToonShaderStyles.Initialize();
            
            props = new ToonShaderProperties(properties);
            
            EditorGUI.BeginChangeCheck();
            
            // Ana başlık
            DrawHeader();
            
            // Ana bölümler
            DrawBaseSectionWithStyle(materialEditor);
            DrawShadowSectionWithStyle(materialEditor);
            DrawHighlightsSectionWithStyle(materialEditor);
            DrawRimSectionWithStyle(materialEditor);
            DrawAdvancedSectionWithStyle(materialEditor);
            DrawSubsurfaceSectionWithStyle(materialEditor);
            DrawOutlineSectionWithStyle(materialEditor);
            DrawWindSectionWithStyle(materialEditor);
            DrawPerformanceSectionWithStyle(materialEditor);
            
            // Footer
            DrawFooter();
            
            if (EditorGUI.EndChangeCheck())
            {
                // Değişiklikleri uygula
                foreach (var obj in materialEditor.targets)
                    EditorUtility.SetDirty(obj);
            }
        }

        private void DrawHeader()
        {
            // SEÇENEK 1: Kompakt Header (Logo + Yazı yan yana, küçük) - AKTİF
            ToonShaderStyles.DrawHeader("Gorgonize Toon Shader", "BETA (v0.7)");
            
            ToonShaderStyles.DrawSeparator();
            EditorGUILayout.Space(8);
        }

        private void DrawBaseSectionWithStyle(MaterialEditor materialEditor)
        {
            showBase = DrawFoldoutSection("🎨 Base Properties", showBase, () =>
            {
                EditorGUILayout.Space(5);
                
                // Base Color
                ToonShaderStyles.DrawPropertyField(materialEditor, props.baseColor, "Base Color", 
                    "The main color of the material");
                
                // Base Texture
                ToonShaderStyles.DrawPropertyField(materialEditor, props.baseMap, "Base Texture", 
                    "The main albedo texture");
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawShadowSectionWithStyle(MaterialEditor materialEditor)
        {
            showShadows = DrawFoldoutSection("🌙 Shadow Properties", showShadows, () =>
            {
                EditorGUILayout.Space(5);
                
                // Lighting Mode dropdown
                if (props.lightingMode != null)
                {
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.lightingMode, "Lighting Mode", 
                        "Choose the lighting calculation method");
                }
                
                // Tint Shadow On Base toggle
                if (props.tintShadowOnBase != null)
                {
                    DrawToggleProperty(materialEditor, props.tintShadowOnBase, "Tint Shadow On Base", 
                        "Apply shadow color tinting to base color");
                }
                
                // Shadow properties
                ToonShaderStyles.DrawPropertyField(materialEditor, props.shadowSteps, "Shadow Steps", 
                    "Number of shadow gradient steps for toon effect");
                
                ToonShaderStyles.DrawPropertyField(materialEditor, props.shadowSmoothness, "Shadow Smoothness", 
                    "Smoothness of shadow transitions");
                
                if (props.shadowRamp != null)
                {
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.shadowRamp, "Shadow Ramp", 
                        "Custom shadow gradient texture");
                }
                
                ToonShaderStyles.DrawPropertyField(materialEditor, props.shadowColor, "Shadow Color", 
                    "Color tint for shadowed areas");
                
                ToonShaderStyles.DrawPropertyField(materialEditor, props.shadowIntensity, "Shadow Intensity", 
                    "Overall shadow intensity multiplier");
                
                ToonShaderStyles.DrawPropertyField(materialEditor, props.shadowOffset, "Shadow Offset", 
                    "Offset to prevent shadow acne and z-fighting");
                
                if (props.occlusionStrength != null)
                {
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.occlusionStrength, "Occlusion Strength", 
                        "Ambient occlusion influence on shadows");
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawHighlightsSectionWithStyle(MaterialEditor materialEditor)
        {
            if (props.enableHighlights == null) return;
            
            showHighlights = DrawFoldoutSection("✨ Highlights", showHighlights, () =>
            {
                EditorGUILayout.Space(5);
                
                bool highlightsEnabled = DrawToggleProperty(materialEditor, props.enableHighlights, 
                    "Enable Highlights", "Enable specular highlights");
                
                if (highlightsEnabled)
                {
                    EditorGUI.indentLevel++;
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.specularColor, "Specular Color", 
                        "Color of the specular highlights");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.specularSize, "Specular Size", 
                        "Size of the specular highlights");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.specularSmoothness, "Specular Smoothness", 
                        "Smoothness of specular transition");
                    
                    if (props.specularSteps != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.specularSteps, "Specular Steps", 
                            "Number of steps in specular gradient");
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawRimSectionWithStyle(MaterialEditor materialEditor)
        {
            if (props.enableRim == null) return;
            
            showRim = DrawFoldoutSection("🌟 Rim Lighting", showRim, () =>
            {
                EditorGUILayout.Space(5);
                
                bool rimEnabled = DrawToggleProperty(materialEditor, props.enableRim, 
                    "Enable Rim Lighting", "Enable rim lighting effect");
                
                if (rimEnabled)
                {
                    EditorGUI.indentLevel++;
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.rimColor, "Rim Color", 
                        "Color of the rim lighting");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.rimPower, "Rim Power", 
                        "Falloff power of rim lighting");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.rimIntensity, "Rim Intensity", 
                        "Intensity of rim lighting");
                    
                    if (props.rimOffset != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.rimOffset, "Rim Offset", 
                            "Offset for rim lighting calculation");
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawAdvancedSectionWithStyle(MaterialEditor materialEditor)
        {
            showAdvanced = DrawFoldoutSection("⚙️ Advanced Features", showAdvanced, () =>
            {
                EditorGUILayout.Space(5);
                
                // Normal mapping
                if (props.normalMap != null)
                {
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.normalMap, "Normal Map", 
                        "Normal map for surface detail");
                    
                    if (props.normalStrength != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.normalStrength, "Normal Strength", 
                            "Strength of the normal map effect");
                    }
                }
                
                // Emission
                if (props.emissionMap != null || props.emissionColor != null)
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField("Emission", EditorStyles.miniBoldLabel);
                    
                    if (props.emissionColor != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.emissionColor, "Emission Color", 
                            "Emissive color");
                    }
                    
                    if (props.emissionMap != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.emissionMap, "Emission Map", 
                            "Emissive texture");
                    }
                    
                    if (props.emissionIntensity != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.emissionIntensity, "Emission Intensity", 
                            "Intensity of emission");
                    }
                }
                
                // Detail textures
                if (props.detailMap != null || props.detailNormalMap != null)
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField("Detail Textures", EditorStyles.miniBoldLabel);
                    
                    if (props.detailMap != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.detailMap, "Detail Map", 
                            "Detail albedo texture");
                    }
                    
                    if (props.detailNormalMap != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.detailNormalMap, "Detail Normal", 
                            "Detail normal map");
                    }
                    
                    if (props.detailStrength != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.detailStrength, "Detail Strength", 
                            "Strength of detail textures");
                    }
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawSubsurfaceSectionWithStyle(MaterialEditor materialEditor)
        {
            if (props.enableSubsurface == null) return;
            
            showSubsurface = DrawFoldoutSection("🔆 Subsurface Scattering", showSubsurface, () =>
            {
                EditorGUILayout.Space(5);
                
                bool subsurfaceEnabled = DrawToggleProperty(materialEditor, props.enableSubsurface, 
                    "Enable Subsurface", "Enable subsurface scattering");
                
                if (subsurfaceEnabled)
                {
                    EditorGUI.indentLevel++;
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.subsurfaceColor, "Subsurface Color", 
                        "Color of subsurface scattering");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.subsurfaceIntensity, "Intensity", 
                        "Intensity of subsurface effect");
                    
                    if (props.subsurfaceDistortion != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.subsurfaceDistortion, "Distortion", 
                            "Light distortion in subsurface");
                    }
                    
                    if (props.subsurfacePower != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.subsurfacePower, "Power", 
                            "Falloff power of subsurface effect");
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawOutlineSectionWithStyle(MaterialEditor materialEditor)
        {
            if (props.enableOutline == null) return;
            
            showOutline = DrawFoldoutSection("📝 Outline", showOutline, () =>
            {
                EditorGUILayout.Space(5);
                
                bool outlineEnabled = DrawToggleProperty(materialEditor, props.enableOutline, 
                    "Enable Outline", "Enable toon-style outline");
                
                if (outlineEnabled)
                {
                    EditorGUI.indentLevel++;
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.outlineColor, "Outline Color", 
                        "Color of the outline");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.outlineWidth, "Outline Width", 
                        "Width of the outline");
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawWindSectionWithStyle(MaterialEditor materialEditor)
        {
            if (props.enableWind == null) return;
            
            showWind = DrawFoldoutSection("🍃 Wind Animation", showWind, () =>
            {
                EditorGUILayout.Space(5);
                
                bool windEnabled = DrawToggleProperty(materialEditor, props.enableWind, 
                    "Enable Wind", "Enable wind animation for vegetation");
                
                if (windEnabled)
                {
                    EditorGUI.indentLevel++;
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.windSpeed, "Wind Speed", 
                        "Speed of wind animation");
                    
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.windStrength, "Wind Strength", 
                        "Strength of wind effect");
                    
                    if (props.windDirection != null)
                    {
                        ToonShaderStyles.DrawPropertyField(materialEditor, props.windDirection, "Wind Direction", 
                            "Direction of wind (as vector)");
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawPerformanceSectionWithStyle(MaterialEditor materialEditor)
        {
            showPerformance = DrawFoldoutSection("⚡ Performance Settings", showPerformance, () =>
            {
                EditorGUILayout.Space(5);
                
                if (props.receiveShadows != null)
                {
                    DrawToggleProperty(materialEditor, props.receiveShadows, "Receive Shadows", 
                        "Allow this material to receive shadows");
                }
                
                if (props.enableAdditionalLights != null)
                {
                    DrawToggleProperty(materialEditor, props.enableAdditionalLights, "Additional Lights", 
                        "Enable additional light sources (impacts performance)");
                }
                
                if (props.lightmapInfluence != null)
                {
                    ToonShaderStyles.DrawPropertyField(materialEditor, props.lightmapInfluence, "Lightmap Influence", 
                        "Influence of baked lightmaps");
                }
                
                // Performance tips
                EditorGUILayout.Space(8);
                ToonShaderStyles.DrawInfoBox(
                    "💡 Performance Tips:\n" +
                    "• Disable unused features to improve performance\n" +
                    "• Use lower shadow steps for better performance\n" +
                    "• Consider disabling additional lights on mobile",
                    MessageType.Info);
                
                EditorGUILayout.Space(5);
            });
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(15);
            ToonShaderStyles.DrawSeparator();
            EditorGUILayout.Space(15);
            
            // Modern footer
            var footerRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(footerRect, new Color(ToonShaderStyles.darkBackground.r, ToonShaderStyles.darkBackground.g, ToonShaderStyles.darkBackground.b, 0.5f));
            
            GUILayout.Space(15);
            
            var copyrightStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = ToonShaderStyles.accentColor },
                alignment = TextAnchor.MiddleCenter
            };
            
            var versionStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 10,
                fontStyle = FontStyle.Italic,
                normal = { textColor = new Color(ToonShaderStyles.lightText.r, ToonShaderStyles.lightText.g, ToonShaderStyles.lightText.b, 0.7f) },
                alignment = TextAnchor.MiddleCenter
            };
            
            GUILayout.Label("Gorgonize Games © 2025", copyrightStyle);
            GUILayout.Space(2);
            GUILayout.Label("Gorgonize Toon Shader v0.7 Beta", versionStyle);
            
            GUILayout.Space(15);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
        }

        // Yardımcı metodlar
        private bool DrawFoldoutSection(string title, bool foldout, System.Action drawContent)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            
            GUILayout.Space(5);
            foldout = ToonShaderStyles.DrawFoldoutHeader(title, foldout);
            
            if (foldout)
            {
                EditorGUI.indentLevel++;
                drawContent?.Invoke();
                EditorGUI.indentLevel--;
            }
            
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            
            return foldout;
        }

        private bool DrawToggleProperty(MaterialEditor editor, MaterialProperty prop, string label, string tooltip)
        {
            if (prop == null) return false;
            
            var content = new GUIContent(label, tooltip);
            
            EditorGUI.BeginChangeCheck();
            bool value = EditorGUILayout.Toggle(content, prop.floatValue > 0.5f);
            
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value ? 1.0f : 0.0f;
            }
            
            return value;
        }
    }
}