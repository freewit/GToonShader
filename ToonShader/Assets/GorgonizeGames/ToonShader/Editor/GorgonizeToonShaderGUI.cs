using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Professional Asset Store Quality Material Editor for Gorgonize Toon Shader
    /// Designed for commercial distribution with premium user experience
    /// </summary>
    public class GorgonizeToonShaderGUI : ShaderGUI
    {
        #region Private Fields
        
        private ToonShaderProperties props;
        
        // Foldout states - Professional defaults
        private static bool showBase = true;
        private static bool showShadows = true;
        private static bool showHighlights = false;
        private static bool showRim = false;
        private static bool showAdvanced = false;
        private static bool showSubsurface = false;
        private static bool showOutline = false;
        private static bool showWind = false;
        private static bool showPerformance = false;
        private static bool showHelp = false;
        
        // Animation
        private static double lastUpdateTime;
        
        #endregion
        
        #region Main GUI Override
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Initialize professional styles
            ToonShaderStyles.Initialize();
            
            // Initialize properties
            props = new ToonShaderProperties(properties);
            
            // Begin change detection
            EditorGUI.BeginChangeCheck();
            
            // Professional header
            DrawProfessionalHeader();
            
            // Main content sections
            DrawMainContentSections(materialEditor);
            
            // Professional footer
            ToonShaderStyles.DrawProfessionalFooter();
            
            // Apply changes and update keywords
            if (EditorGUI.EndChangeCheck())
            {
                ApplyChanges(materialEditor);
            }
            
            // Force repaint for animations
            HandleAnimationRepaint();
        }
        
        #endregion
        
        #region Header Section
        
        private void DrawProfessionalHeader()
        {
            ToonShaderStyles.DrawProfessionalHeader(
                "GORGONIZE GAMES",
                "Professional Toon Shader",
                "v1.0 Asset Store Edition"
            );
            
            // Quick stats or info bar
            DrawQuickStatsBar();
        }
        
        private void DrawQuickStatsBar()
        {
            EditorGUILayout.Space(5);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                var statsStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = new Color(0.8f, 0.8f, 0.8f, 1f) },
                    fontSize = 9
                };
                
                // Feature count
                int enabledFeatures = CountEnabledFeatures();
                GUILayout.Label($"🎛️ {enabledFeatures} Features Active", statsStyle);
                
                GUILayout.Label("•", statsStyle);
                
                // Performance indicator
                string perfLevel = GetPerformanceLevel();
                GUILayout.Label("⚡ " + perfLevel + " Performance", statsStyle);
                
                GUILayout.Label("•", statsStyle);
                
                // Render pipeline
                GUILayout.Label("🔧 URP Compatible", statsStyle);
                
                GUILayout.FlexibleSpace();
            }
            
            EditorGUILayout.Space(8);
        }
        
        #endregion
        
        #region Main Content Sections
        
        private void DrawMainContentSections(MaterialEditor materialEditor)
        {
            // Base Properties - Always visible and important
            DrawBaseSectionProfessional(materialEditor);
            
            // Lighting & Shadows - Core toon functionality
            DrawShadowSectionProfessional(materialEditor);
            
            // Visual Effects
            DrawHighlightsSectionProfessional(materialEditor);
            DrawRimSectionProfessional(materialEditor);
            DrawOutlineSectionProfessional(materialEditor);
            DrawSubsurfaceSectionProfessional(materialEditor);
            
            // Advanced Features
            DrawAdvancedSectionProfessional(materialEditor);
            DrawWindSectionProfessional(materialEditor);
            
            // System & Performance
            DrawPerformanceSectionProfessional(materialEditor);
            
            // Help & Documentation
            DrawHelpSectionProfessional();
        }
        
        #endregion
        
        #region Individual Sections - Professional Implementation
        
        private void DrawBaseSectionProfessional(MaterialEditor materialEditor)
        {
            showBase = ToonShaderStyles.DrawProfessionalFoldout("Base Properties", showBase, "🎨");
            
            if (showBase)
            {
                ToonShaderStyles.DrawPropertyGroup("Surface", () =>
                {
                    if (props.IsPropertyValid(props.baseColor))
                        materialEditor.ShaderProperty(props.baseColor, "🎨 Albedo Color");
                        
                    if (props.IsPropertyValid(props.baseMap))
                        materialEditor.TexturePropertySingleLine(new GUIContent("🖼️ Albedo Texture"), props.baseMap);
                }, true);
                
                // Live preview tip
                ToonShaderStyles.DrawInfoBox("Changes are applied in real-time to the scene view for instant feedback.");
            }
        }
        
        private void DrawShadowSectionProfessional(MaterialEditor materialEditor)
        {
            showShadows = ToonShaderStyles.DrawProfessionalFoldout("Toon Lighting System", showShadows, "🌑");
            
            if (showShadows)
            {
                ToonShaderStyles.DrawPropertyGroup("Lighting Mode", () =>
                {
                    if (props.IsPropertyValid(props.lightingMode))
                        materialEditor.ShaderProperty(props.lightingMode, "⚙️ Lighting Method");
                }, true);

                var lightingMode = (int)props.GetFloatValue(props.lightingMode);

                // Draw controls based on selected mode
                switch (lightingMode)
                {
                    case 0: // Single Cell Mode
                        DrawSingleCellModeControls(materialEditor);
                        break;
                    case 1: // Banded Mode
                        DrawBandedModeControls(materialEditor);
                        break;
                    case 2: // Ramp Mode
                        DrawRampModeControls(materialEditor);
                        break;
                }

                // General lighting controls that apply to all modes
                ToonShaderStyles.DrawPropertyGroup("General Lighting Settings", () =>
                {
                    if (props.IsPropertyValid(props.occlusionStrength))
                        materialEditor.ShaderProperty(props.occlusionStrength, "🕳️ Baked AO Strength");
                }, true);
                
                ToonShaderStyles.DrawInfoBox("The lighting system creates the characteristic stylized lighting. 'Single Cell' is for classic cel-shading, 'Banded' adds more depth.");
            }
        }

        private void DrawSingleCellModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Single Cell (Tek Ton) Ayarları", () =>
            {
                if (props.IsPropertyValid(props.shadowThreshold))
                    materialEditor.ShaderProperty(props.shadowThreshold, "Gölge Eşiği");
                            
                if (props.IsPropertyValid(props.transitionSoftness))
                    materialEditor.ShaderProperty(props.transitionSoftness, "Geçiş Yumuşaklığı");

                if (props.IsPropertyValid(props.shadowColor))
                    materialEditor.ShaderProperty(props.shadowColor, "🎨 Gölge Rengi");
                        
                if (props.IsPropertyValid(props.tintShadowOnBase))
                    ToonShaderStyles.DrawFeatureToggle(props.tintShadowOnBase, "Shadow Tinting", "Apply shadow color to entire object", "🎭");
            }, true);
        }

        private void DrawBandedModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Banded (Çok Tonlu) Ayarları", () =>
            {
                if (props.IsPropertyValid(props.bandCount))
                {
                    EditorGUI.BeginChangeCheck();
                    float bandCountFloat = props.bandCount.floatValue;
                    int bandCountInt = EditorGUILayout.IntSlider("Bant Miktarı", (int)bandCountFloat, 1, 4);
                    if (EditorGUI.EndChangeCheck())
                    {
                        props.bandCount.floatValue = bandCountInt;
                    }
                }

                if (props.IsPropertyValid(props.shadowThreshold))
                    materialEditor.ShaderProperty(props.shadowThreshold, "Ana Gölge Eşiği");

                if (props.IsPropertyValid(props.midtoneThreshold) && props.IsPropertyValid(props.shadowThreshold))
                {
                    float shadowThresholdValue = props.shadowThreshold.floatValue;
                    if (props.midtoneThreshold.floatValue < shadowThresholdValue)
                    {
                        props.midtoneThreshold.floatValue = shadowThresholdValue;
                    }
                    props.midtoneThreshold.floatValue = EditorGUILayout.Slider("Ara Ton Bitişi", props.midtoneThreshold.floatValue, shadowThresholdValue, 1.0f);
                }
                            
                if (props.IsPropertyValid(props.bandSoftness))
                    materialEditor.ShaderProperty(props.bandSoftness, "Bant Yumuşaklığı");
                
                if (props.IsPropertyValid(props.shadowColor))
                    materialEditor.ShaderProperty(props.shadowColor, "🎨 Gölge Rengi");
            }, true);
        }
        
        private void DrawRampModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Ramp Texture (Doku) Ayarları", () =>
            {
                if (props.IsPropertyValid(props.shadowRamp))
                    materialEditor.TexturePropertySingleLine(new GUIContent("📈 Rampa Dokusu"), props.shadowRamp);
        
                ToonShaderStyles.DrawInfoBox("Aydınlatma ve gölge renkleri doğrudan bu dokudan alınır. Soldan sağa doğru karanlıktan aydınlığa bir gradyan kullanın.");

                if (GUILayout.Button("Ramp Editor", ToonShaderStyles.ButtonSecondaryStyle))
                {
                    EditorWindow.GetWindow<RampCreatorEditor>("Ramp Creator");
                }
            }, true);
        }

        private void DrawHighlightsSectionProfessional(MaterialEditor materialEditor)
        {
            showHighlights = ToonShaderStyles.DrawProfessionalFoldout("Specular Highlights", showHighlights, "✨");
            
            if (showHighlights)
            {
                if (props.IsPropertyValid(props.enableHighlights))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableHighlights, "Enable Specular Highlights", "Add cartoon-style reflections", "✨");
                }
                
                if (props.IsFeatureEnabled(props.enableHighlights))
                {
                    ToonShaderStyles.DrawPropertyGroup("Highlight Properties", () =>
                    {
                        if (props.IsPropertyValid(props.specularColor))
                            materialEditor.ShaderProperty(props.specularColor, "💎 Highlight Color");
                            
                        if (props.IsPropertyValid(props.specularSize))
                            materialEditor.ShaderProperty(props.specularSize, "🔍 Highlight Size");
                            
                        if (props.IsPropertyValid(props.specularSmoothness))
                            materialEditor.ShaderProperty(props.specularSmoothness, "🌊 Edge Smoothness");
                            
                        if (props.IsPropertyValid(props.specularSteps))
                            materialEditor.ShaderProperty(props.specularSteps, "📊 Highlight Steps");
                    }, true);
                    
                    ToonShaderStyles.DrawInfoBox("Lower step values create more dramatic, anime-style highlights.");
                }
            }
        }
        
        private void DrawRimSectionProfessional(MaterialEditor materialEditor)
        {
            showRim = ToonShaderStyles.DrawProfessionalFoldout("Rim Lighting", showRim, "🌟");
            
            if (showRim)
            {
                if (props.IsPropertyValid(props.enableRim))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableRim, "Enable Rim Lighting", "Creates glowing edge effect", "🌟");
                }
                
                if (props.IsFeatureEnabled(props.enableRim))
                {
                    ToonShaderStyles.DrawPropertyGroup("Rim Properties", () =>
                    {
                        if (props.IsPropertyValid(props.rimColor))
                            materialEditor.ShaderProperty(props.rimColor, "🌈 Rim Color");
                            
                        if (props.IsPropertyValid(props.rimPower))
                            materialEditor.ShaderProperty(props.rimPower, "⚡ Rim Power");
                            
                        if (props.IsPropertyValid(props.rimIntensity))
                            materialEditor.ShaderProperty(props.rimIntensity, "💪 Rim Intensity");
                            
                        if (props.IsPropertyValid(props.rimOffset))
                            materialEditor.ShaderProperty(props.rimOffset, "📏 Rim Offset");
                    }, true);
                    
                    ToonShaderStyles.DrawInfoBox("Rim lighting helps separate characters from backgrounds and adds dramatic flair.");
                }
            }
        }
        
        private void DrawOutlineSectionProfessional(MaterialEditor materialEditor)
        {
            showOutline = ToonShaderStyles.DrawProfessionalFoldout("Cartoon Outline", showOutline, "📝");
            
            if (showOutline)
            {
                if (props.IsPropertyValid(props.enableOutline))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableOutline, "Enable Outline Effect", "Classic toon shader outline", "📝");
                }
                
                if (props.IsFeatureEnabled(props.enableOutline))
                {
                    ToonShaderStyles.DrawPropertyGroup("Outline Settings", () =>
                    {
                        if (props.IsPropertyValid(props.outlineColor))
                            materialEditor.ShaderProperty(props.outlineColor, "🖍️ Outline Color");
                            
                        if (props.IsPropertyValid(props.outlineWidth))
                            materialEditor.ShaderProperty(props.outlineWidth, "📏 Outline Thickness");
                    }, true);
                    
                    ToonShaderStyles.DrawInfoBox("For best results, ensure your mesh has smooth normals. Outline uses vertex expansion technique.");
                    
                    if (props.IsPropertyValid(props.outlineWidth) && props.outlineWidth.floatValue > 5f)
                    {
                        ToonShaderStyles.DrawInfoBox("Very thick outlines may cause visual artifacts on complex geometry.", MessageType.Warning);
                    }
                }
            }
        }
        
        private void DrawSubsurfaceSectionProfessional(MaterialEditor materialEditor)
        {
            showSubsurface = ToonShaderStyles.DrawProfessionalFoldout("Subsurface Scattering", showSubsurface, "🌸");
            
            if (showSubsurface)
            {
                if (props.IsPropertyValid(props.enableSubsurface))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableSubsurface, "Enable Subsurface Scattering", "Light transmission through thin materials", "🌸");
                }
                
                if (props.IsFeatureEnabled(props.enableSubsurface))
                {
                    ToonShaderStyles.DrawPropertyGroup("Subsurface Properties", () =>
                    {
                        if (props.IsPropertyValid(props.subsurfaceColor))
                            materialEditor.ShaderProperty(props.subsurfaceColor, "🌺 Subsurface Color");
                            
                        if (props.IsPropertyValid(props.subsurfaceIntensity))
                            materialEditor.ShaderProperty(props.subsurfaceIntensity, "💪 Scattering Intensity");
                            
                        if (props.IsPropertyValid(props.subsurfaceDistortion))
                            materialEditor.ShaderProperty(props.subsurfaceDistortion, "🌊 Light Distortion");
                            
                        if (props.IsPropertyValid(props.subsurfacePower))
                            materialEditor.ShaderProperty(props.subsurfacePower, "⚡ Scattering Power");
                    }, true);
                    
                    ToonShaderStyles.DrawInfoBox("Perfect for skin, leaves, fabric, and other translucent materials.");
                }
            }
        }
        
        private void DrawAdvancedSectionProfessional(MaterialEditor materialEditor)
        {
            showAdvanced = ToonShaderStyles.DrawProfessionalFoldout("Surface Details", showAdvanced, "⚙️");
            
            if (showAdvanced)
            {
                ToonShaderStyles.DrawPropertyGroup("Normal Mapping", () =>
                {
                    if (props.IsPropertyValid(props.normalMap))
                    {
                        materialEditor.TexturePropertySingleLine(new GUIContent("🗺️ Normal Map"), props.normalMap, 
                            props.IsPropertyValid(props.normalStrength) ? props.normalStrength : null);
                    }
                }, true);
                
                ToonShaderStyles.DrawPropertyGroup("Detail Textures", () =>
                {
                    if (props.IsPropertyValid(props.detailMap))
                        materialEditor.TexturePropertySingleLine(new GUIContent("🔍 Detail Albedo"), props.detailMap);
                        
                    if (props.IsPropertyValid(props.detailNormalMap))
                        materialEditor.TexturePropertySingleLine(new GUIContent("🗺️ Detail Normal"), props.detailNormalMap);
                        
                    if (props.IsPropertyValid(props.detailStrength))
                        materialEditor.ShaderProperty(props.detailStrength, "💪 Detail Strength");
                }, true);
                
                ToonShaderStyles.DrawPropertyGroup("Emission", () =>
                {
                    if (props.IsPropertyValid(props.emissionMap))
                    {
                        materialEditor.TexturePropertyWithHDRColor(new GUIContent("💡 Emission Map"), props.emissionMap, 
                            props.IsPropertyValid(props.emissionColor) ? props.emissionColor : null, false);
                    }
                    
                    if (props.IsPropertyValid(props.emissionIntensity))
                        materialEditor.ShaderProperty(props.emissionIntensity, "🔆 Emission Intensity");
                }, true);
            }
        }
        
        private void DrawWindSectionProfessional(MaterialEditor materialEditor)
        {
            showWind = ToonShaderStyles.DrawProfessionalFoldout("Wind Animation", showWind, "🌬️");
            
            if (showWind)
            {
                if (props.IsPropertyValid(props.enableWind))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableWind, "Enable Wind Animation", "Procedural vertex animation", "🌬️");
                }
                
                if (props.IsFeatureEnabled(props.enableWind))
                {
                    ToonShaderStyles.DrawPropertyGroup("Wind Settings", () =>
                    {
                        if (props.IsPropertyValid(props.windSpeed))
                            materialEditor.ShaderProperty(props.windSpeed, "💨 Wind Speed");
                            
                        if (props.IsPropertyValid(props.windStrength))
                            materialEditor.ShaderProperty(props.windStrength, "💪 Wind Strength");
                            
                        if (props.IsPropertyValid(props.windDirection))
                            materialEditor.ShaderProperty(props.windDirection, "🧭 Wind Direction");
                    }, true);
                    
                    ToonShaderStyles.DrawInfoBox("Wind animation works best on vegetation and cloth objects.");
                }
            }
        }
        
        private void DrawPerformanceSectionProfessional(MaterialEditor materialEditor)
        {
            showPerformance = ToonShaderStyles.DrawProfessionalFoldout("Rendering & Performance", showPerformance, "⚡");
            
            if (showPerformance)
            {
                ToonShaderStyles.DrawPropertyGroup("Lighting Options", () =>
                {
                    if (props.IsPropertyValid(props.receiveShadows))
                        materialEditor.ShaderProperty(props.receiveShadows, "🌑 Receive Shadows");
                        
                    if (props.IsPropertyValid(props.enableAdditionalLights))
                        materialEditor.ShaderProperty(props.enableAdditionalLights, "💡 Additional Lights");
                        
                    if (props.IsPropertyValid(props.lightmapInfluence))
                        materialEditor.ShaderProperty(props.lightmapInfluence, "🗺️ Lightmap Influence");
                }, true);
                
                // Performance recommendations
                ToonShaderStyles.DrawPropertyGroup("Optimization Tips", () =>
                {
                    string perfTips = GetPerformanceTips();
                    ToonShaderStyles.DrawInfoBox(perfTips);
                }, false);
            }
        }
        
        private void DrawHelpSectionProfessional()
        {
            showHelp = ToonShaderStyles.DrawProfessionalFoldout("Help & Resources", showHelp, "📚");
            
            if (showHelp)
            {
                ToonShaderStyles.DrawPropertyGroup("Quick Start", () =>
                {
                    EditorGUILayout.LabelField("1. 🎨 Set your base color and texture");
                    EditorGUILayout.LabelField("2. 🌑 Configure shadow steps (2-4 for cartoon look)");
                    EditorGUILayout.LabelField("3. 📝 Enable outline for classic toon style");
                    EditorGUILayout.LabelField("4. ✨ Add highlights and rim lighting as needed");
                    EditorGUILayout.LabelField("5. ⚡ Optimize performance settings");
                }, false);
                
                ToonShaderStyles.DrawPropertyGroup("Common Use Cases", () =>
                {
                    if (GUILayout.Button("🎭 Anime Character Setup"))
                        ApplyAnimePreset();
                    if (GUILayout.Button("🌿 Vegetation Setup"))
                        ApplyVegetationPreset();
                    if (GUILayout.Button("🏢 Environment Setup"))
                        ApplyEnvironmentPreset();
                }, false);
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        private void ApplyChanges(MaterialEditor materialEditor)
        {
            foreach (var obj in materialEditor.targets)
            {
                EditorUtility.SetDirty(obj);
                if (obj is Material material)
                {
                    props.UpdateKeywords(material);
                }
            }
        }
        
        private void HandleAnimationRepaint()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - lastUpdateTime > 0.1) // 10 FPS for smooth animations
            {
                lastUpdateTime = currentTime;
                if (EditorWindow.focusedWindow != null)
                {
                    EditorWindow.focusedWindow.Repaint();
                }
            }
        }
        
        private int CountEnabledFeatures()
        {
            int count = 0;
            if (props.IsFeatureEnabled(props.enableHighlights)) count++;
            if (props.IsFeatureEnabled(props.enableRim)) count++;
            if (props.IsFeatureEnabled(props.enableOutline)) count++;
            if (props.IsFeatureEnabled(props.enableSubsurface)) count++;
            if (props.IsFeatureEnabled(props.enableWind)) count++;
            return count;
        }
        
        private string GetPerformanceLevel()
        {
            int features = CountEnabledFeatures();
            return features switch
            {
                0 => "Maximum",
                1 or 2 => "High",
                3 or 4 => "Medium",
                _ => "Custom"
            };
        }
        
        private string GetPerformanceTips()
        {
            int features = CountEnabledFeatures();
            if (features > 3)
                return "💡 Consider disabling unused features for better performance on mobile devices.";
            else if (features == 0)
                return "⚡ Maximum performance mode active. Enable features as needed for your visual style.";
            else
                return "✅ Good balance of features and performance.";
        }
        
        #endregion
        
        #region Preset Methods
        
        private void ApplyAnimePreset()
        {
            if (props.IsPropertyValid(props.shadowThreshold))
                props.shadowThreshold.floatValue = 0.5f;
            if (props.IsPropertyValid(props.enableOutline))
                props.enableOutline.floatValue = 1f;
            if (props.IsPropertyValid(props.enableHighlights))
                props.enableHighlights.floatValue = 1f;
            if (props.IsPropertyValid(props.enableRim))
                props.enableRim.floatValue = 1f;
        }
        
        private void ApplyVegetationPreset()
        {
            if (props.IsPropertyValid(props.enableWind))
                props.enableWind.floatValue = 1f;
            if (props.IsPropertyValid(props.enableSubsurface))
                props.enableSubsurface.floatValue = 1f;
            if (props.IsPropertyValid(props.shadowThreshold))
                props.shadowThreshold.floatValue = 0.6f;
        }
        
        private void ApplyEnvironmentPreset()
        {
            if (props.IsPropertyValid(props.shadowThreshold))
                props.shadowThreshold.floatValue = 0.5f;
            if (props.IsPropertyValid(props.enableOutline))
                props.enableOutline.floatValue = 0f;
        }
        
        #endregion
    }
}

