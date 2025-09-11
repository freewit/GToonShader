using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Toon Shader arayüzünün her bir bölümünü çizen statik metotları içerir.
    /// </summary>
    public static class ToonShaderSections
    {
        private static bool showLighting = true;
        private static bool showAdvancedShadowBias = false;
        private static bool showHighlights = false;
        private static bool showRim = false;
        private static bool showAdvanced = false;
        private static bool showSubsurface = false;
        private static bool showOutline = false;
        private static bool showWind = false;
        private static bool showPerformance = false;

        public static void DrawHeader()
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            EditorGUILayout.LabelField("⚡ Ultimate Toon Shader - Shadow Fix", ToonShaderStyles.headerStyle);
            EditorGUILayout.LabelField("Advanced Shadow Acne & Z-Fighting Solution v2.0", ToonShaderStyles.versionStyle);
            EditorGUILayout.EndVertical();
        }

        public static void DrawBaseProperties(MaterialEditor editor, MaterialProperty color, MaterialProperty map)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            EditorGUILayout.LabelField("🎨 Base Properties", EditorStyles.boldLabel);
            editor.ColorProperty(color, "Base Color");
            editor.TextureProperty(map, "Base Texture");
            EditorGUILayout.EndVertical();
        }
        
        public static void DrawLightingSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showLighting = EditorGUILayout.Foldout(showLighting, "🌟 Lighting System", ToonShaderStyles.foldoutStyle);
            if (showLighting)
            {
                EditorGUI.BeginChangeCheck();
                int mode = (int)props.lightingMode.floatValue;
                mode = EditorGUILayout.Popup("Lighting Mode", mode, new[] { "Stepped", "Smooth", "Ramp" });
                if (EditorGUI.EndChangeCheck())
                {
                    props.lightingMode.floatValue = mode;
                    ToonShaderKeywords.SetLightingKeywords(editor.target as Material, mode);
                }

                if (mode < 2)
                {
                    editor.RangeProperty(props.shadowSteps, "Shadow Steps");
                    editor.RangeProperty(props.shadowSmoothness, "Shadow Smoothness");
                }
                else
                {
                    editor.TextureProperty(props.shadowRamp, "Shadow Ramp", false);
                }
                editor.ColorProperty(props.shadowColor, "Shadow Color");
                editor.RangeProperty(props.shadowIntensity, "Shadow Intensity");
                editor.RangeProperty(props.shadowOffset, "Shadow Offset");
                editor.RangeProperty(props.occlusionStrength, "Occlusion Strength");
            }
            EditorGUILayout.EndVertical();
        }
        
        public static void DrawAdvancedShadowBiasSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showAdvancedShadowBias = EditorGUILayout.Foldout(showAdvancedShadowBias, "🛠️ Advanced Shadow Fix (Anti Z-Fighting)", ToonShaderStyles.foldoutStyle);
            if (showAdvancedShadowBias)
            {
                EditorGUILayout.HelpBox("Bu gelişmiş ayarlar Z-fighting ve shadow acne problemlerini çözer. Değerleri kademeli olarak artırın.", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                bool adaptive = EditorGUILayout.Toggle(new GUIContent("Use Adaptive Bias", "Yüzey açısına göre otomatik bias ayarı"), props.useAdaptiveBias.floatValue > 0.5f);
                bool pancaking = EditorGUILayout.Toggle(new GUIContent("Use Shadow Pancaking", "Arka yüzlerdeki shadow acne'yi önler"), props.usePancaking.floatValue > 0.5f);
                if (EditorGUI.EndChangeCheck())
                {
                    props.useAdaptiveBias.floatValue = adaptive ? 1f : 0f;
                    props.usePancaking.floatValue = pancaking ? 1f : 0f;
                    ToonShaderKeywords.SetAdvancedShadowKeywords(editor.target as Material, adaptive, pancaking);
                }

                EditorGUILayout.Space(5);
                editor.RangeProperty(props.shadowDepthBias, "Shadow Depth Bias");
                EditorGUILayout.LabelField("• Düz yüzeylerdeki shadow acne'yi düzeltir", EditorStyles.miniLabel);
                editor.RangeProperty(props.shadowNormalBias, "Shadow Normal Bias");
                EditorGUILayout.LabelField("• Kavisli yüzeylerdeki shadow acne'yi düzeltir", EditorStyles.miniLabel);
                editor.RangeProperty(props.shadowSlopeBias, "Shadow Slope Bias");
                EditorGUILayout.LabelField("• Eğimli yüzeylerdeki problemleri çözer", EditorStyles.miniLabel);
                editor.RangeProperty(props.shadowDistanceFade, "Shadow Distance Fade");
                EditorGUILayout.LabelField("• Uzak nesnelerde gölgeleri yumuşatır", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
        }
        
        public static void DrawHighlightsSection(MaterialEditor editor, ToonShaderProperties props)
        {
             EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
             showHighlights = EditorGUILayout.Foldout(showHighlights, "✨ Highlight System", ToonShaderStyles.foldoutStyle);
             if(showHighlights)
             {
                editor.ColorProperty(props.specularColor, "Specular Color");
                editor.RangeProperty(props.specularSize, "Specular Size");
                editor.RangeProperty(props.specularSmoothness, "Specular Smoothness");
                editor.RangeProperty(props.specularSteps, "Specular Steps");
             }
             EditorGUILayout.EndVertical();
        }
        
        public static void DrawRimSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showRim = EditorGUILayout.Foldout(showRim, "🌅 Rim Lighting", ToonShaderStyles.foldoutStyle);
            if(showRim)
            {
                editor.ColorProperty(props.rimColor, "Rim Color");
                editor.RangeProperty(props.rimPower, "Rim Power");
                editor.RangeProperty(props.rimIntensity, "Rim Intensity");
                editor.RangeProperty(props.rimOffset, "Rim Offset");
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
                 editor.ColorProperty(props.subsurfaceColor, "Subsurface Color");
                 editor.RangeProperty(props.subsurfaceIntensity, "Intensity");
                 editor.RangeProperty(props.subsurfaceDistortion, "Distortion");
                 editor.RangeProperty(props.subsurfacePower, "Power");
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
            EditorGUILayout.LabelField("• Shadow Acne: 'Standard Fix' preset'ini deneyin.", tipStyle);
            EditorGUILayout.LabelField("• Z-Fighting: 'Aggressive Fix' preset'ini kullanın ve Adaptive Bias'ı aktif edin.", tipStyle);
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

