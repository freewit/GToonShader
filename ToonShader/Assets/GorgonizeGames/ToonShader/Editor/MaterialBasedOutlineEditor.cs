using UnityEditor;
using UnityEngine;

namespace Gorgonize.ToonShader.Editor
{
    /// <summary>
    /// A professionally designed Unity Editor window for creating and managing material-based outline effects.
    /// </summary>
    public class MaterialBasedOutlineEditor : EditorWindow
    {
        // Constants
        private const string GuideURL = "https://www.gorgonize.com/docs/gtoon-shader/outline-editor-guide";

        // GUI Variables
        private Vector2 mainScrollPosition;
        
        [MenuItem("Gorgonize Game Tools/Material Based Outline Editor")]
        public static void ShowWindow()
        {
            GetWindow<MaterialBasedOutlineEditor>("Outline Editor").minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            // Initialization logic will go here
        }

        private void OnGUI()
        {
            // Initialize professional styles
            ToonShaderStyles.Initialize();

            // Header
            GUILayout.Label("Material Outline Editor", ToonShaderStyles.HeaderStyle);
            ToonShaderStyles.DrawAccentSeparator();

            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition, GUILayout.ExpandHeight(true));
            
            // Main content will be drawn here
            DrawEditorContent();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawEditorContent()
        {
            EditorGUILayout.BeginVertical();

            // This is where we will add all the controls and logic
            ToonShaderStyles.DrawPropertyGroup("Outline Settings", () =>
            {
                // Placeholder for future controls
                EditorGUILayout.LabelField("Outline controls will be here.", EditorStyles.centeredGreyMiniLabel);
            });
            
            // Help Section
            ToonShaderStyles.DrawPropertyGroup("Help & Tips", () =>
            {
                ToonShaderStyles.DrawInfoBox("This tool will help you manage advanced outline properties directly on materials or models.");
                if (GUILayout.Button("Guide", ToonShaderStyles.ButtonSecondaryStyle))
                {
                    Application.OpenURL(GuideURL);
                }
            });

            EditorGUILayout.EndVertical();
        }
    }
}
