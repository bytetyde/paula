using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace Assets.Editor.Custom_Editor
{
    [CustomEditor(typeof (StringParser))]
    public class StringParserEditor : UnityEditor.Editor
    {
        private readonly Texture2D _greyTexture2D = new Texture2D(1, 1);

        private ReorderableList _reorderableList;
        private StringParser _stringParser;
        private SerializedProperty _codonNumbers;
        private SerializedProperty _groundCells;
        private SerializedProperty _startString;
        private SerializedProperty _startSeed;


        private void OnEnable()
        {
            _stringParser = (StringParser) target;

            _codonNumbers = serializedObject.FindProperty("CodonNumbers");
            _startString = serializedObject.FindProperty("StartString");
            _startSeed = serializedObject.FindProperty("StartSeed");

            _greyTexture2D.SetPixel(1, 1, new Color32(70, 70, 70, 1));
            _greyTexture2D.Apply();

            _reorderableList = new ReorderableList(_stringParser.InputList, typeof (KeyValueTokenString), true, true, true,
                true);

            _reorderableList.drawHeaderCallback += DrawHeader;
            _reorderableList.drawElementCallback += DrawElement;

            _reorderableList.onAddCallback += AddItem;
            _reorderableList.onRemoveCallback += RemoveItem;
        }

        private void OnDisable()
        {
            _reorderableList.drawHeaderCallback -= DrawHeader;
            _reorderableList.drawElementCallback -= DrawElement;

            _reorderableList.onAddCallback -= AddItem;
            _reorderableList.onRemoveCallback -= RemoveItem;
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Level Grammar");
        }

        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            if (_stringParser.InputList.ElementAtOrDefault(index) == null)
            {
                return;
            }

            var style = new GUIStyle();
            style.fontSize = 8;
            style.normal.textColor = new Color(0, 0, 0, 0.5f);

            var item = _stringParser.InputList[index];

            EditorGUI.BeginChangeCheck();
            item.KeyString = EditorGUI.TextField(new Rect(rect.x + 18, rect.y, 0 + rect.width/5, rect.height),
                item.KeyString);
            item.ValueString =
                EditorGUI.TextField(new Rect(rect.x + 18 + rect.width*0.2f, rect.y, rect.width*0.8f - 220, rect.height),
                    item.ValueString);
            EditorGUI.DrawPreviewTexture(new Rect(rect.width - 170, rect.y, 204, rect.height),_greyTexture2D, null, ScaleMode.StretchToFill);
            if (item.IsManuallyMapped || item.IsMapped)
            {
                item.Difficulty =
                    (int)
                        GUI.HorizontalSlider(new Rect(rect.width - 166, rect.y + 2, 88, rect.height), item.Difficulty, 0,
                            100);
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.width - 162, rect.y + 2, 88, rect.height), "No mapped Pattern", style);
            }
            EditorGUI.DrawPreviewTexture(new Rect(rect.width - 74, rect.y + 1, 106, rect.height - 3),
                EditorHelper.EntryValidation(item.ValueString, item.IsMapped, item.IsManuallyMapped, _stringParser.InputList), null, ScaleMode.StretchToFill);
            item.IsCell = EditorGUI.Toggle(new Rect(rect.width - 22 - 16*3, rect.y + 2, 20, rect.height), item.IsCell);
            item.IsFinalString = EditorGUI.Toggle(new Rect(rect.width - 22 - 16*2, rect.y + 2, 20, rect.height),
                item.IsFinalString);
            item.IsMapped = EditorGUI.Toggle(new Rect(rect.width - 22 - 16*1, rect.y + 2, 20, rect.height), item.IsMapped);
            item.IsManuallyMapped = EditorGUI.Toggle(new Rect(rect.width - 22, rect.y + 2, 20, rect.height),
                item.IsManuallyMapped);
            if (GUI.Button(new Rect(rect.width - 5f, rect.y + 2, 35f, rect.height - 5), "Del"))
            {
                _stringParser.InputList.Remove(item);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

   

        private void AddItem(ReorderableList reordableList)
        {
            _stringParser.InputList.Add(new KeyValueTokenString());

            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList reordableList)
        {
            _stringParser.InputList.RemoveAt(reordableList.index);

            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var style = new GUIStyle();
            style.richText = true;
            style.stretchWidth = true;

            GUILayout.Space(10f);
            EditorGUILayout.PropertyField(_codonNumbers, new GUIContent("Codon Numbers"));
            EditorGUILayout.PropertyField(_startString, new GUIContent("Start String"));
            EditorGUILayout.PropertyField(_startSeed, new GUIContent("Start Seed"));

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            #region VerticalGroup
            GUILayout.BeginVertical();
            GUILayout.Label("<size=11><b> Level Grammar Options</b></size>", style);
            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            #region VerticalGroup
            GUILayout.BeginVertical();
            GUILayout.Label("Options");
            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(
                "Each token on the left side will be replaced by one of the options on the right side.\n" +
                "Tokens have to be encapsulated by <> chars\n" +
                "Tokens can be grouped by a \"|\" delimiter.\n" +
                "The left-hand-side must contain only one token.", MessageType.Info);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            #endregion
            GUILayout.Label("Grammar Syntax");
            GUILayout.Label("Token:" + "\t" + "<color=teal><b><</b></color>Token<color=teal><b>></b></color>", style);
            GUILayout.Label(
                "Group:" + "\t" +
                "<color=teal><token1><token2><token3></color><color=red>|</color><color=olive><otherToken1><token2></color>",
                style);
            GUILayout.Space(10);
            GUILayout.EndVertical();
            #endregion

            #region VerticalGroup
            GUILayout.BeginVertical();
            GUILayout.Space(70);
            GUILayout.FlexibleSpace();

            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Manually Prefab-Mapped");
            GUILayout.Label("¬");
            GUILayout.Space(42);
            GUILayout.EndHorizontal();
            #endregion

            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Automatically Prefab-Mapped");
            GUILayout.Label("¬");
            GUILayout.Space(4f);
            GUILayout.Label("|");
            GUILayout.Space(42);
            GUILayout.EndHorizontal();
            #endregion

            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Use for Folder Structure Search");
            GUILayout.Label("¬");
            GUILayout.Space(4f);
            GUILayout.Label("|");
            GUILayout.Space(4f);
            GUILayout.Label("|");
            GUILayout.Space(42);
            GUILayout.EndHorizontal();
            #endregion

            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Level Cell");
            GUILayout.Label("¬");
            GUILayout.Space(4f);
            GUILayout.Label("|");
            GUILayout.Space(4f);
            GUILayout.Label("|");
            GUILayout.Space(4f);
            GUILayout.Label("|");
            GUILayout.Space(42);
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.EndVertical();
            #endregion
            GUILayout.EndHorizontal();
            #endregion
            _reorderableList.DoLayoutList();
            #region HorizontalGroup
            GUILayout.BeginHorizontal();
            #region VerticalGroup
            GUILayout.BeginVertical();
            GUILayout.Label("Dictionary Options");
            if (GUILayout.Button("Clear"))
            {
                _stringParser.ClearInputs();
            }

            if (GUILayout.Button("Save to file"))
            {
                _stringParser.SafeToJsonFile();
            }

            if (GUILayout.Button("Load from file"))
            {
                _stringParser.ReadFromJsonFile();
            }
            GUILayout.EndVertical();
            #endregion
            #region VerticalGroup
            GUILayout.BeginVertical();
            GUILayout.Label("Prefab Setup");
            EditorGUILayout.HelpBox(
                "For each path marked as 'Use for Folder Structure Search' the folder \"Levelgenerator Prefabs\" in the Resources folder will be searched for prefabs." +
                " The Instantiater component will take those found prefabs-paths an instantiate the actual level from them.",
                MessageType.Info);
            if (GUILayout.Button("Search for Prefabs in Folders", GUILayout.Width(250f)))
            {
                _stringParser.SearchPrefabPaths();
            }
            GUILayout.EndVertical();
            #endregion
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            #endregion
            GUILayout.EndVertical();
            #endregion
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);

            serializedObject.ApplyModifiedProperties();
        }
    }
}