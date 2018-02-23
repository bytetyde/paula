using System.Linq;
using System.Text;
using Assets.Scripts;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Populator;
using Assets.Scripts.Level_Generator.Stats;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof (LevelPopulator))]
    public class LevelPopulatorEditor : UnityEditor.Editor
    {
        private LevelPopulator _levelPopulator;

        private ReorderableList reorderableList;
        private SerializedProperty _showStats;
        private SerializedProperty _populateLevel;
        private SerializedProperty _excludeLayerProperty;

        private void OnEnable()
        {
            _levelPopulator = (LevelPopulator) target;

            reorderableList = new ReorderableList(_levelPopulator.GrammarItems, typeof (KeyValueString), true, true, true, true);

            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.drawElementCallback += DrawElement;

            reorderableList.onAddCallback += AddItem;
            reorderableList.onRemoveCallback += RemoveItem;

            _showStats = serializedObject.FindProperty("ShowStats");
            _populateLevel = serializedObject.FindProperty("PopulateLevel");
            _excludeLayerProperty = serializedObject.FindProperty("ExcludeLayer");

        }

        private void OnDisable()
        {
            reorderableList.drawHeaderCallback -= DrawHeader;
            reorderableList.drawElementCallback -= DrawElement;

            reorderableList.onAddCallback -= AddItem;
            reorderableList.onRemoveCallback -= RemoveItem;
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Population Grammar");
        }

        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            if (_levelPopulator.GrammarItems.ElementAtOrDefault(index) == null)
            {
                return;
            }

            var item = _levelPopulator.GrammarItems[index];

            EditorGUI.BeginChangeCheck();
            item.Key = EditorGUI.TextField(new Rect(rect.x + 18, rect.y, 0 + rect.width/6, rect.height),
                item.Key);
            item.Value =
                EditorGUI.TextField(new Rect(rect.x + 18 + rect.width*0.16666f, rect.y, rect.width*0.833333f - 270, rect.height),
                    item.Value);
            item.Curve = EditorGUI.CurveField(new Rect(rect.width - 220, rect.y - 1, 150, rect.height), item.Curve);
            item.TargetObjectCount = EditorGUI.IntField(new Rect(rect.width - 65, rect.y, 55, rect.height),
                item.TargetObjectCount);
            if (GUI.Button(new Rect(rect.width - 5f, rect.y + 2, 35f, rect.height - 5), "Del"))
            {
                _levelPopulator.GrammarItems.Remove(item);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void AddItem(ReorderableList reordableList)
        {
            _levelPopulator.GrammarItems.Add(new KeyValueCurveGroup());

            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList reordableList)
        {
            _levelPopulator.GrammarItems.RemoveAt(reordableList.index);

            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var style = new GUIStyle();
            style.richText = true;
            style.stretchWidth = true;

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_populateLevel, new GUIContent("Populate Level"));
            EditorGUILayout.PropertyField(_excludeLayerProperty, new GUIContent("Exclude Layer from Population"));
            GUILayout.EndHorizontal();

            if (_populateLevel.boolValue)
            {   
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                reorderableList.DoLayoutList();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Search Prefabs"))
                {
                    _levelPopulator.MapPrefabs();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(_showStats, new GUIContent("Show Stats"));

                if (_levelPopulator.ShowStats)
                {
                    GUILayout.BeginHorizontal();
                    EditorHelper.DrawHelpBox("Placed Objects" + "\n" + GetRtfFormatedGroupedCount());
                    EditorHelper.DrawHelpBox(
                        "<b>Duration: </b>" + StatsLocator.GetLevelPopulationLog().PopulationDuration + "ms" + "\n" +
                        "<b>Overall Object Count: </b>" + StatsLocator.GetLevelPopulationLog().GetOverallObjectCount() + "\n");
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Populate Level"))
                {
                    _levelPopulator.gameObject.GetComponent<LevelGenerator>().SetupLevelParentObject();
                    _levelPopulator.StartPopulation(new XXHash(1515), false, _levelPopulator.GrammarItems, GameObjectHelper.CreateEmptyPCGParent(_levelPopulator.gameObject.GetComponent<GeneratorData>().ParentName));
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.EndVertical();
                GUILayout.Space(10f);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public string GetRtfFormatedGroupedCount()
        {
            StringBuilder sb = new StringBuilder();

            var counter = 0;
            if (StatsLocator.GetLevelPopulationLog() == null) return sb.ToString();

            foreach (var group in StatsLocator.GetLevelPopulationLog().ObjectCount)
            {
                sb.Append("---" + "<b>" + StringHelper.ExceptCharsFromString(@group._Key, new[] {'<', '>'}) + "</b>" + " (" + @group._Value.Sum(item => item.Value) +  ")" + "\n");
                foreach (var pair in @group._Value)
                {
                    sb.Append("------" + StringHelper.ExceptCharsFromString(pair.Key, new[] { '<', '>' }) + " (" + pair.Value + ")" + "\n");
                }
                counter++;
                if (counter < StatsLocator.GetLevelPopulationLog().ObjectCount.Count)
                {
                    sb.Append("\n");
                }
            }


            return sb.ToString();
        }
    }
}