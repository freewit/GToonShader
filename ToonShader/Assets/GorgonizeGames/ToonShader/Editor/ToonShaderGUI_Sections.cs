using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// Toon Shader arayüzünün her bir bölümünü çizen statik metotları içerir.
    /// </summary>
    public static class ToonShaderSections
    {
        // 'showLighting' değişkeni 'showShadows' olarak güncellendi.
        private static bool showShadows = true;
        private static bool showHighlights = true;
        private static bool showRim = true;
        private static bool showAdvanced = false;
        private static bool showSubsurface = true;
        private static bool showOutline = false;
        private static bool showWind = false;
        private static bool showPerformance = false;

        public static void DrawHeader()
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);

            // Başlığı ve logoyu ortalamak için yatay bir düzen kullan
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Logoyu ölçeklendirerek çiz
            if (ToonShaderStyles.logoTexture != null)
            {
                // Başlık font boyutuna göre dinamik bir logo boyutu belirle
                // Bu sayede logo her zaman metinle orantılı görünür.
                float logoSize = ToonShaderStyles.headerStyle.fontSize * 1.8f; 
                GUILayout.Label(ToonShaderStyles.logoTexture, GUILayout.Width(logoSize), GUILayout.Height(logoSize));
            }
            
            // Başlık metnini çiz
            GUILayout.Label(" Gorgonize Toon Shader", ToonShaderStyles.headerStyle);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            // Alt başlık
            EditorGUILayout.LabelField("Advanced Shadow Acne & Z-Fighting Solution v2.0", ToonShaderStyles.versionStyle);
            EditorGUILayout.EndVertical();

            // Logo bulunamazsa bir uyarı göster
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
        
        // 'DrawLightingSection' metodu, 'DrawShadowSection' olarak yeniden adlandırıldı ve yeniden düzenlendi.
        public static void DrawShadowSection(MaterialEditor editor, ToonShaderProperties props)
        {
            EditorGUILayout.BeginVertical(ToonShaderStyles.sectionStyle);
            showShadows = EditorGUILayout.Foldout(showShadows, "👻 Shadow System", ToonShaderStyles.foldoutStyle);
            if (showShadows)
            {
                // --- Grup 1: Gölgelendirme Modu ---
                EditorGUILayout.LabelField("Toon Shading Mode", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                int mode = (int)props.lightingMode.floatValue;
                
                // Dropdown yerine butonlu bir toolbar kullanılarak arayüz modernize edildi.
                mode = GUILayout.Toolbar(mode, new[] { "Stepped", "Smooth", "Ramp" });
                
                if (EditorGUI.EndChangeCheck())
                {
                    props.lightingMode.floatValue = mode;
                    ToonShaderKeywords.SetLightingKeywords(editor.target as Material, mode);
                }
                EditorGUILayout.Space();

                // --- Grup 2: Gölge Şekli (Shadow Shape) ---
                EditorGUILayout.LabelField("Shadow Shape", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                if (mode < 2) // Stepped veya Smooth modu seçiliyse
                {
                    editor.RangeProperty(props.shadowSteps, "Steps");
                    editor.RangeProperty(props.shadowSmoothness, "Smoothness");
                }
                else // Ramp modu seçiliyse
                {
                    editor.TextureProperty(props.shadowRamp, "Ramp Texture", false);
                }
                editor.RangeProperty(props.shadowOffset, "Offset");
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                // --- Grup 3: Gölge Görünümü (Shadow Appearance) ---
                // Bu grup, isteğin üzerine sadece Stepped ve Smooth modlarında gösterilecek şekilde güncellendi.
                if (mode < 2)
                {
                    EditorGUILayout.LabelField("Shadow Appearance", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    editor.ColorProperty(props.shadowColor, "Color");
                    editor.RangeProperty(props.shadowIntensity, "Intensity");
                    
                    // Yeni Checkbox eklendi
                    EditorGUI.BeginChangeCheck();
                    bool tintOnBase = EditorGUILayout.Toggle("Tint On Full Object", props.tintShadowOnBase.floatValue > 0.5f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        props.tintShadowOnBase.floatValue = tintOnBase ? 1f : 0f;
                        ToonShaderKeywords.SetKeyword(editor.target as Material, "_TINT_SHADOW_ON_BASE", tintOnBase);
                    }
                    
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                // Occlusion Strength, tüm modları etkilediği için ayrı olarak gösteriliyor.
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

