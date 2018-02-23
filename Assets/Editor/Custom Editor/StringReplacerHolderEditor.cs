using Assets.Scripts.Level_Generator.Gramatical_Textreplacer;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof(StringReplacerHolder))]
    public class StringReplacerHolderEditor : UnityEditor.Editor
    {

        private StringReplacerHolder _stringReplacerHolder;

        private void OnEnable()
        {
            _stringReplacerHolder = (StringReplacerHolder) target;
        }

        private void OnDisable()
        {
        
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();
            DrawDefaultInspector();
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Start Text Replacing"))
            {
                _stringReplacerHolder.StartTextReplacing();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
