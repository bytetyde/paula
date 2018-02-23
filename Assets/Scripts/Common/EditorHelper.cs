using System.Collections.Generic;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Assets.Scripts.Common
{
    public static class EditorHelper
    {
        public static Texture2D EntryValidation(string str, bool mapped, bool manually, List<KeyValueTokenString> list)
        {
            var _greenTexture2D = new Texture2D(1, 1);
            var _redTexture2D = new Texture2D(1, 1);

            _greenTexture2D.SetPixel(1, 1, new Color32(69, 191, 85, 1));
            _greenTexture2D.Apply();
            _redTexture2D.SetPixel(1, 1, new Color32(217, 91, 69, 1));
            _redTexture2D.Apply();

            var bracketCounter = 0;
            var isValid = false;

            if (str != null)
            {
                for (var i = 0; i < str.Length; i++)
                {
                    bracketCounter += str[i] == '<' ? 1 : 0;
                    bracketCounter += str[i] == '>' ? -1 : 0;
                }
                isValid = bracketCounter == 0 ? true : false;

                if (!mapped && !manually)
                {
                    isValid = isValid && CheckDictionaryContainment(str, list);
                }
            }

            return isValid ? _greenTexture2D : _redTexture2D;
        }


        public static bool CheckDictionaryContainment(string str, List<KeyValueTokenString> list)
        {
            var editorEntries = StringHelper.GetMatchingStringPartsInString(str, @"<([^<>].*?)>");
            var grammarList = new List<string>();

            foreach (var entry in list)
            {
                grammarList.Add(entry.KeyString);
            }

            foreach (var entry in editorEntries)
            {
                if (!grammarList.Contains(entry))
                {
                    return false;
                }
            }
            return true;
        }

        public static void DrawHeader(string label)
        {
            var labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.richText = true;
#if UNITY_EDITOR
            EditorGUILayout.LabelField(new GUIContent("<b>" + label + "</b>"), labelStyle);
#endif
        }

        public static void DrawSeparator()
        {
#if UNITY_EDITOR
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
#endif
        }

        public static void DrawHelpBox(string text)
        {
            var helpBoxStyle = GUI.skin.GetStyle("HelpBox");
            helpBoxStyle.richText = true;
#if UNITY_EDITOR
            EditorGUILayout.TextArea(text, helpBoxStyle);
#endif
        }
    }
}