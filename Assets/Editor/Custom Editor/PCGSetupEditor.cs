using Assets.Scripts;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Setup;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(PCGSetup))]
    public class PCGSetupEditor : UnityEditor.Editor
    {

        private SerializedProperty _parentNameProperty;
        private SerializedProperty _twoDimensionalProperty;
        private SerializedProperty _infiniteGenerationProperty;

        private PCGSetup _pcgSetup;

        void OnEnable()
        {
            _parentNameProperty = serializedObject.FindProperty("ParentGameobjectName");
            _twoDimensionalProperty = serializedObject.FindProperty("TwoDimensional");
            _pcgSetup = (PCGSetup) target;
        }

        void OnDisable()
        {
        
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _pcgSetup.GetComponent<GeneratorData>().ParentName = _parentNameProperty.stringValue;
            _pcgSetup.GetComponent<GeneratorData>().TwoDimensional = _twoDimensionalProperty.boolValue;

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_parentNameProperty, new GUIContent("PCG Parent Name"));
            EditorGUILayout.PropertyField(_twoDimensionalProperty, new GUIContent("2D Colliders"));
            if (GUILayout.Button("Setup PCG"))
            {
                Scripts.Common.TagsAndLayers.AddLayer("pcgplatform");
                Scripts.Common.TagsAndLayers.AddLayer("tempRaycastLayer");
                Scripts.Common.TagsAndLayers.AddTag("pcgplatform");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
