using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Infinite;
using Assets.Scripts.Level_Generator.Populator;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof (InfiniteLevelSettings))]
    public class InfiniteLevelSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _activeSections;
        private SerializedProperty _generateNegativeDirection;
        private SerializedProperty _generatePositiveDirection;
        private SerializedProperty _getBackToStart;
        private SerializedProperty _infinite;
        private SerializedProperty _isVertical;
        private SerializedProperty _minimumClearance;
        private SerializedProperty _player;
        private SerializedProperty _startSeed;
        private SerializedProperty _startValue;

        private InfiniteLevelSettings _infiniteLevel;
        private ReorderableList _sectionStringList;
        private StringParser _stringParser;

        private void OnEnable()
        {
            _infiniteLevel = (InfiniteLevelSettings) target;

            _generatePositiveDirection = serializedObject.FindProperty("GeneratePositiveDirection");
            _generateNegativeDirection = serializedObject.FindProperty("GenerateNegativeDirection");
            _getBackToStart = serializedObject.FindProperty("GetBackToStart");
            _activeSections = serializedObject.FindProperty("ActiveSections");
            _infinite = serializedObject.FindProperty("Infinite");
            _isVertical = serializedObject.FindProperty("IsVertical");
            _minimumClearance = serializedObject.FindProperty("MinimumClearance");
            _player = serializedObject.FindProperty("Player");
            _startSeed = serializedObject.FindProperty("StartSeed");
            _startValue = serializedObject.FindProperty("StartValue");

            _sectionStringList = SetupSectionStringList(_infiniteLevel.SectionStringList);
            _stringParser = _infiniteLevel.gameObject.GetComponent<StringParser>();
        }

        private void OnDisable()
        {
            RemoveCallbacksFromSectionList(_sectionStringList);
        }

        public void RemoveCallbacksFromSectionList(ReorderableList reorderableList)
        {
            reorderableList.drawHeaderCallback -= DrawSectionListHeader;
            reorderableList.drawElementCallback -= DrawSectionListElement;

            reorderableList.onAddCallback -= AddSectionItem;
            reorderableList.onRemoveCallback -= RemoveItem;
        }

        public ReorderableList SetupSectionStringList(List<IntStringPair> list)
        {
            var reorderableList = new ReorderableList(list, typeof(IntStringPair), true, true, true, true);

            reorderableList.drawHeaderCallback += DrawSectionListHeader;
            reorderableList.drawElementCallback += DrawSectionListElement;
            reorderableList.onAddCallback += AddSectionItem;
            reorderableList.onRemoveCallback += RemoveItem;

            return reorderableList;
        }

        private void DrawSectionListHeader(Rect rect)
        {
            GUI.Label(rect, "Level Sections GE Startstrings");
        }

        private void DrawSectionListElement(Rect rect, int index, bool active, bool focused)
        {
            if (_infiniteLevel.SectionStringList.ElementAtOrDefault(index) == null)
            {
                return;
            }

            var style = new GUIStyle();
            style.fontSize = 8;
            style.normal.textColor = new Color(0, 0, 0, 0.5f);

            var item = _infiniteLevel.SectionStringList[index];

            EditorGUI.BeginChangeCheck();
            item.Key = EditorGUI.IntField(new Rect(rect.x + 18, rect.y, 0 + rect.width / 10, rect.height ), item.Key);
            item.Value = EditorGUI.TextField(new Rect(rect.x + 18 + rect.width * 0.1f, rect.y, rect.width * 0.9f - 65, rect.height ), item.Value);
            EditorGUI.DrawPreviewTexture(new Rect(rect.width - 15, rect.y + 1, 48, rect.height  - 3), EditorHelper.EntryValidation(item.Value, false, false, _stringParser.InputList), null, ScaleMode.StretchToFill);
            if (GUI.Button(new Rect(rect.width - 5f, rect.y + 2, 35f, rect.height  - 5), "Del"))
            {
                _infiniteLevel.SectionStringList.Remove(item);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void AddSectionItem(ReorderableList reordableList)
        {
            reordableList.list.Add(new IntStringPair());
        
            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList reordableList)
        {
            reordableList.list.RemoveAt(reordableList.index);

            EditorUtility.SetDirty(target);
        }


        private void DrawAssetCountList(float width)
        {
            var style = new GUIStyle();
            style.fontSize = 8;
            style.normal.textColor = new Color(0, 0, 0, 0.5f);

            GUILayout.BeginVertical();

            for (int index = 0; index < _infiniteLevel.AssetCountList.Count; index++)
            {    
                var item = _infiniteLevel.AssetCountList[index];

                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                item.Key = EditorGUILayout.IntField(item.Key, GUILayout.Width(width / 5));
                var counter = 1;
                if (GUILayout.Button("Clear"))
                {
                    item.Value.Clear();
                }
                if (GUILayout.Button("Del"))
                {
                    _infiniteLevel.AssetCountList.Remove(item);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical();
                foreach (KeyValueCurveGroup curveGroup in item.Value)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(width / 5);
                    EditorGUILayout.LabelField(curveGroup.Key);
                    curveGroup.TargetObjectCount = EditorGUILayout.IntField(curveGroup.TargetObjectCount, GUILayout.Width(width / 5));
                    GUILayout.EndHorizontal();
                    counter++;
                }
                GUILayout.EndVertical();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }
                EditorHelper.DrawSeparator();
            }
            GUILayout.EndVertical();
        }

        private void AddAssetCountItem(List<IntListStringCurveIntPair> list)
        {
            list.Add(new IntListStringCurveIntPair());
            _infiniteLevel.UpdateAssetCountPopulationLists();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_infinite, new GUIContent("Generate Infinite Level"));

            if (_infinite.boolValue)
            {
                DrawSeparator();
                DrawSettingsPanel();
                DrawLists();
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load"))
            {
                _infiniteLevel.ReadFromJsonFile();
            }
            if (GUILayout.Button("Save"))
            {
                _infiniteLevel.SafeToJsonFile();
            }
            GUILayout.EndHorizontal();
        
            DrawSeparator();
            serializedObject.ApplyModifiedProperties();
        }

        public void DrawSettingsPanel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_isVertical, new GUIContent("Vertical Construction"));
            var positiveText = _isVertical.boolValue ? "Up" : "Right";
            var negativeText = _isVertical.boolValue ? "Down" : "Left";

            EditorGUILayout.PropertyField(_generatePositiveDirection,
                new GUIContent("Generate " + positiveText + " Direction"));
            EditorGUILayout.PropertyField(_generateNegativeDirection,
                new GUIContent("Generate " + negativeText + " Direction"));
            EditorGUILayout.PropertyField(_getBackToStart, new GUIContent("Generate Back to Start"));
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_activeSections, new GUIContent("Concurrent Sections"));
            EditorGUILayout.PropertyField(_minimumClearance, new GUIContent("Clearance Value"));
            EditorGUILayout.PropertyField(_player, new GUIContent("Player Object"));
            EditorGUILayout.PropertyField(_startSeed, new GUIContent("Start Seed"));
            EditorGUILayout.PropertyField(_startValue, new GUIContent("Start Section Number"));
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        public void DrawLists()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorHelper.DrawHelpBox("Note: This settings will partially override the settings in the Level Grammar and Levelpopulator. Set your startstring and populations counts here");
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            var width = EditorGUIUtility.currentViewWidth - 44;
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUILayout.Width(width / 2));
            _sectionStringList.DoLayoutList();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(width / 2));
            GUILayout.Label("Section Population Settings", EditorStyles.boldLabel);
            DrawAssetCountList(width / 2);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add"))
            {
                _infiniteLevel.AssetCountList.Add(new IntListStringCurveIntPair());
                OnEnable();
            }
            if (GUILayout.Button("Update Populationlist"))
            {
                _infiniteLevel.UpdateAssetCountPopulationLists();
                OnEnable();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }
        public void DrawSeparator()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }
    }
}