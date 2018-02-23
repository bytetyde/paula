using Assets.Scripts;
using Assets.Scripts.Craft;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(BackgroundPrefabGenerator))]
    public class BackgroundPrefabGeneratorEditor : UnityEditor.Editor
    {
        private BackgroundPrefabGenerator _generateBackgroundPrefabs;

        void OnEnable()
        {
            _generateBackgroundPrefabs = target as BackgroundPrefabGenerator;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                _generateBackgroundPrefabs.Generate();
            }
        }
    }
}
