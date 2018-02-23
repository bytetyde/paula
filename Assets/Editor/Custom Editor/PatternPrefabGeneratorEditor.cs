using Assets.Scripts;
using Assets.Scripts.Craft;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(PatternPrefabGenerator))]
    public class PatternPrefabGeneratorEditor : UnityEditor.Editor
    {
        private PatternPrefabGenerator _patternPrefabGenerator;

        void OnEnable()
        {
            _patternPrefabGenerator = target as PatternPrefabGenerator;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                _patternPrefabGenerator.Generate();
            }
        }
    }
}
