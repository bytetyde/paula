using Assets.Scripts.Craft;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(MainGenerator))]
    public class MainGeneratorEditor : UnityEditor.Editor
    {
        private MainGenerator _mainGenerator;

        void OnEnable()
        {
            _mainGenerator = target as MainGenerator;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                _mainGenerator.Generate();
            }
        }
    }
}
