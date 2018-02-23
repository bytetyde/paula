using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Gramatical_Textreplacer;
using Assets.Scripts.Level_Generator.Logging;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Level_Generator.Textreplacer_PreSetup
{
    public class StringParser : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("This numbers will be used to choose a right-hand-site rule during the evolution process" +
                 "\n" +
                 "\n" +
                 "You can set multiple numbers comma-delimited.\n" +
                 "If you inset a -1 the choice will be based on a random number.\n" +
                 "If you set nothing or you provide not enough numbers the choice will also based on a random number")]

        #endregion

        public string CodonNumbers = "";
        [HideInInspector] public List<KeyValueTokenString> InputList = new List<KeyValueTokenString>();
        public int StartSeed;
        public string StartString;

        public void RemoveSpareEntries(List<string> mappedTokens)
        {
            for (var i = 0; i < InputList.Count; i++)
            {
                var elem = InputList[i];

                if (elem.IsMapped && !mappedTokens.Contains(elem.KeyString) && !elem.IsManuallyMapped)
                {
                    InputList.Remove(elem);
                    i--;
                }
            }
        }

        public void SearchPrefabPaths()
        {
            var list = PrefabMappingHelper.SetupPrefabStringList(InputList);

            var prefabMapping = PrefabMappingHelper.GetPrefabsForTokens(list,
                Application.dataPath + "/Resources/Levelgenerator Prefabs/");

            foreach (var prefabPair in prefabMapping)
            {
                var stringList = StringHelper.GetListEntriesAsString(prefabPair.PrefabPath, "<", ">", "|");

                var keyValueToken = new KeyValueTokenString(prefabPair.Token, stringList, false, false, true, false);

                var index = InputList.IndexOf(keyValueToken);
                if (InputList.Contains(keyValueToken))
                {
                    if (InputList[index].IsMapped && !InputList[index].IsManuallyMapped)
                    {
                        var existingToken = InputList[InputList.IndexOf(keyValueToken)];
                        existingToken.ValueString = keyValueToken.ValueString;
                        existingToken.IsCell = false;
                        existingToken.IsFinalString = false;
                        existingToken.IsMapped = true;
                    }
                }
                else
                {
                    InputList.Add(keyValueToken);
                }
            }
            RemoveSpareEntries(list);
        }

        public List<string> GetStartString()
        {
            var startStrings = StringHelper.GetAllTokensInString(StartString);
            var tokenEvaluator = new LevelTokenReplacer();
            var queque = new Queue<int>();
            GetCodonList().ForEach(item => queque.Enqueue(item));
            tokenEvaluator.QueueSetup(new XXHash(StartSeed), StartSeed, new List<string>(), queque, 20);
            var tken = tokenEvaluator.ParseTokenList(20, startStrings);
            LogLocator.GetLogger().InsertNewSection("Start Tokens: ", StringHelper.GetListEntriesAsString(tken));
            return tken;
        }

        public List<int> GetCodonList()
        {
            var list = CodonNumbers.Split(' ');
            var integerList = new List<int>();
            foreach (var codonString in list)
            {
                try
                {
                    integerList.Add(int.Parse(codonString));
                }
                catch (Exception)
                {
                    throw new Exception("Could not parse input codon " + codonString);
                }
            }
            LogLocator.GetLogger()
                .InsertNewSection("Start Codon Values: ", StringHelper.GetListEntriesAsString(integerList));
            return integerList;
        }

        public Dictionary<string, List<List<string>>> GenerateTokenDictionary(List<KeyValueTokenString> list)
        {
            var dictionary = new Dictionary<string, List<List<string>>>();

            LogLocator.GetLogger().InsertSeparator();
            LogLocator.GetLogger().InsertNewSection("Token Dictionary Generation");
            foreach (var entry in list)
            {
                string keyString;
                var stringList = new List<List<string>>();

                if (entry.IsMapped)
                {
                    PrepareMappedEntries(entry, out keyString, stringList);
                    LogLocator.GetLogger().LogLine("Mapped Entry: " + keyString);
                }
                else
                {
                    PrepareDefaultEntries(entry, out keyString, stringList);
                    LogLocator.GetLogger().LogLine("Entry: " + keyString);
                }
                LogLocator.GetLogger().LogLine("Check Dictionary Containment");
                if (!dictionary.ContainsKey(keyString))
                {
                    LogLocator.GetLogger().LogLine("Dictionary does not contain key", keyString, "Entry and Values will be inserted","\nValues: ");
                    stringList.ForEach(listEntry => LogLocator.GetLogger().LogLine(StringHelper.GetListEntriesAsString(listEntry)));
                    LogLocator.GetLogger().InsertSeparator();
                    dictionary.Add(keyString, stringList);
                }
            }
            return dictionary;
        }

        private void PrepareDefaultEntries(KeyValueTokenString entry, out string keyString, List<List<string>> stringList)
        {
            keyString = entry.KeyString;
            var stringGroups = StringHelper.SeparateStringIntoPartsBySingleDelimiter(entry.ValueString, '|');
            foreach (var stringGroup in stringGroups)
            {
                var preparedRules = StringHelper.GetAllTokensInString(stringGroup);

                if (entry.IsCell)
                {
                    SetupCellTokens(stringList, preparedRules);
                }
                else
                {
                    stringList.Add(preparedRules);
                }
            }
        }

        private static void PrepareMappedEntries(KeyValueTokenString entry, out string keyString, List<List<string>> stringList)
        {
            keyString = entry.KeyString;
            var stringGroups = StringHelper.GetAllTokensInString(entry.ValueString);

            stringList.AddRange(stringGroups.Select(path => new List<string> {path}));
        }

        private void SetupCellTokens(List<List<string>> stringList, List<string> preparedRules)
        {
            preparedRules.Insert(0, "<" + preparedRules.Count + "-way-cell-group>");
            stringList.Add(preparedRules);
        }

        public void ClearInputs()
        {
            CodonNumbers = "";
            StartString = "";
            InputList.Clear();
        }

        public void SafeToJsonFile()
        {
#if UNITY_EDITOR
            var path = EditorUtility.SaveFilePanel("Save Content Settings", "", gameObject.GetComponent<GeneratorData>().ParentName, "txt");

            var sb = new StringBuilder();

            sb.Append(JsonUtility.ToJson(new KeyValueTokenString("CodonNumbers", CodonNumbers)));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueTokenString("StartString", StartString)));
            sb.Append("\n");

            for (var i = 0; i < InputList.Count; i++)
            {
                sb.Append(JsonUtility.ToJson(InputList[i]));
                if (i < InputList.Count - 1)
                {
                    sb.Append("\n");
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, sb.ToString());
            }
#endif
        }

        public void ReadFromJsonFile()
        {
#if UNITY_EDITOR
            var path = EditorUtility.OpenFilePanel("Choose file", "", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                var fileText = File.ReadAllText(path);
                var linesInFile = fileText.Split('\n');

                InputList.Clear();

                var counter = 0;
                foreach (var line in linesInFile)
                {
                    if (counter < 2)
                    {
                        var obj = JsonUtility.FromJson<KeyValueTokenString>(line);
                        var fieldInfo = GetType().GetField(obj.KeyString);
                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(this, Convert.ChangeType(obj.ValueString, fieldInfo.FieldType));
                        }
                    }
                    else
                    {
                        var keyValuePair = JsonUtility.FromJson<KeyValueTokenString>(line);

                        InputList.Add(keyValuePair);
                    }
                    counter++;
                }
            }
#endif
        }
    }
}