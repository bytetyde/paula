using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Gramatical_Textreplacer;
using Assets.Scripts.Level_Generator.Instantiater;
using Assets.Scripts.Level_Generator.Populator;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Infinite
{
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    public class InfiniteLevelSettings : MonoBehaviour
    {
        [HideInInspector] public List<IntListStringCurveIntPair> AssetCountList = new List<IntListStringCurveIntPair>();
        [HideInInspector] public List<IntStringPair> SectionStringList = new List<IntStringPair>();
        public bool GenerateNegativeDirection;
        public bool GeneratePositiveDirection;
        public bool GetBackToStart;
        public bool Infinite;
        public bool IsVertical;
        public int MinimumClearance = 20;
        public GameObject Player;
        public int ActiveSections = 2;
        public int StartSeed;
        public int StartValue;

        private StringReplacerHolder _stringReplacerHolder;
        private LevelInstantiater _levelInstantiater;
        private LevelPopulator _levelPopulator;
        private XXHash _xxHash;
        private Direction _lastMovementDirection;
        private List<string> _startStringList = new List<string>();
        private List<KeyValueCurveGroup> _grammarItems = new List<KeyValueCurveGroup>();
        private readonly List<int> _modifiedObjects = new List<int>();
        private readonly List<GameObject> _sections = new List<GameObject>();
        private readonly SortedList<int, List<KeyValueCurveGroup>> _sortedAssetGrammarItemList = new SortedList<int, List<KeyValueCurveGroup>>();
        private readonly SortedList<int, string> _sortedSectionList = new SortedList<int, string>();
        private Bounds _levelBounds;
        private GameObject _levelGameObject;
        private StringParser _stringParser;
        private bool _updateBounds;
        private int _lastSectionKey;
        private int _sectionNumber = -1;

        // Use this for initialization
        private void Awake()
        {
            if (Application.isPlaying && Infinite)
            {
                SetComponents();
            }
        }

        public void Start()
        {
            if (Application.isPlaying && Infinite)
            {
                GenerateNewSection(true);
            }
        }

        public void Setup(GameObject lvlObj)
        {
            _levelGameObject = lvlObj;
            _sectionNumber = StartValue - 1;

            SetComponents();
            SetupSortedSectionStartStringDictionary();
            SetupSortedSectionGrammarItemsDictionary();
        }

        public void SetupSortedSectionStartStringDictionary()
        {
            foreach (var pair in SectionStringList)
            {
                _sortedSectionList.Add(pair.Key, pair.Value);
            }
        }

        public void SetupSortedSectionGrammarItemsDictionary()
        {
            foreach (var pair in AssetCountList)
            {
                _sortedAssetGrammarItemList.Add(pair.Key, pair.Value);
            }
        }

        public void SetComponents()
        {
            var seed = StartSeed == -1 ? (int) DateTime.Now.Ticks : StartSeed;
            _xxHash = new XXHash(seed);
            _stringReplacerHolder = gameObject.GetComponent<StringReplacerHolder>();
            _stringParser = gameObject.GetComponent<StringParser>();
            _levelInstantiater = gameObject.GetComponent<LevelInstantiater>();
            _levelPopulator = gameObject.GetComponent<LevelPopulator>();
        }

        public List<string> SetSectionStartString(bool insertAhead, int sectionNumber)
        {
            var valueString = "";

            if (_sortedSectionList.ContainsKey(sectionNumber))
            {
                valueString = _sortedSectionList[sectionNumber];
                _lastSectionKey = sectionNumber;
            }
            else if (insertAhead && sectionNumber < _lastSectionKey)
            {
                var index = _sortedSectionList.IndexOfKey(_lastSectionKey) - 1;
                if (index != -1 && index != -2 && index < _sortedSectionList.Count)
                {
                    valueString = _sortedSectionList.ElementAt(index).Value;
                }
            }
            else if (!insertAhead && sectionNumber > _lastSectionKey)
            {
                var index = _sortedSectionList.IndexOfKey(_lastSectionKey) + 1;
                if (index != -1 && index != 0 && index < _sortedSectionList.Count)
                {
                    if (sectionNumber == _sortedSectionList.ElementAt(index).Key)
                    {
                        valueString = _sortedSectionList.ElementAt(index).Value;
                    }
                }
            }

            if (valueString == "")
            {
                return _startStringList;
            }

            var tokens = StringHelper.GetAllTokensInString(valueString);

            return tokens;
        }

        public List<KeyValueCurveGroup> SetSectionAssetList(bool insertAhead, int sectionNumber)
        {
            List<KeyValueCurveGroup> list = null;

            if (_sortedAssetGrammarItemList.ContainsKey(sectionNumber))
            {
                list = _sortedAssetGrammarItemList[sectionNumber];
                _lastSectionKey = sectionNumber;
            }
            else if (insertAhead && sectionNumber < _lastSectionKey)
            {
                var index = _sortedAssetGrammarItemList.IndexOfKey(_lastSectionKey) - 1;
                if (index != -1 && index != -2 && index < _sortedAssetGrammarItemList.Count)
                {
                    list = _sortedAssetGrammarItemList.ElementAt(index).Value;
                }
            }
            else if (!insertAhead && sectionNumber > _lastSectionKey)
            {
                var index = _sortedAssetGrammarItemList.IndexOfKey(_lastSectionKey) + 1;
                if (index != -1 && index != 0 && index < _sortedAssetGrammarItemList.Count)
                {
                    if (sectionNumber == _sortedSectionList.ElementAt(index).Key)
                    {
                        list = _sortedAssetGrammarItemList.ElementAt(index).Value;
                    }
                }
            }

            return list ?? _grammarItems;
        }

        public void GenerateNewSection(bool rightOrUp)
        {
            var insertAhead = SetGenerationDirections(rightOrUp);
            _startStringList = SetSectionStartString(insertAhead, _sectionNumber);
            _grammarItems = SetSectionAssetList(insertAhead, _sectionNumber);

            var sectionObj = new GameObject("section" + _sectionNumber);
            sectionObj.transform.parent = _levelGameObject.transform;

            Add(insertAhead, sectionObj);

            _stringReplacerHolder.Setup(new LevelTokenReplacer(), _xxHash, _sectionNumber,
                _stringParser.GenerateTokenDictionary(_stringParser.InputList), _startStringList,
                _stringParser.GetCodonList());
            _stringReplacerHolder.StartTextReplacing();
            var levelString = _stringReplacerHolder.GetGeneratedLevel();
            _levelInstantiater.Setup(levelString, sectionObj, _stringReplacerHolder._generationDuration, _stringReplacerHolder._usedCodonNumbersString);
            if (_levelPopulator.PopulateLevel)
            {
                _levelPopulator.StartPopulation(_xxHash, true, _grammarItems, sectionObj, _sectionNumber, _modifiedObjects);
            }
            else
            {
                _levelInstantiater.SetupLevelObjects();
            }

            if (rightOrUp)
            {
                if (IsVertical)
                {
                    if (_sectionNumber > StartValue)
                    {
                        var bounds = GameObjectHelper.CalculateBoundsWithChildren(sectionObj, "pcgplatform");

                        var yOffset = sectionObj.transform.position.y + (bounds.min.y + sectionObj.transform.position.y);
                        sectionObj.transform.position = new Vector3(0, _levelBounds.max.y + _levelInstantiater.MinMaxVertical.x - yOffset, 0);
                    }
                }
                else
                {
                    sectionObj.transform.position =
                        new Vector3(Mathf.Round(_levelBounds.max.x + _levelInstantiater.MinMaxHorizontal.x), 0, 0);
                }
            }
            else
            {
                if (IsVertical)
                {
                    var bounds = GameObjectHelper.CalculateBoundsWithChildren(sectionObj, "pcgplatform");

                    var yOffset = sectionObj.transform.position.y - (bounds.max.y - sectionObj.transform.position.y);

                    sectionObj.transform.position = new Vector3(0, _levelBounds.min.y - _levelInstantiater.MinMaxVertical.x + yOffset , 0);
                }
                else
                {
                    sectionObj.transform.position =
                        new Vector3(_levelBounds.min.x -(GameObjectHelper.CalculateBoundsWithChildren(sectionObj).size.x +_levelInstantiater.MinMaxHorizontal.x), 0, 0);
                }
            }

            _levelBounds = GameObjectHelper.CalculateGlobalBoundsOfChildren(_levelGameObject);
        }

        private bool SetGenerationDirections(bool rightOrUp)
        {
            bool insertAhead;
            if (!rightOrUp && (_lastMovementDirection == Direction.Right || _lastMovementDirection == Direction.Up))
            {
                _sectionNumber -= ActiveSections;
                insertAhead = true;
            }
            else if (!rightOrUp)
            {
                _sectionNumber--;
                insertAhead = true;
            }
            else if (rightOrUp && (_lastMovementDirection == Direction.Left || _lastMovementDirection == Direction.Down))
            {
                _sectionNumber += ActiveSections;
                insertAhead = false;
            }
            else
            {
                _sectionNumber++;
                insertAhead = false;
            }

            return insertAhead;
        }

        public bool ExistsAt(int index)
        {
            return _sections.ElementAtOrDefault(index) != null;
        }

        public void Add(bool insertAhead, GameObject item)
        {
            if (insertAhead)
            {
                _sections.Insert(0, item);
            }
            else
            {
                _sections.Add(item);
            }
        }

        public void InsertAt(int index, GameObject item)
        {
            _sections.Insert(index, item);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Infinite) return;

            if (_updateBounds)
            {
                _levelBounds = GameObjectHelper.CalculateGlobalBoundsOfChildren(_levelGameObject);
                _updateBounds = false;
            }

            if (GeneratePositiveDirection || (GetBackToStart && _sectionNumber < 0 && _sectionNumber != 0))
            {
                if (IsVertical)
                {
                    if (Player.transform.position.y > _levelBounds.max.y - MinimumClearance)
                    {
                        GenerateNewSection(true);
                        _lastMovementDirection = Direction.Up;

                        if (_sections.Count > ActiveSections)
                        {
                            var elem = _sections[0];
                            var serializers = elem.GetComponentsInChildren<InfiniteLevelObjectSerializer>();
                            foreach (var serializer in serializers)
                            {
                                serializer.SectionRemoved = true;
                            }
                            Destroy(elem);
                            _sections.RemoveAt(0);
                            _updateBounds = true;
                        }
                    }
                }
                else
                {
                    if (Player.transform.position.x > _levelBounds.max.x - MinimumClearance)
                    {
                        GenerateNewSection(true);
                        _lastMovementDirection = Direction.Right;

                        if (_sections.Count > ActiveSections)
                        {
                            var elem = _sections[0];
                            var serializers = elem.GetComponentsInChildren<InfiniteLevelObjectSerializer>();
                            foreach (var serializer in serializers)
                            {
                                serializer.SectionRemoved = true;
                            }
                            Destroy(elem);
                            _sections.RemoveAt(0);
                            _updateBounds = true;
                        }
                    }
                }
            }

            if (GenerateNegativeDirection || (GetBackToStart && _sectionNumber > 0))
            {
                if (IsVertical)
                {
                    if (Player.transform.position.y < _levelBounds.min.y + MinimumClearance)
                    {
                        GenerateNewSection(false);
                        _lastMovementDirection = Direction.Down;

                        if (_sections.Count > ActiveSections)
                        {
                            var elem = _sections[ActiveSections];
                            var serializers = elem.GetComponentsInChildren<InfiniteLevelObjectSerializer>();
                            foreach (var serializer in serializers)
                            {
                                serializer.SectionRemoved = true;
                            }
                            Destroy(elem);
                            _sections.RemoveAt(ActiveSections);
                            _updateBounds = true;
                        }
                    }
                }
                else
                {
                    if (Player.transform.position.x < _levelBounds.min.x + MinimumClearance)
                    {
                        GenerateNewSection(false);
                        _lastMovementDirection = Direction.Left;

                        if (_sections.Count > ActiveSections)
                        {
                            var elem = _sections[ActiveSections];
                            var serializers = elem.GetComponentsInChildren<InfiniteLevelObjectSerializer>();
                            foreach (var serializer in serializers)
                            {
                                serializer.SectionRemoved = true;
                            }
                            Destroy(elem);
                            _sections.RemoveAt(ActiveSections);
                            _updateBounds = true;
                        }
                    }
                }
            }
        }

        public void UpdateAssetCountPopulationLists()
        {
            for (var i = 0; i < AssetCountList.Count; i++)
            {
                var assetCountItem = AssetCountList[i];
                foreach (var curveGroup in _levelPopulator.GrammarItems)
                {
                    if (assetCountItem.Value.All(item => item.Key != curveGroup.Key))
                    {
                        assetCountItem.Value.Add(new KeyValueCurveGroup(curveGroup.Key, curveGroup.Value,
                            curveGroup.TargetObjectCount, curveGroup.Curve));
                    }
                }
                for (var index = assetCountItem.Value.Count - 1; index >= 0; index--)
                {
                    var @group = assetCountItem.Value[index];
                    if (_levelPopulator.GrammarItems.All(item => item.Key != @group.Key))
                    {
                        assetCountItem.Value.Remove(@group);
                    }
                }
            }
        }

        public int GetAssetCountListItemHeight()
        {
            return _levelPopulator.GrammarItems.Count;
        }

        public void SafeToJsonFile()
        {
#if UNITY_EDITOR
            var path = EditorUtility.SaveFilePanel("Save Infinite Generation Settings", "", "InfiniteSettings", "txt");

            var sb = new StringBuilder();

            sb.Append("Variables");
            sb.Append("\n");
            sb.Append(
                JsonUtility.ToJson(new KeyValueString("GeneratePositiveDirection", GeneratePositiveDirection.ToString())));
            sb.Append("\n");
            sb.Append(
                JsonUtility.ToJson(new KeyValueString("GenerateNegativeDirection", GenerateNegativeDirection.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("GetBackToStart", GetBackToStart.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("ActiveSections", ActiveSections.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("Infinite", Infinite.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("IsVertical", IsVertical.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("MinimumClearance", MinimumClearance.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("Player", EditorJsonUtility.ToJson(Player))));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("StartSeed", StartSeed.ToString())));
            sb.Append("\n");
            sb.Append(JsonUtility.ToJson(new KeyValueString("StartValue", StartValue.ToString())));
            sb.Append("\n");

            sb.Append("SectionStringList");
            sb.Append("\n");
            for (var i = 0; i < SectionStringList.Count; i++)
            {
                sb.Append(JsonUtility.ToJson(SectionStringList[i]));
                sb.Append("\n");
            }

            sb.Append("AssetCountList");
            if (AssetCountList.Count > 0)
            {
                sb.Append("\n");
            }
            for (var i = 0; i < AssetCountList.Count; i++)
            {
                sb.Append(JsonUtility.ToJson(AssetCountList[i]));
                if (i < AssetCountList.Count - 1)
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

                SectionStringList.Clear();
                AssetCountList.Clear();

                for (var index = 0; index < linesInFile.Length; index++)
                {
                    if (linesInFile[index] == "Variables")
                    {
                        index++;
                        while (linesInFile[index] != "SectionStringList")
                        {
                            var obj = JsonUtility.FromJson<KeyValueString>(linesInFile[index]);
                            var fieldInfo = GetType().GetField(obj.key);
                            if (fieldInfo != null)
                            {
                                if (obj.key == "Player")
                                {
                                    //EditorJsonUtility.FromJsonOverwrite(obj.value, Player);
                                }
                                else
                                {
                                    fieldInfo.SetValue(this, Convert.ChangeType(obj.value, fieldInfo.FieldType));
                                }
                            }
                            index++;
                        }
                    }
                    if (linesInFile[index] == "SectionStringList")
                    {
                        index++;
                        while (linesInFile[index] != "AssetCountList")
                        {
                            var intStringPair = JsonUtility.FromJson<IntStringPair>(linesInFile[index]);

                            SectionStringList.Add(intStringPair);
                            index++;
                        }
                    }
                    if (linesInFile[index] == "AssetCountList")
                    {
                        index++;
                        while (index < linesInFile.Length)
                        {
                            var keyValueCurveGroup = JsonUtility.FromJson<IntListStringCurveIntPair>(linesInFile[index]);

                            AssetCountList.Add(keyValueCurveGroup);
                            index++;
                        }
                    }
                }
            }
#endif
        }
    }
}