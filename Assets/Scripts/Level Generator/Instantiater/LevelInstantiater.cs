using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generator.Instantiater
{
    public class LevelInstantiater : MonoBehaviour
    {
        [HideInInspector] public List<string> GeneratedLevel = new List<string>();
        [HideInInspector] public List<LevelCell> LevelBlueprint;
        [HideInInspector] public string LevelGenerationDuration;
        [HideInInspector] public string UsedGenomes;
        public Vector3 ParentPosition;
        public string LayerName;
        public bool AnimateCells;
        public int AnimationOffset;
        public bool GroundCells;
        public string ExcludeTag;
        public float AnimationDuration;
        public bool VerticalConstruction;
        public Vector2 MinMaxVertical;
        public Vector2 MinMaxHorizontal;

        private GameObject _parentGameObject;
        private Bounds _levelBounds;
        private GeneratorData _generatorData;

        public void Setup(List<string> level, GameObject levelParent, string dur = "", string genomes = "")
        {
            GeneratedLevel = level;
            LevelGenerationDuration = dur;
            UsedGenomes = genomes;
            SetParent(levelParent);
        }

        public void SetParent(GameObject levelParent)
        {
            _parentGameObject = levelParent;
        }

        public List<LevelCell> GenerateLevelList()
        {
            LevelBlueprint = new List<LevelCell>();

            if (GeneratedLevel != null)
            {
                for (var cellCounter = 0; cellCounter < GeneratedLevel.Count; cellCounter++)
                {
                    if (GeneratedLevel[cellCounter].Contains("way-cell-group"))
                    {
                        int numberOfWays;
                        GetCellSettings(GeneratedLevel[cellCounter], out numberOfWays);

                        var patternList = new List<LevelPattern>();

                        for (var patternCounter = 1; patternCounter <= numberOfWays; patternCounter++)
                        {
                            if (cellCounter + patternCounter < GeneratedLevel.Count)
                            {
                                var rawPatternString = GeneratedLevel[cellCounter + patternCounter];
                                var patternAttributes = StringHelper.GetTokenAttributes(rawPatternString);

                                rawPatternString = StringHelper.GetTokenNameWithoutAttributes(rawPatternString);
                                patternList.Add(new LevelPattern(rawPatternString, patternAttributes));
                            }
                        }
                        LevelBlueprint.Add(new LevelCell(numberOfWays, numberOfWays + "-way-cell-group", patternList));
                    }
                }
            }
            return LevelBlueprint;
        }

        public void SetupLevelObjects()
        {
            _parentGameObject.transform.position = Vector3.zero;
            var cells = GenerateLevelList();

            foreach (var cell in cells)
            {
                var cellObj = GenerateParent(_parentGameObject, cell.NumberOfWays);
                cell.CellObject = cellObj;

                foreach (var pattern in cell.Patterns)
                {
                    var patternObj = InstatiatePrefabs(cellObj, pattern.Name, Vector3.zero);
                    pattern.GameObject = patternObj;
                }
            }

            DistributeCells(LevelBlueprint);

            SetLevelData();
            _parentGameObject.transform.position = ParentPosition;
        }

        private void DistributeCells(List<LevelCell> list)
        {
            _levelBounds = new Bounds();
            var lastCellYPosition = 0f;
        
            for (int index = 0; index < list.Count; index++)
            {
                var spacingX = index == 0 ? 0f : Random.Range(MinMaxHorizontal.x, MinMaxHorizontal.y);

                var cell = list[index];

                DistributePatterns(cell.CellObject, cell.Patterns);

                if (VerticalConstruction)
                {
                    var groundToTopHeight = index == 0 ? 0 : -cell.CellObject.transform.position.y - GameObjectHelper.CalculateBoundsWithChildren(cell.CellObject, "", ExcludeTag).min.y;

                    if (AnimateCells)
                    {
                        cell.CellObject.AddComponent<CellAnimator>().Setup(
                            gameObject.GetComponent<GeneratorData>(),
                            new Vector3(0 - AnimationOffset, _levelBounds.max.y + spacingX + groundToTopHeight, 0),
                            new Vector3(0, _levelBounds.max.y + spacingX + groundToTopHeight, 0) + ParentPosition,
                            AnimationDuration,
                            true);
                    }
                    else
                    {
                        cell.CellObject.transform.position = new Vector3(0, _levelBounds.max.y + spacingX + groundToTopHeight, 0);
                    }
                }
                else
                {
                    if (AnimateCells)
                    {
                        cell.CellObject.AddComponent<CellAnimator>().Setup(
                            gameObject.GetComponent<GeneratorData>(), 
                            new Vector3(_levelBounds.max.x + spacingX, lastCellYPosition - AnimationOffset, 0), 
                            new Vector3(_levelBounds.max.x + spacingX, lastCellYPosition, 0) + ParentPosition, 
                            AnimationDuration, 
                            false);
                    }
                    else
                    {
                        cell.CellObject.transform.position = new Vector3(_levelBounds.max.x + spacingX, lastCellYPosition, 0);
                    }
                }

                var groundPattern = cell.Patterns.FirstOrDefault();
                if (groundPattern != null)
                {
                    lastCellYPosition = GroundCells ? 0 : GetLastYPositionOfCell(groundPattern.GameObject, 0.5f).y;
                }
                if (!string.IsNullOrEmpty(LayerName))
                {
                    GameObjectHelper.ChangeGameObjectLayerRecursive(cell.CellObject, LayerName);
                }
                _levelBounds.Encapsulate(GameObjectHelper.CalculateGlobalBoundsOfChildren(cell.CellObject, "", ExcludeTag));
            }
        }

        public void DistributePatterns(GameObject cell, List<LevelPattern> patterns)
        {
            var cellBounds = new Bounds();

            for (int index = 0; index < patterns.Count; index++)
            {
                var pattern = patterns[index].GameObject;
                var spacingY = index == 0 ? 0 : Random.Range(MinMaxVertical.x, MinMaxVertical.y);

                if (index == 0)
                {
                    pattern.transform.position = new Vector3(0, 0, 0);
                }
                else
                {
                    var groundToTopHeight = index == 0 ? 0 : -pattern.transform.position.y - GameObjectHelper.CalculateBoundsWithChildren(pattern).min.y;
                    pattern.transform.position = new Vector3(0, cellBounds.max.y + spacingY + groundToTopHeight, 0);
                }

                cellBounds.Encapsulate(GameObjectHelper.CalculateGlobalBoundsOfChildren(cell, "", ExcludeTag));
            }
        }

        public void SetLevelData()
        {
            _generatorData = gameObject.GetComponent<GeneratorData>();
            _generatorData.Bounds = _levelBounds;
            _generatorData.Center = _levelBounds.center;
            _generatorData.MinX = _levelBounds.min.x;
            _generatorData.MinY = _levelBounds.min.y;
            _generatorData.MaxX = _levelBounds.max.x;
            _generatorData.MaxY = _levelBounds.max.y;
            _generatorData.CellCount = LevelBlueprint.Count;
            _generatorData.ParentPosition = ParentPosition;
        }

        private Vector3 GetLastYPositionOfCell(GameObject obj, float offset)
        {
            var initalLayer = obj.layer;
            GameObjectHelper.ChangeGameObjectLayerRecursive(obj, "tempRaycastLayer");
            var bounds = GameObjectHelper.CalculateGlobalBoundsOfChildren(obj, "", ExcludeTag);

            var uses2DCollider = gameObject.GetComponent<GeneratorData>().TwoDimensional;
            if (uses2DCollider)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(bounds.max.x - offset, bounds.max.y + 100),
                    new Vector2(0, -1), Mathf.Infinity, 1 << LayerMask.NameToLayer("tempRaycastLayer"));
                GameObjectHelper.ChangeGameObjectLayerRecursive(obj, initalLayer);
                if (hit.collider != null)
                {
                    return hit.point;
                }
            }
            else
            {
                var ray = new Ray();
                RaycastHit hit;
                ray.origin = new Vector3(bounds.max.x - offset, bounds.max.y + 100, bounds.center.z);
                ray.direction = new Vector3(0, -1, 0);

                if (Physics.Raycast(ray, out hit))
                {
                    return hit.point;
                }
            }
            
            return Vector3.zero;
        }

        private GameObject GenerateParent(GameObject level, int numberOfWays)
        {
            var parent = new GameObject {name = numberOfWays + "way-cell-group"};
            GameObject parentObject = level ?? GameObjectHelper.CreateEmptyPCGParent(gameObject.GetComponent<GeneratorData>().ParentName);
            parent.transform.parent = parentObject.transform;
            parent.transform.position = new Vector3(0, 0, 0);
            return parent;
        }

        private void GetCellSettings(string inputToken, out int numberOfWays)
        {
            numberOfWays = int.Parse(Regex.Replace(inputToken, @"[\D]", string.Empty));
        }

        private GameObject InstatiatePrefabs(GameObject parent, string pathString, Vector3 pos)
        {
            var path = StringHelper.ExceptCharsFromString(pathString, new[] {'<', '>'});
            var obj = Resources.Load("Levelgenerator Prefabs" + "/" + path);
            if (obj != null)
            {
                var gObj = (GameObject) Instantiate(obj, pos, new Quaternion());
                gObj.name = path;
                foreach (var gRenderer in gObj.GetComponentsInChildren<Renderer>())
                {
                    gRenderer.gameObject.tag = "pcgplatform";
                }
                gObj.tag = "pcgplatform";
                gObj.transform.parent = parent.transform;
                return gObj;
            }

            throw new Exception("Could not instantiate object: " + pathString);
        }

        public void SaveLevel()
        {
#if UNITY_EDITOR
            var path = EditorUtility.SaveFilePanel("Save Content", "", gameObject.GetComponent<GeneratorData>().ParentName + DateTime.Now.GetHashCode(), "txt");

            var sb = new StringBuilder();

            for (var i = 0; i < GeneratedLevel.Count; i++)
            {
                sb.Append(JsonUtility.ToJson(new KeyValueString(i.ToString(), GeneratedLevel[i])));
                if (i < GeneratedLevel.Count - 1)
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

        public void LoadLevel()
        {
#if UNITY_EDITOR
            var path = EditorUtility.OpenFilePanel("Choose Content File", "", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                var fileText = File.ReadAllText(path);
                var linesInFile = fileText.Split('\n');

                GeneratedLevel.Clear();
                LevelGenerationDuration = "";
                UsedGenomes = "";

                foreach (var line in linesInFile)
                {
                    var obj = JsonUtility.FromJson<KeyValueString>(line);
                    GeneratedLevel.Add(obj.value);
                }
                Setup(GeneratedLevel, GameObjectHelper.CreateEmptyPCGParent(gameObject.GetComponent<GeneratorData>().ParentName));
            }
#endif
        }
    }

    public class LevelCell
    {
        public string CellName;
        public GameObject CellObject;
        public int CombinedDifficulty;
        public int NumberOfWays;
        public List<LevelPattern> Patterns;

        public LevelCell(int numberOfWays = 0, string name = "", List<LevelPattern> patterns = null,
            int combinedDif = 0)
        {
            NumberOfWays = numberOfWays;
            CellName = name;
            Patterns = patterns;
            CombinedDifficulty = combinedDif;
        }
    }

    public class LevelPattern
    {
        public List<KeyValueString> Attributes;
        public float Difficulty;
        public GameObject GameObject;
        public string Name;

        public LevelPattern()
        {
        }

        public LevelPattern(string name, float difficulty = 0)
        {
            Name = name;
            Difficulty = difficulty;
        }

        public LevelPattern(string name, List<KeyValueString> attributes = null)
        {
            Name = name;
            Attributes = attributes;
        }
    }
}