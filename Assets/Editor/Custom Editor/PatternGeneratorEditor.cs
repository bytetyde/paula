using Assets.Scripts;
using Assets.Scripts.Craft;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(PatternGenerator))]
    public class PatternGeneratorEditor : UnityEditor.Editor
    {
        private PatternGenerator _patternGenerator;

        void OnEnable()
        {
            _patternGenerator = target as PatternGenerator;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Create Prefabs from Blueprints"))
            {
                _patternGenerator.CreatePatternPrefabs();
            }

            if (GUILayout.Button("Create Empty Template"))
            {
                _patternGenerator.CreateTemplate();
            }
        }
    }
}
