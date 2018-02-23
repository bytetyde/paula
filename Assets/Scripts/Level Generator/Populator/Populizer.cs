using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Gramatical_Textreplacer;
using Assets.Scripts.Level_Generator.Infinite;
using Assets.Scripts.Level_Generator.Logging;
using Assets.Scripts.Level_Generator.Stats;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Populator
{
    [ExecuteInEditMode]
    public class Populizer : MonoBehaviour
    {
        private bool _checkSerialization = true;
        private string _excludeLayer;
        public int _frameCounter;
        private GeneratorData _generatorData;
        private bool _infiniteGeneration;
        private List<int> _modifiedObjects;
        public List<GameObject> _Objects = new List<GameObject>();
        private int _sectionNumber;
        private int _seed;

        private StringReplacerHolder _stringReplacerHolder;
        public int _waitForFrame;
        private XXHash _xxHash;
        public List<int> CodonList;

        public string CodonNumbers = "-1 -1 -1 -1";
        public Dictionary<string, List<List<string>>> Dictionary;
        public string EvolutionString;
        public List<PopulationPair> ObjectPool = new List<PopulationPair>();
        public List<SpawnPosition> SpawnPositions = new List<SpawnPosition>();
        public List<string> StringList;
        public string UsedCodonNumbers;

        public void Setup(XXHash xxHash, int seed, int waitForFrame, bool serializeObjects, int sectionNumber,
            List<int> modifiedObjects, bool infiniteGeneration, GeneratorData data, string excludeLayer)
        {
            _xxHash = xxHash;
            _seed = seed;
            _frameCounter = 0;
            _waitForFrame = waitForFrame;
            _infiniteGeneration = infiniteGeneration;
            _sectionNumber = sectionNumber;
            _modifiedObjects = modifiedObjects;
            _generatorData = data;
            _excludeLayer = excludeLayer;
            _checkSerialization = serializeObjects;

            var bounds = GameObjectHelper.CalculateGlobalBoundsOfChildren(gameObject);
            SpawnPositions.Clear();

            var minVector = bounds.min;
            var maxVector = bounds.max;

            for (var i = Mathf.RoundToInt(minVector.x) + 0.5f; i < maxVector.x; i++)
            {
                var pos = gameObject.transform.position;
                pos = new Vector3(i, maxVector.y, pos.z);

    //Method to shoot a vertical raycast at the desired position and if found, return the hitpoint
    bool hasHitSomething;
    Vector3 hitPoint;
    RayCastPoint(pos, out hitPoint, out hasHitSomething);
    if (hasHitSomething)
    {
        SpawnPositions.Add(new SpawnPosition(hitPoint));
                }
            }
            if (Application.isEditor && !Application.isPlaying || !infiniteGeneration)
            {
                StartEvolution();
            }
        }

        public void Update()
        {
            if (!_infiniteGeneration)
            {
                return;
            }

            if (Application.isEditor && !Application.isPlaying) return;

            if (_frameCounter == _waitForFrame)
            {
                StartEvolution();
                _frameCounter = -1;
            }
            if (_frameCounter != -1)
            {
                _frameCounter++;
            }
        }

        public void StartEvolution()
        {
            _stringReplacerHolder = !gameObject.GetComponent<StringReplacerHolder>()
                ? gameObject.AddComponent<StringReplacerHolder>()
                : gameObject.GetComponent<StringReplacerHolder>();

            Dictionary = PrepareDictionary();
            StringList = PrepareStartString();
            CodonList = GetCodonList();

            LogLocator.GetLogger().LogLine("\nPopulization Evolution");

            _stringReplacerHolder.Setup(new AssetTokenReplacer(), _xxHash, _seed, Dictionary, StringList, CodonList);
            _stringReplacerHolder.StartTextReplacing();
            UsedCodonNumbers = _stringReplacerHolder._usedCodonNumbersString;
            EvolutionString = StringHelper.GetListEntriesAsString(_stringReplacerHolder.GetGeneratedLevel());
            InstantiateObjects(EvolutionString);
        }

        public void Repopulate()
        {
            RemoveObjectsFromPattern();
            if (Dictionary != null && CodonList != null)
            {
                Dictionary.Clear();
                CodonList.Clear();
            }
            StartEvolution();
        }

        public Dictionary<string, List<List<string>>> PrepareDictionary()
        {
            var dictionary = new Dictionary<string, List<List<string>>>();

            foreach (var populationPair in ObjectPool)
            {
                var keyString = populationPair.GrammarEntry;
                var stringGroups = StringHelper.SeparateStringIntoPartsBySingleDelimiter(populationPair.Value, '|');
                var stringList = new List<List<string>>();

                foreach (var entry in stringGroups)
                {
                    var pos = "<spawn-position>";
                    var name = StringHelper.GetTokenNameWithoutAttributes(entry);

                    if (StringHelper.HasTokenAttribute(entry, "stackable"))
                    {
                        pos = "<spawn-position stackable=\'yes\'>";
                    }

                    stringList.Add(new List<string> {pos, name});
                }

                if (!dictionary.ContainsKey(keyString))
                {
                    dictionary.Add(keyString, stringList);
                }
            }
            if (!dictionary.ContainsKey("<spawn-position>"))
            {
                var spawnPositions = new List<List<string>>();
                foreach (var posPair in SpawnPositions)
                {
                    spawnPositions.Add(new List<string> {"<" + JsonUtility.ToJson(posPair) + ">"});
                }
                dictionary.Add("<spawn-position>", spawnPositions);
            }

            return dictionary;
        }

        public List<string> PrepareStartString()
        {
            if (StringList != null)
            {
                return StringList;
            }

            var stringList = new List<string>();

            foreach (var pair in ObjectPool)
            {
                for (var i = 0; i < pair.Count; i++)
                {
                    stringList.Add(pair.GrammarEntry);
                }
            }
            return stringList;
        }

        public List<int> GetCodonList()
        {
            if (CodonNumbers == null)
            {
                return PrepareCodonList();
            }
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
            return integerList;
        }

        public List<int> PrepareCodonList()
        {
            var codonList = new List<int>();

            foreach (var pair in ObjectPool)
            {
                for (var i = 0; i < pair.Count; i++)
                {
                    codonList.Add(-1);
                }
            }
            return codonList;
        }

        public void RemoveObjectsFromPattern()
        {
            foreach (var o in _Objects)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(o);
                }
                else
                {
                    Destroy(o);
                }
            }
            _Objects.Clear();
        }

        public void InstantiateObjects(string objects)
        {
            var objList = StringHelper.GetAllTokensInString(objects);

            for (var i = 0; i < objList.Count; i+=2)
            {
                var positionItem = objList[i];

                if (positionItem.Contains("\"x\":") && positionItem.Contains("\"y\":") &&
                    positionItem.Contains("\"z\":"))
                {
                    var objItem = objList[i + 1];
                    var positionString = StringHelper.ExceptCharsFromString(positionItem, new[] {'<', '>'});
                    var objPath = StringHelper.ExceptCharsFromString(objItem, new[] {'<', '>'});
                    var pos = JsonUtility.FromJson<SpawnPosition>(positionString);


                    var rayCastPos = _infiniteGeneration
                        ? gameObject.transform.TransformPoint(pos.Position)
                        : pos.Position;

                    var hasHitSomething = false;
                    Vector3 hitPoint;
                    RayCastPoint(rayCastPos, out hitPoint, out hasHitSomething);

                    if (!hasHitSomething || gameObject.layer == LayerMask.NameToLayer(_excludeLayer))
                    {
                        return;
                    }

                    var hash = 0;
                    if (_checkSerialization)
                    {
                        hash = StringHelper.CalculateHash(_sectionNumber + objPath + (gameObject.transform.position - hitPoint));
                        if (_modifiedObjects.Contains(hash))
                        {
                            return;
                        }
                    }

                    var obj = Resources.Load("Levelgenerator Prefabs" + "/" + objPath);

                    if (obj != null)
                    {
                        pos.Position = hitPoint;
                        var gObj = (GameObject) Instantiate(obj, new Vector3(), new Quaternion());
                        StatsLocator.GetLevelPopulationLog().AddCountToDictionary(objItem, 1, 1);
                        gObj.transform.parent = gameObject.transform;
                        gObj.transform.position = pos.Position;

                        var serializer = gObj.AddComponent<InfiniteLevelObjectSerializer>();
                        if (hash != 0)
                        {
                            serializer.Setup(_modifiedObjects, hash);
                        }
                        _Objects.Add(gObj);
                        gObj.name = objPath;
                    }
                }
            }
        }

        public void RayCastPoint(Vector3 pos, out Vector3 hitPoint, out bool hasHitSomething, float originHeight = 100)
        {
            var uses2DCollider = _generatorData.TwoDimensional;
            var initialLayer = gameObject.layer;
            hitPoint = Vector3.zero;
            hasHitSomething = false;

            GameObjectHelper.ChangeGameObjectLayerRecursive(gameObject, "tempRaycastLayer");

            if (uses2DCollider)
            {
                var hit = Physics2D.Raycast(new Vector2(pos.x, pos.y + originHeight), new Vector2(0, -1), Mathf.Infinity,1 << LayerMask.NameToLayer("tempRaycastLayer"));

                GameObjectHelper.ChangeGameObjectLayerRecursive(gameObject, initialLayer);

                if (hit.collider != null)
                {
                    hasHitSomething = true;
                    hitPoint = hit.point;
                }
            }
            else
            {
                var ray = new Ray();
                RaycastHit hit;
                ray.origin = new Vector3(pos.x, pos.y + originHeight, pos.z);
                ray.direction = new Vector3(0, -1, 0);

                GameObjectHelper.ChangeGameObjectLayerRecursive(gameObject, initialLayer);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("tempRaycastLayer")))
                {
                    hasHitSomething = true;
                    hitPoint = hit.point;
                }

                GameObjectHelper.ChangeGameObjectLayerRecursive(gameObject, initialLayer);
            }
        }
    }

    [Serializable]
    public class SpawnPosition
    {
        public List<GameObject> Objects = new List<GameObject>();
        public Vector3 Position;

        public SpawnPosition()
        {
        }

        public SpawnPosition(Vector3 pos)
        {
            Position = pos;
        }
    }

    [Serializable]
    public class PopulationPair
    {
        public int Count;
        public string GrammarEntry;
        public string Value;

        public PopulationPair()
        {
        }

        public PopulationPair(string grammarEntry, string value, int count)
        {
            GrammarEntry = grammarEntry;
            Value = value;
            Count = count;
        }
    }
}