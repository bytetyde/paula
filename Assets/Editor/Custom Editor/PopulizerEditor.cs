using Assets.Scripts.Level_Generator.Populator;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(Populizer))]
    public class PopulizerEditor : UnityEditor.Editor
    {
        private Populizer _populizer;
        private SerializedProperty _codonNumbers;
        private SerializedProperty _startString;

        void OnEnable()
        {
            _populizer = (Populizer) target;
            _codonNumbers = serializedObject.FindProperty("CodonNumbers");
            _startString = serializedObject.FindProperty("StringList");
        }

        void OnDisable()
        {
        
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Repopulate"))
            {
                _populizer.Repopulate();
            }
        }

        public void OnSceneGUI()
        {
            serializedObject.Update();

            Handles.BeginGUI();
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_startString, new GUIContent("Start String"), true, GUILayout.ExpandWidth(true));
            EditorGUILayout.PropertyField(_codonNumbers, new GUIContent("Codons"), true, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("Used Codon Numbers", _populizer.UsedCodonNumbers);
            if (GUILayout.Button("Repopulate"))
            {
                _populizer.Repopulate();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            Handles.EndGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
