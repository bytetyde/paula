using System.Collections.Generic;
using System.Text;
using Assets.Scripts;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Instantiater;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof (LevelInstantiater))]
    public class LevelInstantiaterEditor : UnityEditor.Editor
    {
        private LevelInstantiater _levelInstantiater;
        private SerializedProperty _verticalProperty;
        private SerializedProperty _animationDuration;
        private SerializedProperty _animateCellsProperty;
        private SerializedProperty _animationOffsetProperty;
        private SerializedProperty _groundCellsProperty;
        private SerializedProperty _parentPositionProperty;
        private SerializedProperty _layerNameProperty;
        private SerializedProperty _excludeTagProperty;
        private SerializedProperty _minMaxVerticalProperty;
        private SerializedProperty _minMaxHorizontalProperty;

        private void OnEnable()
        {
            _levelInstantiater = (LevelInstantiater) target;
            _verticalProperty = serializedObject.FindProperty("VerticalConstruction");
            _animationDuration = serializedObject.FindProperty("AnimationDuration");
            _animateCellsProperty = serializedObject.FindProperty("AnimateCells");
            _animationOffsetProperty = serializedObject.FindProperty("AnimationOffset");
            _groundCellsProperty = serializedObject.FindProperty("GroundCells");
            _parentPositionProperty = serializedObject.FindProperty("ParentPosition");
            _layerNameProperty = serializedObject.FindProperty("LayerName");
            _excludeTagProperty = serializedObject.FindProperty("ExcludeTag");
            _minMaxVerticalProperty = serializedObject.FindProperty("MinMaxVertical");
            _minMaxHorizontalProperty = serializedObject.FindProperty("MinMaxHorizontal");
        }

        private void OnDisable()
        {
        }

        public string HighlightText(List<string> input)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < input.Count; i++)
            {
                var str = input[i];

                if (str.Contains("way-cell-group"))
                { 
                    sb.Append("<color=green><b>" + str +"</b></color>");
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (var c in str)
                    {
                        if (c == '<')
                        {
                            stringBuilder.Append("<color=purple><b><</b></color>");
                            stringBuilder.Append("<color=teal><b>");
                        }
                        else if (c == '>')
                        {
                            stringBuilder.Append("</b></color>");
                            stringBuilder.Append("<color=purple><b>></b></color>");
                        }
                        else
                        {
                            stringBuilder.Append(c);
                        }
                    }
                    sb.Append(stringBuilder);
                }
            }

            return sb.ToString();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorHelper.DrawHelpBox("<b>Generated Level</b>" + "\n" + HighlightText(_levelInstantiater.GeneratedLevel));
            EditorGUILayout.BeginHorizontal();
            EditorHelper.DrawHelpBox("<b>Duration: </b>" + _levelInstantiater.LevelGenerationDuration + "\n" +
                                     "<b>Used Codon Numbers </b>" + _levelInstantiater.UsedGenomes);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Level"))
            {
                _levelInstantiater.SaveLevel();
            }
            if (GUILayout.Button("Load Level"))
            {
                _levelInstantiater.LoadLevel();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10f);
            EditorHelper.DrawHeader("Instantiation settings");
            EditorHelper.DrawSeparator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_verticalProperty, new GUIContent("Vertical Construction"));
            EditorGUILayout.PropertyField(_groundCellsProperty, new GUIContent("Ground Cell Objects"));
            EditorGUILayout.PropertyField(_parentPositionProperty, new GUIContent("Parent Position"));
            EditorGUILayout.PropertyField(_minMaxVerticalProperty, new GUIContent("Min Max Vertical Offset"));
            EditorGUILayout.PropertyField(_minMaxHorizontalProperty, new GUIContent("Min Max Horizontal Offset"));

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_animateCellsProperty, new GUIContent("Animate Cells"));
            EditorGUILayout.PropertyField(_animationDuration, new GUIContent("Animation Duration (ms)"));
            EditorGUILayout.PropertyField(_animationOffsetProperty, new GUIContent("Animation Offset"));
            EditorGUILayout.PropertyField(_layerNameProperty, new GUIContent("Apply Layer Name"));
            EditorGUILayout.PropertyField(_excludeTagProperty, new GUIContent("Exclude Objects with Tag"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Instantiate Level"))
            {
                GameObjectHelper.CreateEmptyPCGParent(_levelInstantiater.GetComponent<GeneratorData>().ParentName);
                _levelInstantiater.SetupLevelObjects();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
       
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

        }
    }
}