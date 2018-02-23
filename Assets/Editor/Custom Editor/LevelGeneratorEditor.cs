using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Stats;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : UnityEditor.Editor
    {
        private LevelGenerator LevelGenerator;

        private void OnEnable()
        {
            LevelGenerator = (LevelGenerator) target;
        }

        private void OnDisable()
        {
        
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal( GUILayout.ExpandWidth(true));
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorHelper.DrawHelpBox("<b>Duration: </b>" + StatsLocator.GetLevelGenerationLog().GenerationDuration + "ms");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Setup Layers and Tags"))
            {
                Scripts.Common.TagsAndLayers.AddLayer("pcgplatform");
                Scripts.Common.TagsAndLayers.AddTag("pcgplatform");
            }
            if (GUILayout.Button("Load Raw Level"))
            {
                LevelGenerator.LoadLevel();
            }
            if (GUILayout.Button("Save Raw Level"))
            {
                LevelGenerator.SaveLevel();
            }
            if (GUILayout.Button(LevelGenerator.LogGeneration ? "Log to File" : "No Logging"))
            {
                LevelGenerator.ChangeLog();
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate"))
            {
                LevelGenerator.Generate();
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

    }
}
