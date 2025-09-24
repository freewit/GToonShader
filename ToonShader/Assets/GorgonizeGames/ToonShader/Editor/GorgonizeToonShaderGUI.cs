using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Gorgonize.ToonShader.Editor
{
    public class GorgonizeToonShaderGUI : ShaderGUI
    {
        private ToonShaderProperties props;
        private int currentMaterialHash;
        private MaterialEditor m_MaterialEditor;

        private static readonly Dictionary<string, (string DisplayName, string Icon, string ToggleProperty)> Features = new Dictionary<string, (string, string, string)>
        {
            { "GTOON_FEATURE_ADVANCEDLIGHTING", ("Advanced Lighting System", "🌑", "_FeatureAdvancedLightingToggle") },
            { "GTOON_FEATURE_SPECULARSYSTEM", ("Specular System", "✨", "_FeatureSpecularSystemToggle") },
            { "GTOON_FEATURE_ADVANCEDRIMLIGHTING", ("Advanced Rim Lighting", "🌟", "_FeatureAdvancedRimLightingToggle") },
            { "GTOON_FEATURE_SMARTOUTLINESYSTEM", ("Smart Outline System", "📝", "_FeatureSmartOutlineSystemToggle") },
            { "GTOON_FEATURE_SUBSURFACESCATTERING", ("Subsurface Scattering", "🌸", "_FeatureSubsurfaceScatteringToggle") },
            { "GTOON_FEATURE_ADVANCEDSURFACEDETAILS", ("Advanced Surface Details", "⚙️", "_FeatureAdvancedSurfaceDetailsToggle") },
            { "GTOON_FEATURE_ADVANCEDWINDSYSTEM", ("Advanced Wind System", "🌬️", "_FeatureAdvancedWindSystemToggle") },
            { "GTOON_FEATURE_RENDERINGANDPERFORMANCE", ("Rendering & Performance", "⚡", "_FeatureRenderingAndPerformanceToggle") },
            { "GTOON_FEATURE_HELPANDRESOURCES", ("Help & Resources", "📚", "_FeatureHelpAndResourcesToggle") }
        };

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            m_MaterialEditor = materialEditor;
            var material = materialEditor.target as Material;
            if (material == null) return;

            currentMaterialHash = material.GetInstanceID();

            ToonShaderStyles.Initialize();
            props = new ToonShaderProperties(properties);
            
            EditorGUI.BeginChangeCheck();

            DrawProfessionalHeader();
            DrawMainContentSections(materialEditor);

            if (EditorGUI.EndChangeCheck())
            {
                ApplyChanges(materialEditor);
            }
        }

        private void DrawProfessionalHeader()
        {
            ToonShaderStyles.DrawProfessionalHeader(
                "GORGONIZE GAMES",
                "Professional Toon Shader",
                "v1.0 Asset Store Edition"
            );
        }

        private void DrawMainContentSections(MaterialEditor materialEditor)
        {
            DrawBaseSectionProfessional(materialEditor);
            DrawActiveFeatures(materialEditor);
            
            EditorGUILayout.Space(10);
            if (GUILayout.Button("✚ Feature Catalog...", ToonShaderStyles.ButtonPrimaryStyle, GUILayout.Height(35)))
            {
                FeatureCatalogEditor.ShowWindow(materialEditor);
            }
            EditorGUILayout.Space(5);

            ToonShaderStyles.DrawProfessionalFooter();
        }

        private void DrawActiveFeatures(MaterialEditor materialEditor)
        {
            foreach (var feature in Features)
            {
                MaterialProperty toggleProp = FindProperty(feature.Value.ToggleProperty, props.All);
                if (toggleProp != null && toggleProp.floatValue > 0.5f)
                {
                    DrawFeatureSection(materialEditor, feature.Key);
                }
            }
        }

        private void DrawFeatureSection(MaterialEditor materialEditor, string featureKey)
        {
            switch (featureKey)
            {
                case "GTOON_FEATURE_ADVANCEDLIGHTING": DrawShadowSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_SPECULARSYSTEM": DrawHighlightsSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_ADVANCEDRIMLIGHTING": DrawRimSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_SMARTOUTLINESYSTEM": DrawOutlineSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_SUBSURFACESCATTERING": DrawSubsurfaceSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_ADVANCEDSURFACEDETAILS": DrawAdvancedSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_ADVANCEDWINDSYSTEM": DrawWindSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_RENDERINGANDPERFORMANCE": DrawPerformanceSectionProfessional(materialEditor, featureKey); break;
                case "GTOON_FEATURE_HELPANDRESOURCES": DrawHelpSectionProfessional(featureKey); break;
            }
        }
        
        private bool DrawFeatureFoldout(string title, string icon, string featureKey, bool canBeRemoved = true)
        {
            string sessionKey = $"{currentMaterialHash}_{featureKey}";
            bool currentState = SessionState.GetBool(sessionKey, false);

            EditorGUILayout.Space(5);
            var rect = EditorGUILayout.GetControlRect(false, 30);
            
            var bgRect = new Rect(rect.x - 8, rect.y - 2, rect.width + 16, rect.height + 4);
            EditorGUI.DrawRect(bgRect, new Color32(45, 48, 58, 255));
            var accentRect = new Rect(bgRect.x, bgRect.y, 4, bgRect.height);
            EditorGUI.DrawRect(accentRect, currentState ? ToonShaderStyles.AccentBlue : new Color(ToonShaderStyles.AccentBlue.r, ToonShaderStyles.AccentBlue.g, ToonShaderStyles.AccentBlue.b, 0.5f));
            
            if (canBeRemoved)
            {
                var removeButtonRect = new Rect(rect.xMax - 28, rect.y + 7, 20, 20);
                var removeButtonStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, normal = {textColor = new Color(0.8f, 0.5f, 0.5f)}, fontStyle = FontStyle.Bold };

                if (GUI.Button(removeButtonRect, "✖", removeButtonStyle))
                {
                    RemoveFeature(featureKey);
                    return currentState; 
                }
            }
            
            var contentRect = new Rect(rect.x + 8, rect.y, rect.width - (canBeRemoved ? 40 : 8), rect.height);
            string displayTitle = $"{icon}  {title}";
            var titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13, fontStyle = FontStyle.Bold, normal = { textColor = currentState ? ToonShaderStyles.AccentBlue : Color.white }, alignment = TextAnchor.MiddleLeft };
            
            if (GUI.Button(contentRect, displayTitle, titleStyle))
            {
                currentState = !currentState;
                SessionState.SetBool(sessionKey, currentState);
            }
            
            return currentState;
        }

        private void RemoveFeature(string featureKey)
        {
            if (Features.TryGetValue(featureKey, out var featureInfo))
            {
                 MaterialProperty toggleProp = FindProperty(featureInfo.ToggleProperty, props.All);
                 if (toggleProp != null)
                 {
                     toggleProp.floatValue = 0f;
                 }
            }
        }
        
        #region Section Drawing Methods
        
        private void DrawBaseSectionProfessional(MaterialEditor materialEditor)
        {
            if (DrawFeatureFoldout("Base Properties", "🎨", "BaseProperties", canBeRemoved: false))
            {
                ToonShaderStyles.DrawPropertyGroup("Surface", () =>
                {
                    if (props.IsPropertyValid(props.baseColor)) materialEditor.ShaderProperty(props.baseColor, "🎨 Albedo Color");
                    if (props.IsPropertyValid(props.baseMap)) materialEditor.TexturePropertySingleLine(new GUIContent("🖼️ Albedo Texture"), props.baseMap);
                    if (props.IsPropertyValid(props.metallic)) materialEditor.ShaderProperty(props.metallic, "⚙️ Metallic");
                    if (props.IsPropertyValid(props.smoothness)) materialEditor.ShaderProperty(props.smoothness, "✨ Smoothness");
                }, true);
                
                ToonShaderStyles.DrawPropertyGroup("Reflections", () =>
                {
                    if(props.IsPropertyValid(props.enableEnvironmentReflections))
                        ToonShaderStyles.DrawFeatureToggle(props.enableEnvironmentReflections, "Environment Reflections", "Enable realistic reflections from the skybox or probes.", "🌍");

                    if (props.IsFeatureEnabled(props.enableEnvironmentReflections) && props.IsPropertyValid(props.environmentReflections))
                        materialEditor.ShaderProperty(props.environmentReflections, "Reflection Strength");
                }, true);
            }
        }

        private void DrawShadowSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                ToonShaderStyles.DrawPropertyGroup("Lighting Mode", () =>
                {
                    if (props.IsPropertyValid(props.lightingMode))
                        materialEditor.ShaderProperty(props.lightingMode, "⚙️ Lighting Method");
                }, true);

                var lightingMode = (int)props.GetFloatValue(props.lightingMode);

                switch (lightingMode)
                {
                    case 0: DrawSingleCellModeControls(materialEditor); break;
                    case 1: DrawDualCellModeControls(materialEditor); break;
                    case 2: DrawEnhancedBandedModeControls(materialEditor); break;
                    case 3: DrawGradientRampModeControls(materialEditor); break;
                    case 4: DrawCustomRampModeControls(materialEditor); break;
                }

                ToonShaderStyles.DrawPropertyGroup("General Lighting", () =>
                {
                    if (props.IsPropertyValid(props.occlusionStrength)) materialEditor.ShaderProperty(props.occlusionStrength, "🕳️ Baked AO Strength");
                    if (props.IsPropertyValid(props.lightmapInfluence)) materialEditor.ShaderProperty(props.lightmapInfluence, "🗺️ Lightmap Influence");
                    if (props.IsPropertyValid(props.receiveShadows)) ToonShaderStyles.DrawFeatureToggle(props.receiveShadows, "Receive Shadows", "Object can receive shadows from other objects.", "🌑");
                }, true);
            }
        }
        
        private void DrawHighlightsSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if(DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                if(props.IsPropertyValid(props.enableSpecularHighlights))
                    ToonShaderStyles.DrawFeatureToggle(props.enableSpecularHighlights, "Enable Toon Specular", "Enable stylized, artistic highlights.", "✨");

                if (props.IsFeatureEnabled(props.enableSpecularHighlights))
                {
                    ToonShaderStyles.DrawPropertyGroup("Specular Mode", () =>
                    {
                        if (props.IsPropertyValid(props.specularMode))
                            materialEditor.ShaderProperty(props.specularMode, "⚙️ Specular Method");
                    }, true);

                    var specularMode = (int)props.GetFloatValue(props.specularMode);

                    switch (specularMode)
                    {
                        case 0: DrawSteppedSpecularControls(materialEditor); break;
                        case 1: DrawSoftSpecularControls(materialEditor); break;
                        case 2: DrawAnisotropicSpecularControls(materialEditor); break;
                        case 3: DrawSparkleSpecularControls(materialEditor); break;
                        case 4: DrawDoubleToneSpecularControls(materialEditor); break;
                        case 5: DrawMatcapSpecularControls(materialEditor); break;
                        case 6: DrawHairSpecularControls(materialEditor); break;
                    }
                }
            }
        }
        
        private void DrawRimSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                if (props.IsPropertyValid(props.enableRim))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableRim, "Enable Rim Lighting", "Creates glowing edge effect", "🌟");
                }
                
                if (props.IsFeatureEnabled(props.enableRim))
                {
                    ToonShaderStyles.DrawPropertyGroup("Rim Mode", () => {
                        if(props.IsPropertyValid(props.rimMode))
                            materialEditor.ShaderProperty(props.rimMode, "⚙️ Rim Method");
                    }, true);

                    var rimMode = (int)props.GetFloatValue(props.rimMode);
                    switch(rimMode)
                    {
                        case 0: DrawStandardRimControls(materialEditor); break;
                        case 1: DrawSteppedRimControls(materialEditor); break;
                        case 2: DrawLightBasedRimControls(materialEditor); break;
                        case 3: DrawTexturedRimControls(materialEditor); break;
                        case 4: DrawFresnelEnhancedRimControls(materialEditor); break;
                        case 5: DrawColorGradientRimControls(materialEditor); break;
                    }
                }
            }
        }
        
        private void DrawOutlineSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                if (props.IsPropertyValid(props.enableOutline))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableOutline, "Enable Outline Effect", "Classic toon shader outline", "📝");
                }

                if (props.IsFeatureEnabled(props.enableOutline))
                {
                    ToonShaderStyles.DrawPropertyGroup("Main Settings", () =>
                    {
                        if (props.IsPropertyValid(props.outlineColor)) materialEditor.ShaderProperty(props.outlineColor, "🖍️ Outline Color");
                        if (props.IsPropertyValid(props.outlineWidth)) materialEditor.ShaderProperty(props.outlineWidth, "📏 Base Thickness");
                        if (props.IsPropertyValid(props.outlineMode)) materialEditor.ShaderProperty(props.outlineMode, "Extrusion Mode");
                    }, true);
                }
            }
        }
        
        private void DrawSubsurfaceSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                if (props.IsPropertyValid(props.enableSubsurface))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableSubsurface, "Enable Subsurface Scattering", "Light transmission through thin materials", "🌸");
                }
                
                if (props.IsFeatureEnabled(props.enableSubsurface))
                {
                    ToonShaderStyles.DrawPropertyGroup("SSS Mode", () => {
                         if(props.IsPropertyValid(props.subsurfaceMode))
                            materialEditor.ShaderProperty(props.subsurfaceMode, "Scattering Mode");
                    }, true);

                    ToonShaderStyles.DrawPropertyGroup("SSS Properties", () =>
                    {
                        if (props.IsPropertyValid(props.subsurfaceColor)) materialEditor.ShaderProperty(props.subsurfaceColor, "🌺 Subsurface Color");
                        if (props.IsPropertyValid(props.subsurfaceIntensity)) materialEditor.ShaderProperty(props.subsurfaceIntensity, "💪 Scattering Intensity");
                        if (props.IsPropertyValid(props.subsurfaceDistortion)) materialEditor.ShaderProperty(props.subsurfaceDistortion, "🌊 Light Distortion");
                        if (props.IsPropertyValid(props.subsurfacePower)) materialEditor.ShaderProperty(props.subsurfacePower, "⚡ Scattering Power");
                    }, true);
                }
            }
        }

        private void DrawAdvancedSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                ToonShaderStyles.DrawPropertyGroup("Normal & Parallax Mapping", () =>
                {
                    if (props.IsPropertyValid(props.normalMap))
                        materialEditor.TexturePropertySingleLine(new GUIContent("🗺️ Normal Map"), props.normalMap, props.normalStrength);

                    EditorGUILayout.Space();

                    if(props.IsPropertyValid(props.enableParallax))
                        ToonShaderStyles.DrawFeatureToggle(props.enableParallax, "Parallax Mapping", "Creates depth illusion using a height map.", "🧊");
                    
                    if(props.IsFeatureEnabled(props.enableParallax))
                    {
                        if(props.IsPropertyValid(props.heightMap))
                            materialEditor.TexturePropertySingleLine(new GUIContent("📈 Height Map"), props.heightMap, props.heightScale);
                    }
                }, true);
                
                ToonShaderStyles.DrawPropertyGroup("Detail Textures", () =>
                {
                    if (props.IsPropertyValid(props.detailMap)) materialEditor.TexturePropertySingleLine(new GUIContent("🔍 Detail Albedo"), props.detailMap);
                    if (props.IsPropertyValid(props.detailNormalMap)) materialEditor.TexturePropertySingleLine(new GUIContent("🗺️ Detail Normal"), props.detailNormalMap, props.detailNormalScale);
                    if (props.IsPropertyValid(props.detailStrength)) materialEditor.ShaderProperty(props.detailStrength, "💪 Detail Strength");
                }, true);
                
                ToonShaderStyles.DrawPropertyGroup("Emission", () =>
                {
                    if (props.IsPropertyValid(props.emissionMap))
                    {
                        materialEditor.TexturePropertyWithHDRColor(new GUIContent("💡 Emission Map"), props.emissionMap, 
                            props.IsPropertyValid(props.emissionColor) ? props.emissionColor : null, false);
                    }
                    
                    if(props.IsPropertyValid(props.enableEmissionPulse))
                        ToonShaderStyles.DrawFeatureToggle(props.enableEmissionPulse, "Pulse Animation", "Animates emission intensity over time.", "💓");

                    if(props.IsFeatureEnabled(props.enableEmissionPulse) && props.IsPropertyValid(props.pulseSpeed))
                        materialEditor.ShaderProperty(props.pulseSpeed, "Pulse Speed");
                }, true);
            }
        }
        
        private void DrawWindSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                if (props.IsPropertyValid(props.enableWind))
                {
                    ToonShaderStyles.DrawFeatureToggle(props.enableWind, "Enable Wind Animation", "Procedural vertex animation", "🌬️");
                }
                
                if (props.IsFeatureEnabled(props.enableWind))
                {
                    ToonShaderStyles.DrawPropertyGroup("Wind Mode", () => {
                         if(props.IsPropertyValid(props.windMode))
                            materialEditor.ShaderProperty(props.windMode, "Wind Simulation Mode");
                    }, true);

                    ToonShaderStyles.DrawPropertyGroup("Basic Wind", () =>
                    {
                        if (props.IsPropertyValid(props.windSpeed)) materialEditor.ShaderProperty(props.windSpeed, "💨 Wind Speed");
                        if (props.IsPropertyValid(props.windStrength)) materialEditor.ShaderProperty(props.windStrength, "💪 Wind Strength");
                        if (props.IsPropertyValid(props.windDirection)) materialEditor.ShaderProperty(props.windDirection, "🧭 Wind Direction");
                    }, true);
                }
            }
        }

        private void DrawPerformanceSectionProfessional(MaterialEditor materialEditor, string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                ToonShaderStyles.DrawPropertyGroup("Performance Options", () =>
                {
                    if (props.IsPropertyValid(props.enableAdditionalLights))
                        ToonShaderStyles.DrawFeatureToggle(props.enableAdditionalLights, "Additional Lights", "Allow more than one light to affect the object.", "💡");
                }, true);
            }
        }

        private void DrawHelpSectionProfessional(string featureKey)
        {
            if (DrawFeatureFoldout(Features[featureKey].DisplayName, Features[featureKey].Icon, featureKey))
            {
                 ToonShaderStyles.DrawPropertyGroup("Common Use Cases", () =>
                {
                    if (GUILayout.Button("🎭 Anime Character Setup")) { /* Apply preset logic */ }
                    if (GUILayout.Button("🌿 Vegetation Setup")) { /* Apply preset logic */ }
                    if (GUILayout.Button("🏢 Environment Setup")) { /* Apply preset logic */ }
                }, false);
            }
        }

        #endregion

        #region Sub-drawing Methods
        
        private void DrawSingleCellModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Single Cell (Classic Toon)", () =>
            {
                if (props.IsPropertyValid(props.shadowThreshold)) materialEditor.ShaderProperty(props.shadowThreshold, "Shadow Threshold");
                if (props.IsPropertyValid(props.transitionSoftness)) materialEditor.ShaderProperty(props.transitionSoftness, "Transition Softness");
                if (props.IsPropertyValid(props.shadowContrast)) materialEditor.ShaderProperty(props.shadowContrast, "Shadow Contrast");
                if (props.IsPropertyValid(props.shadowColor)) materialEditor.ShaderProperty(props.shadowColor, "🎨 Shadow Color");
                if (props.IsPropertyValid(props.tintShadowOnBase)) ToonShaderStyles.DrawFeatureToggle(props.tintShadowOnBase, "Tint Shadow on Base", "Apply shadow color to entire object", "🎭");
            }, true);
        }

        private void DrawDualCellModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Dual Cell (Anime Style)", () =>
            {
                if (props.IsPropertyValid(props.primaryThreshold)) materialEditor.ShaderProperty(props.primaryThreshold, "Primary Threshold");
                if (props.IsPropertyValid(props.primaryShadowColor)) materialEditor.ShaderProperty(props.primaryShadowColor, "🎨 Primary Shadow Color");
                EditorGUILayout.Space();
                if (props.IsPropertyValid(props.secondaryThreshold)) materialEditor.ShaderProperty(props.secondaryThreshold, "Secondary Threshold");
                if (props.IsPropertyValid(props.secondaryShadowColor)) materialEditor.ShaderProperty(props.secondaryShadowColor, "🎨 Secondary Shadow Color");
                if (props.IsPropertyValid(props.transitionSoftness)) materialEditor.ShaderProperty(props.transitionSoftness, "Transition Softness");
            }, true);
        }

        private void DrawEnhancedBandedModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Enhanced Banded", () =>
            {
                if (props.IsPropertyValid(props.bandCount)) materialEditor.ShaderProperty(props.bandCount, "Band Count");
                if (props.IsPropertyValid(props.midtoneThreshold)) materialEditor.ShaderProperty(props.midtoneThreshold, "Midtone Threshold");
                if (props.IsPropertyValid(props.bandSoftness)) materialEditor.ShaderProperty(props.bandSoftness, "Band Softness");
                if (props.IsPropertyValid(props.bandDistribution)) materialEditor.ShaderProperty(props.bandDistribution, "Band Distribution");
                if (props.IsPropertyValid(props.shadowColor)) materialEditor.ShaderProperty(props.shadowColor, "🎨 Shadow Tint");
            }, true);
        }

        private void DrawGradientRampModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Gradient Ramp", () =>
            {
                if (props.IsPropertyValid(props.shadowRamp)) materialEditor.TexturePropertySingleLine(new GUIContent("📈 Gradient Ramp Texture"), props.shadowRamp);
                if (props.IsPropertyValid(props.rampIntensity)) materialEditor.ShaderProperty(props.rampIntensity, "Ramp Intensity");
            }, true);
        }
        
        private void DrawCustomRampModeControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Custom Ramp", () =>
            {
                if (props.IsPropertyValid(props.customRamp)) materialEditor.TexturePropertySingleLine(new GUIContent("🎨 Custom Ramp Texture"), props.customRamp);
                if (props.IsPropertyValid(props.rampIntensity)) materialEditor.ShaderProperty(props.rampIntensity, "Ramp Intensity");
                if (GUILayout.Button("Open Ramp Creator", ToonShaderStyles.ButtonSecondaryStyle)) EditorWindow.GetWindow<RampCreatorEditor>("Ramp Creator");
            }, true);
        }

        private void DrawSteppedSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Stepped (Cel) Specular", () =>
            {
                if (props.IsPropertyValid(props.specularColor)) materialEditor.ShaderProperty(props.specularColor, "💎 Highlight Color");
                if (props.IsPropertyValid(props.specularSize)) materialEditor.ShaderProperty(props.specularSize, "🔍 Highlight Size");
                if (props.IsPropertyValid(props.steppedFalloff)) materialEditor.ShaderProperty(props.steppedFalloff, "🍂 Falloff");
                if (props.IsPropertyValid(props.specularSmoothness)) materialEditor.ShaderProperty(props.specularSmoothness, "🌊 Edge Smoothness");
                if (props.IsPropertyValid(props.specularSteps)) materialEditor.ShaderProperty(props.specularSteps, "📊 Highlight Steps");
            }, true);
        }

        private void DrawSoftSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Soft (Blinn-Phong) Specular", () =>
            {
                if (props.IsPropertyValid(props.specularColor)) materialEditor.ShaderProperty(props.specularColor, "💎 Highlight Color");
                if (props.IsPropertyValid(props.softSpecularGlossiness)) materialEditor.ShaderProperty(props.softSpecularGlossiness, "✨ Glossiness");
                if (props.IsPropertyValid(props.softSpecularStrength)) materialEditor.ShaderProperty(props.softSpecularStrength, "💪 Strength");
                if (props.IsPropertyValid(props.softSpecularMask)) materialEditor.TexturePropertySingleLine(new GUIContent("🎭 Specular Mask"), props.softSpecularMask);
            }, true);
        }
        
        private void DrawAnisotropicSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Enhanced Anisotropic Specular", () =>
            {
                if (props.IsPropertyValid(props.specularColor)) materialEditor.ShaderProperty(props.specularColor, "💎 Highlight Color");
                if (props.IsPropertyValid(props.anisotropicDirection)) materialEditor.ShaderProperty(props.anisotropicDirection, "↔️ Direction");
                if (props.IsPropertyValid(props.anisotropicSharpness)) materialEditor.ShaderProperty(props.anisotropicSharpness, "🔪 Sharpness");
                if (props.IsPropertyValid(props.anisotropicIntensity)) materialEditor.ShaderProperty(props.anisotropicIntensity, "💪 Intensity");
                if (props.IsPropertyValid(props.anisotropicOffset)) materialEditor.ShaderProperty(props.anisotropicOffset, "📏 Offset");
                if (props.IsPropertyValid(props.anisotropicFlowMap)) materialEditor.TexturePropertySingleLine(new GUIContent("🌊 Flow Map"), props.anisotropicFlowMap);
            }, true);
        }
        
        private void DrawSparkleSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Animated Sparkle Specular", () =>
            {
                if (props.IsPropertyValid(props.sparkleColor)) materialEditor.ShaderProperty(props.sparkleColor, "✨ Sparkle Color");
                if (props.IsPropertyValid(props.sparkleMap)) materialEditor.TexturePropertySingleLine(new GUIContent("✨ Sparkle Pattern"), props.sparkleMap);
                if (props.IsPropertyValid(props.sparkleDensity)) materialEditor.ShaderProperty(props.sparkleDensity, "🎛️ Density");
                if (props.IsPropertyValid(props.sparkleSize)) materialEditor.ShaderProperty(props.sparkleSize, "🔍 Size");
                if (props.IsPropertyValid(props.sparkleAnimSpeed)) materialEditor.ShaderProperty(props.sparkleAnimSpeed, "🌀 Animation Speed");
            }, true);
        }
        
        private void DrawDoubleToneSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Double Tone Specular", () =>
            {
                if (props.IsPropertyValid(props.specularInnerColor)) materialEditor.ShaderProperty(props.specularInnerColor, "🎨 Inner Color");
                if (props.IsPropertyValid(props.specularOuterColor)) materialEditor.ShaderProperty(props.specularOuterColor, "🎨 Outer Color");
                if (props.IsPropertyValid(props.specularInnerSize)) materialEditor.ShaderProperty(props.specularInnerSize, "🔍 Inner Size");
                if (props.IsPropertyValid(props.specularOuterSize)) materialEditor.ShaderProperty(props.specularOuterSize, "🔍 Outer Size");
                if (props.IsPropertyValid(props.specularDoubleToneSoftness)) materialEditor.ShaderProperty(props.specularDoubleToneSoftness, "🌊 Softness");
            }, true);
        }

        private void DrawMatcapSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Matcap Mode", () =>
            {
                if(props.IsPropertyValid(props.matcapTex)) materialEditor.TexturePropertySingleLine(new GUIContent("🖼️ Matcap Texture"), props.matcapTex);
                if(props.IsPropertyValid(props.matcapIntensity)) materialEditor.ShaderProperty(props.matcapIntensity, "💪 Intensity");
                if(props.IsPropertyValid(props.matcapBlendWithLighting)) ToonShaderStyles.DrawFeatureToggle(props.matcapBlendWithLighting, "Blend with Lighting", "If disabled, Matcap will override all other lighting.", "🔄");
            }, true);
        }
        
        private void DrawHairSpecularControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Hair/Fur Mode (Kajiya-Kay)", () =>
            {
                if(props.IsPropertyValid(props.hairPrimaryColor)) materialEditor.ShaderProperty(props.hairPrimaryColor, "🎨 Primary Color");
                if(props.IsPropertyValid(props.hairPrimaryExponent)) materialEditor.ShaderProperty(props.hairPrimaryExponent, "Primary Exponent");
                if(props.IsPropertyValid(props.hairPrimaryShift)) materialEditor.ShaderProperty(props.hairPrimaryShift, "Primary Shift");
                EditorGUILayout.Space();
                if(props.IsPropertyValid(props.hairSecondaryColor)) materialEditor.ShaderProperty(props.hairSecondaryColor, "🎨 Secondary Color");
                if(props.IsPropertyValid(props.hairSecondaryExponent)) materialEditor.ShaderProperty(props.hairSecondaryExponent, "Secondary Exponent");
                if(props.IsPropertyValid(props.hairSecondaryShift)) materialEditor.ShaderProperty(props.hairSecondaryShift, "Secondary Shift");
            }, true);
        }

        private void DrawStandardRimControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Standard Rim", () => {
                if (props.IsPropertyValid(props.rimColor)) materialEditor.ShaderProperty(props.rimColor, "Rim Color");
                if (props.IsPropertyValid(props.rimPower)) materialEditor.ShaderProperty(props.rimPower, "Rim Power");
                if (props.IsPropertyValid(props.rimIntensity)) materialEditor.ShaderProperty(props.rimIntensity, "Rim Intensity");
                if (props.IsPropertyValid(props.rimOffset)) materialEditor.ShaderProperty(props.rimOffset, "Rim Offset");
            }, true);
        }

        private void DrawSteppedRimControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Stepped Rim", () => {
                if(props.IsPropertyValid(props.rimColor)) materialEditor.ShaderProperty(props.rimColor, "Rim Color");
                if(props.IsPropertyValid(props.rimThreshold)) materialEditor.ShaderProperty(props.rimThreshold, "Threshold");
                if(props.IsPropertyValid(props.rimSoftness)) materialEditor.ShaderProperty(props.rimSoftness, "Softness");
                if(props.IsPropertyValid(props.rimIntensity)) materialEditor.ShaderProperty(props.rimIntensity, "Intensity");
            }, true);
        }

        private void DrawLightBasedRimControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Light-Based Rim", () => {
                if(props.IsPropertyValid(props.rimColor)) materialEditor.ShaderProperty(props.rimColor, "Rim Color");
                if(props.IsPropertyValid(props.rimPower)) materialEditor.ShaderProperty(props.rimPower, "Rim Power");
                if(props.IsPropertyValid(props.rimIntensity)) materialEditor.ShaderProperty(props.rimIntensity, "Intensity");
                if(props.IsPropertyValid(props.rimLightInfluence)) materialEditor.ShaderProperty(props.rimLightInfluence, "Light Influence");
            }, true);
        }

        private void DrawTexturedRimControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Textured Rim", () => {
                if(props.IsPropertyValid(props.rimColor)) materialEditor.ShaderProperty(props.rimColor, "Rim Tint");
                if(props.IsPropertyValid(props.rimTexture)) materialEditor.TexturePropertySingleLine(new GUIContent("Rim Texture"), props.rimTexture);
                if(props.IsPropertyValid(props.rimScrollSpeed)) materialEditor.ShaderProperty(props.rimScrollSpeed, "Scroll Speed");
                if(props.IsPropertyValid(props.rimIntensity)) materialEditor.ShaderProperty(props.rimIntensity, "Intensity");
            }, true);
        }

        private void DrawFresnelEnhancedRimControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Fresnel Enhanced Rim", () => {
                if (props.IsPropertyValid(props.rimColor)) materialEditor.ShaderProperty(props.rimColor, "Rim Color");
                if (props.IsPropertyValid(props.rimIntensity)) materialEditor.ShaderProperty(props.rimIntensity, "Intensity");
                if (props.IsPropertyValid(props.fresnelPower)) materialEditor.ShaderProperty(props.fresnelPower, "Fresnel Power");
                if (props.IsPropertyValid(props.fresnelBias)) materialEditor.ShaderProperty(props.fresnelBias, "Fresnel Bias");
            }, true);
        }
        
        private void DrawColorGradientRimControls(MaterialEditor materialEditor)
        {
            ToonShaderStyles.DrawPropertyGroup("Color Gradient Rim", () => {
                if (props.IsPropertyValid(props.rimColorTop)) materialEditor.ShaderProperty(props.rimColorTop, "Top Color");
                if (props.IsPropertyValid(props.rimColorBottom)) materialEditor.ShaderProperty(props.rimColorBottom, "Bottom Color");
                if (props.IsPropertyValid(props.rimIntensity)) materialEditor.ShaderProperty(props.rimIntensity, "Intensity");
                if (props.IsPropertyValid(props.rimGradientPower)) materialEditor.ShaderProperty(props.rimGradientPower, "Gradient Power");
            }, true);
        }

        #endregion
        
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
    }
}

