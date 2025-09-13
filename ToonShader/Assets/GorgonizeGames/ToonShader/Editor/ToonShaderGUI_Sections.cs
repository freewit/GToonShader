using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Toon Shader arayüzünün her bir bölümünü çizen statik metotları içerir.
    /// </summary>
    public static class ToonShaderSections
    {
        private static bool showShadows = true;
        private static bool showHighlights = true;
        private static bool showRim = true;
        private static bool showAdvanced = false;
        private static bool showSubsurface = false;
        private static bool showOutline = false;
        private static bool showWind = false;
        private static bool showPerformance = false;

        public static void DrawHeader()
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (ToonShaderStyles.logoTexture != null)
            {
                float logoSize = ToonShaderStyles.headerStyle.fontSize * 1.8f; 
                GUILayout.Label(ToonShaderStyles.logoTexture, GUILayout.Width(logoSize), GUILayout.Height(logoSize));
            }
            
            GUILayout.Label(" Gorgonize Toon Shader", ToonShaderStyles.headerStyle);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("Advanced Shadow Acne & Z-Fighting Solution v2.0", ToonShaderStyles.versionStyle);
            EditorGUILayout.EndVertical();

            if (ToonShaderStyles.logoTexture == null)
            {
                EditorGUILayout.HelpBox("Logo bulunamadı! Lütfen 'GorgonizeLogo.png' dosyanızı 'Assets/.../Editor/Resources' klasörüne ekleyin.", MessageType.Warning);
            }
        }

        public static void DrawBaseProperties(MaterialEditor editor, MaterialProperty color, MaterialProperty map)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            EditorGUILayout.LabelField("🎨 Base Properties", EditorStyles.boldLabel);
            editor.ColorProperty(color, "Base Color");
            editor.TextureProperty(map, "Base Texture");
            EditorGUILayout.EndVertical();
        }

        public static void DrawShadowSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showShadows = EditorGUILayout.Foldout(showShadows, "🌑 Shadow System", ToonShaderStyles.foldoutStyle);
            if(showShadows)
            {
                // Shadow mode selection
                EditorGUI.BeginChangeCheck();
                int lightingMode = (int)props.lightingMode.floatValue;
                lightingMode = EditorGUILayout.Popup("Lighting Mode", lightingMode, new string[] {"Stepped", "Smooth", "Ramp"});
                if (EditorGUI.EndChangeCheck())
                {
                    props.lightingMode.floatValue = lightingMode;
                    ToonShaderKeywords.SetLightingKeywords(editor.target as Material, lightingMode);
                }

                EditorGUI.indentLevel++;
                
                // Common shadow properties
                editor.ColorProperty(props.shadowColor, "Shadow Color");
                editor.RangeProperty(props.shadowIntensity, "Shadow Intensity");
                editor.RangeProperty(props.shadowOffset, "Shadow Offset");
                
                // Mode specific properties
                if (lightingMode == 0) // Stepped
                {
                    editor.RangeProperty(props.shadowSteps, "Shadow Steps");
                }
                else if (lightingMode == 1) // Smooth  
                {
                    editor.RangeProperty(props.shadowSmoothness, "Shadow Smoothness");
                }
                else if (lightingMode == 2) // Ramp
                {
                    editor.TextureProperty(props.shadowRamp, "Shadow Ramp");
                }
                
                // Tint shadow on base toggle
                EditorGUI.BeginChangeCheck();
                bool tintOnBase = EditorGUILayout.Toggle("Tint On Full Object", props.tintShadowOnBase.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.tintShadowOnBase.floatValue = tintOnBase ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_TINT_SHADOW_ON_BASE", tintOnBase);
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                // Occlusion Strength
                editor.RangeProperty(props.occlusionStrength, "Occlusion Strength");
            }
            EditorGUILayout.EndVertical();
        }
        
        public static void DrawHighlightsSection(MaterialEditor editor, ToonShaderProperties props)
        {
             EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
             showHighlights = EditorGUILayout.Foldout(showHighlights, "✨ Highlight System", ToonShaderStyles.foldoutStyle);
             if(showHighlights)
             {
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle("Enable Highlights", props.enableHighlights.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.enableHighlights.floatValue = enabled ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_ENABLEHIGHLIGHTS_ON", enabled);
                }

                if(enabled)
                {
                    EditorGUI.indentLevel++;
                    editor.ColorProperty(props.specularColor, "Specular Color");
                    editor.RangeProperty(props.specularSize, "Specular Size");
                    editor.RangeProperty(props.specularSmoothness, "Specular Smoothness");
                    editor.RangeProperty(props.specularSteps, "Specular Steps");
                    EditorGUI.indentLevel--;
                }
             }
             EditorGUILayout.EndVertical();
        }
        
        public static void DrawRimSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showRim = EditorGUILayout.Foldout(showRim, "🌅 Rim Lighting", ToonShaderStyles.foldoutStyle);
            if(showRim)
            {
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle("Enable Rim Lighting", props.enableRim.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.enableRim.floatValue = enabled ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_ENABLERIM_ON", enabled);
                }

                if(enabled)
                {
                    EditorGUI.indentLevel++;
                    editor.ColorProperty(props.rimColor, "Rim Color");
                    editor.RangeProperty(props.rimPower, "Rim Power");
                    editor.RangeProperty(props.rimIntensity, "Rim Intensity");
                    editor.RangeProperty(props.rimOffset, "Rim Offset");
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawAdvancedSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "🔧 Advanced Features", ToonShaderStyles.foldoutStyle);
            if(showAdvanced)
            {
                editor.TextureProperty(props.normalMap, "Normal Map");
                if (props.normalMap.textureValue != null) editor.RangeProperty(props.normalStrength, "Normal Strength");
                EditorGUILayout.Space();
                editor.TextureProperty(props.emissionMap, "Emission Map");
                editor.ColorProperty(props.emissionColor, "Emission Color");
                editor.RangeProperty(props.emissionIntensity, "Emission Intensity");
                EditorGUILayout.Space();
                editor.TextureProperty(props.detailMap, "Detail Albedo");
                editor.TextureProperty(props.detailNormalMap, "Detail Normal");
                editor.RangeProperty(props.detailStrength, "Detail Strength");
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawSubsurfaceSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showSubsurface = EditorGUILayout.Foldout(showSubsurface, "🔴 Subsurface Scattering", ToonShaderStyles.foldoutStyle);
            if(showSubsurface)
            {
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle("Enable Subsurface", props.enableSubsurface.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.enableSubsurface.floatValue = enabled ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_ENABLESUBSURFACE_ON", enabled);
                }

                if(enabled)
                {
                    EditorGUI.indentLevel++;
                    editor.ColorProperty(props.subsurfaceColor, "Subsurface Color");
                    editor.RangeProperty(props.subsurfaceIntensity, "Intensity");
                    editor.RangeProperty(props.subsurfaceDistortion, "Distortion");
                    editor.RangeProperty(props.subsurfacePower, "Power");
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawOutlineSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showOutline = EditorGUILayout.Foldout(showOutline, "⭕ Outline", ToonShaderStyles.foldoutStyle);
            if(showOutline)
            {
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle("Enable Outline", props.enableOutline.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.enableOutline.floatValue = enabled ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_ENABLEOUTLINE_ON", enabled);
                }
                if(enabled)
                {
                    editor.ColorProperty(props.outlineColor, "Outline Color");
                    editor.RangeProperty(props.outlineWidth, "Outline Width");
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawWindSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showWind = EditorGUILayout.Foldout(showWind, "🌪️ Wind Animation", ToonShaderStyles.foldoutStyle);
            if(showWind)
            {
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle("Enable Wind", props.enableWind.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.enableWind.floatValue = enabled ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_ENABLEWIND_ON", enabled);
                }
                if(enabled)
                {
                    editor.RangeProperty(props.windSpeed, "Wind Speed");
                    editor.RangeProperty(props.windStrength, "Wind Strength");
                    editor.VectorProperty(props.windDirection, "Wind Direction");
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawPerformanceSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showPerformance = EditorGUILayout.Foldout(showPerformance, "⚡ Performance", ToonShaderStyles.foldoutStyle);
            if(showPerformance)
            {
                EditorGUI.BeginChangeCheck();
                bool shadows = EditorGUILayout.Toggle("Receive Shadows", props.receiveShadows.floatValue > 0.5f);
                bool additionalLights = EditorGUILayout.Toggle("Additional Lights", props.enableAdditionalLights.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.receiveShadows.floatValue = shadows ? 1f : 0f;
                    props.enableAdditionalLights.floatValue = additionalLights ? 1f : 0f;
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_RECEIVESHADOWS_ON", shadows);
                    ToonShaderKeywords.SetKeyword(editor.target as Material, "_ENABLEADDITIONALLIGHTS_ON", additionalLights);
                }
                editor.RangeProperty(props.lightmapInfluence, "Lightmap Influence");
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawTroubleshootingTips()
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            EditorGUILayout.LabelField("🔧 Sorun Giderme Rehberi", EditorStyles.boldLabel);
            GUIStyle tipStyle = new GUIStyle(EditorStyles.label) { fontSize = 10, wordWrap = true, normal = { textColor = new Color(0.6f, 0.6f, 0.6f, 1f) } };
            EditorGUILayout.LabelField("• Shadow Acne: Directional Light Shadow Bias değerini düşürün.", tipStyle);
            EditorGUILayout.LabelField("• Z-Fighting: Shadow Distance değerini 50-100 arasında tutun.", tipStyle);
            EditorGUILayout.EndVertical();
        }

        public static void DrawFooter()
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            EditorGUILayout.LabelField("💡 Pro Tips:", EditorStyles.miniBoldLabel);
            GUIStyle tipStyle = new GUIStyle(EditorStyles.label) { fontSize = 10, wordWrap = true, normal = { textColor = new Color(0.6f, 0.6f, 0.6f, 1f) } };
            EditorGUILayout.LabelField("• URP Asset'inde Shadow Distance'ı 50-100 arası tutun.", tipStyle);
            EditorGUILayout.LabelField("• Light'ın Shadow Bias ayarlarını da kontrol edin.", tipStyle);
            EditorGUILayout.EndVertical();
        }
    }
}