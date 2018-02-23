using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Craft
{
    public class GameStatsManager : MonoBehaviour
    {
        public Text CurrentCollectableText;
        public Text MaxCollectableText;
        public Text DurationInformation;
        public Text EnemyInformation;
        public Text CollectableInformation;
        public Text DecorationObjects;
        public Text DiamondCount;
        public Text EnemyCount;
        public Text GenerationDuration;
        public Text HashSeedValue;
        public Text LevelLengthValue;
        public GameObject MainGenerator;
        public GameObject LevelGenerator;

        private Stopwatch _stopwatch;
        private int _currentCollectableCount;
        private int _maxCollectableCount;
        private int _currentEnemyCount;
        private int _maxEnemyCount;
        private MainGenerator _mainGenerator;
        private GeneratorData _generatorData;

        void Awake()
        {
            _stopwatch = new Stopwatch();
        }

        // Use this for initialization
        void Start()
        {
            if (_stopwatch == null)
            {
                _stopwatch = new Stopwatch();
            }
            CurrentCollectableText.text = 0.ToString();
            MaxCollectableText.text = 0.ToString();
            _mainGenerator = MainGenerator.GetComponent<MainGenerator>();
            _generatorData = LevelGenerator.GetComponent<GeneratorData>();
        }

        public void ResetStopwatch()
        {
            _stopwatch.Reset();
        }

        public void StartStopwatch()
        {
            _stopwatch.Start();
        }

        public void StopStopwatch()
        {
            _stopwatch.Stop();
        }

        public void IncreaseCurrentEnemyCount(int value = 1)
        {
            _currentEnemyCount += value;
        }

        public void IncreaseCurrentCollectableCount(int value =1)
        {
            _currentCollectableCount += value;
        }

        public void IncreaseMaxEnemyCount(int value = 1)
        {
            _maxEnemyCount += value;
        }

        public void IncreaseMaxCollectableCount(int value = 1)
        {
            _maxCollectableCount += value;
        }

        public int GetCurrentEnemyCount()
        {
            return _currentEnemyCount;
        }

        public int GetCurrentCollectableCount()
        {
            return _currentCollectableCount;    
        }

        public void ResetCurrentCollectableCount()
        {
            _currentCollectableCount = 0;
        }

        public void ResetCurrentEnemyCount()
        {
            _currentEnemyCount = 0;
        }

        public void ResetMaxEnemyCount()
        {
            _maxEnemyCount = 0;
        }

        public void ResetMaxCollectableCount()
        {
            _maxCollectableCount = 0;
        }

        public void ResetAll()
        {
            ResetStopwatch();
            ResetCurrentCollectableCount();
            ResetCurrentEnemyCount();
            ResetMaxCollectableCount();
            ResetMaxEnemyCount();
        }

        // Update is called once per frame
        void Update()
        {
            CurrentCollectableText.text = _currentCollectableCount.ToString();
            MaxCollectableText.text = _maxCollectableCount.ToString();

            string minutes = Mathf.Floor(_stopwatch.Elapsed.Minutes).ToString("00") + "m";
            string seconds = Mathf.RoundToInt(_stopwatch.Elapsed.Seconds).ToString("00") + "s";

            DurationInformation.text = "in " + minutes + " " + seconds;

            EnemyInformation.text = _currentEnemyCount + " von " + _maxEnemyCount + " Gegnern wurden ausgeschalten";

            CollectableInformation.text = _currentCollectableCount + " von " + _maxCollectableCount + " Diamanden wurden eingesammelt";

            var obstacleValue = StatsLocator.GetLevelPopulationLog().ObjectCount.FirstOrDefault(
                item => StringHelper.ExceptCharsFromString(item._Key, new []{'<','>'}) == "obstacles");
            if (obstacleValue != null)
                DecorationObjects.text = obstacleValue._Value.Sum(item => item.Value).ToString();

            DiamondCount.text = _maxCollectableCount.ToString();
            EnemyCount.text = _maxEnemyCount.ToString();
            GenerationDuration.text = (int)_mainGenerator.GetStopwatchValue().TotalMilliseconds + " ms";
            HashSeedValue.text = _mainGenerator.GetHashValue();
            LevelLengthValue.text = ((int)_generatorData.Bounds.size.x).ToString() + " Blöcke";
        }
    }
}
