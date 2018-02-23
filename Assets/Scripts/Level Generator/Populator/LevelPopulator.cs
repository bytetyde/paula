using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Infinite;
using Assets.Scripts.Level_Generator.Instantiater;
using Assets.Scripts.Level_Generator.Logging;
using Assets.Scripts.Level_Generator.Stats;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Populator
{
    [ExecuteInEditMode]
    public class LevelPopulator : MonoBehaviour
    {
        public List<KeyValueCurveGroup> GrammarItems = new List<KeyValueCurveGroup>();
        public List<KeyValueTokenString> PatternList = new List<KeyValueTokenString>();
        public bool PopulateLevel;
        public bool ShowStats = false;
        public string ExcludeLayer;
        private int _waitForFrame = 1;

        public void SetPatternList()
        {
            var list = gameObject.GetComponent<StringParser>().InputList;

            foreach (
                var keyValueTokenString in
                    list.Where(keyValueTokenString => keyValueTokenString.IsMapped || keyValueTokenString.IsManuallyMapped))
            {
                if (!PatternList.Contains(keyValueTokenString))
                {
                    PatternList.Add(new KeyValueTokenString(keyValueTokenString.KeyString,
                        StringHelper.GetTokenNameWithoutAttributes(keyValueTokenString.ValueString),
                        keyValueTokenString.IsCell, keyValueTokenString.IsFinalString, keyValueTokenString.IsMapped,
                        keyValueTokenString.IsManuallyMapped, keyValueTokenString.Difficulty));
                }
            }
        }

        public List<KeyValueTokenString> GetPatternList()
        {
            SetPatternList();

            return PatternList;
        }

        public void StartPopulation(XXHash xxHash, bool serializeObjects, List<KeyValueCurveGroup> grammarItems, GameObject parent,
            int sectionNumber = 0, List<int> modifiedObjects = null)
        {
            _waitForFrame = 0;
            LogLocator.GetLogger().InsertNewSection("Level Populator Started");

            InformationLogSetup();
            PatternList.Clear();
            SetPatternList();
            var instantiater = gameObject.GetComponent<LevelInstantiater>();
            instantiater.SetParent(parent);
            instantiater.SetupLevelObjects();
            var levelObjects = instantiater.LevelBlueprint;
            var counter = 0;

            foreach (var cell in levelObjects)
            {
                foreach (var pattern in cell.Patterns)
                {
                    SetPatternDifficulty(pattern);
                }

                var absDifficulty = cell.Patterns.Sum(pattern => pattern.Difficulty);

                foreach (var pattern in cell.Patterns)
                {
                    if (pattern.Attributes.Contains(new KeyValueString("prepopulated", "yes")))
                    {
                        LogLocator.GetLogger().LogLine("Pattern has Attribute \"prepopulated\"", "won't be populated");
                        continue;
                    }

                    var populizer = pattern.GameObject.AddComponent<Populizer>();

                    foreach (var objectPool in grammarItems)
                    {
                        var time = 1f/levelObjects.Count*(levelObjects.IndexOf(cell) + 1);
                        var curveValue = objectPool.Curve.Evaluate(time);

                        var absoluteCurveValue = CalcAbsoluteCurveValue(levelObjects, objectPool.Curve);
                        var cellObjectCount = objectPool.TargetObjectCount/absoluteCurveValue*curveValue*cell.Patterns.Count;
                        var count = Mathf.CeilToInt(cellObjectCount/absDifficulty*pattern.Difficulty);

                        populizer.ObjectPool.Add(new PopulationPair(objectPool.Key, objectPool.Value, count));
                        LogLocator.GetLogger()
                            .LogLine("For Asset", objectPool.Key, "Populizer was setup with values", "Cell Count", count,
                                objectPool.Value, "set");
                    }
                    _waitForFrame++;
                    populizer.Setup(xxHash, sectionNumber + counter, _waitForFrame, serializeObjects, sectionNumber,
                        modifiedObjects, gameObject.GetComponent<InfiniteLevelSettings>().Infinite, gameObject.GetComponent<GeneratorData>(), ExcludeLayer);
                    counter++;
                }
            }
            StatsLocator.GetLevelPopulationLog().StopPopulationStopwatch();
        }

        private void InformationLogSetup()
        {
            StatsLocator.GetLevelPopulationLog().Reset();
            StatsLocator.GetLevelPopulationLog().StartPopulationStopwatch();
            foreach (var keyValueCurveGroup in GrammarItems)
            {
                StatsLocator.GetLevelPopulationLog()
                    .AddToList(new KeyValPair<string, Dictionary<string, int>>(keyValueCurveGroup.Key,
                        new Dictionary<string, int>()));
            }
        }

        public float CalcAbsoluteCurveValue(List<LevelCell> levelObjects, AnimationCurve curve)
        {
            float counter = levelObjects.SelectMany(levelObject => levelObject.Patterns).Count();

            var sum = 0f;
            for (var i = 0; i <= counter; i++)
            {
                var time = 1f/counter*((float) i + 1);
                sum += curve.Evaluate(time);
            }
            return sum;
        }

        public void MapPrefabs()
        {
            foreach (var item in GrammarItems)
            {
                var folderName = StringHelper.ExceptCharsFromString(item.Key, new[] {'<', '>'});
                var path = PrefabMappingHelper.GetPrefabPath(Application.dataPath + "/Resources/Levelgenerator Prefabs/",
                    folderName);
                var prefabs = PrefabMappingHelper.GetPathsForFilesInFolder(path, folderName);
                item.Value = StringHelper.GetListEntriesAsString(prefabs, "<", ">", "|");
            }
        }

        public void SetPatternDifficulty(LevelPattern pattern)
        {
            foreach (var keyValueTokenString in PatternList)
            {
                if (keyValueTokenString.ValueString.Contains(pattern.Name))
                {
                    pattern.Difficulty = keyValueTokenString.Difficulty;
                }
            }
        }
    }

    [Serializable]
    public class KeyValueCurveGroup
    {
        public AnimationCurve Curve;
        public string Key;
        public int TargetObjectCount;
        public string Value;

        public KeyValueCurveGroup()
        {
            Curve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 0.5f));
        }

        public KeyValueCurveGroup(string name, string value, int targetObjectCount = 0)
        {
            Key = name;
            Value = value;
            TargetObjectCount = targetObjectCount;
            Curve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 0.5f));
        }

        public KeyValueCurveGroup(string name, string value, int targetObjectCount, AnimationCurve curve)
        {
            Key = name;
            Value = value;
            TargetObjectCount = targetObjectCount;
            Curve = curve;
        }
    }
}