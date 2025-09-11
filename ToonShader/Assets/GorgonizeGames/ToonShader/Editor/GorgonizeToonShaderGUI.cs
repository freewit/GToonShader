using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    public class AdvancedToonShaderGUI : ShaderGUI
    {
        private ToonShaderProperties props;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Özellikleri ve stilleri yükle
            props = new ToonShaderProperties(properties);
            ToonShaderStyles.Initialize();
            
            // Arayüzü çiz
            ToonShaderSections.DrawHeader();
            EditorGUILayout.Space(5);

            ToonShaderSections.DrawBaseProperties(materialEditor, props.baseColor, props.baseMap);
            EditorGUILayout.Space(3);
            
            ToonShaderSections.DrawLightingSection(materialEditor, props);
            EditorGUILayout.Space(3);

            ToonShaderSections.DrawAdvancedShadowBiasSection(materialEditor, props);
            EditorGUILayout.Space(3);
            
            ToonShaderSections.DrawHighlightsSection(materialEditor, props);
            EditorGUILayout.Space(3);

            ToonShaderSections.DrawRimSection(materialEditor, props);
            EditorGUILayout.Space(3);

            ToonShaderSections.DrawAdvancedSection(materialEditor, props);
            EditorGUILayout.Space(3);
            
            ToonShaderSections.DrawSubsurfaceSection(materialEditor, props);
            EditorGUILayout.Space(3);

            ToonShaderSections.DrawOutlineSection(materialEditor, props);
            EditorGUILayout.Space(3);

            ToonShaderSections.DrawWindSection(materialEditor, props);
            EditorGUILayout.Space(3);

            ToonShaderSections.DrawPerformanceSection(materialEditor, props);
            EditorGUILayout.Space(10);
            
            ToonShaderSections.DrawTroubleshootingTips();
            EditorGUILayout.Space(5);
            
            ToonShaderSections.DrawFooter();
        }
    }
}

