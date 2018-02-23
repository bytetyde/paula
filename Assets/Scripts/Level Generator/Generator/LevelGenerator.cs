using System;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Gramatical_Textreplacer;
using Assets.Scripts.Level_Generator.Infinite;
using Assets.Scripts.Level_Generator.Instantiater;
using Assets.Scripts.Level_Generator.Logging;
using Assets.Scripts.Level_Generator.Populator;
using Assets.Scripts.Level_Generator.Setup;
using Assets.Scripts.Level_Generator.Stats;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Generator
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(GeneratorData))]
    [RequireComponent(typeof(PCGSetup))]
    [RequireComponent(typeof (InfiniteLevelSettings))]
    [RequireComponent(typeof (StringParser))]
    [RequireComponent(typeof (StringReplacerHolder))]
    [RequireComponent(typeof (LevelInstantiater))]
    [RequireComponent(typeof (LevelPopulator))]
    public class LevelGenerator : MonoBehaviour
    {
        private StringReplacerHolder _trHolder;
        private InfiniteLevelSettings _infiniteLevel;
        private GameObject _levelParentObject;
        private LevelInstantiater LevelInstantiater;
        private LevelPopulator LevelPopulator;
        public bool LogGeneration;

        private void Awake()
        {
            LogLocator.ProvideLog(new NullLog());
            LogGeneration = false;
            StatsLocator.ProvideLevelPopulationStats(new LevelPopulationLog());
            StatsLocator.ProvideLevelGenerationLog(new LevelGenerationLog());
            _infiniteLevel = gameObject.GetComponent<InfiniteLevelSettings>();
            _trHolder = gameObject.GetComponent<StringReplacerHolder>();
            LevelInstantiater = gameObject.GetComponent<LevelInstantiater>();
            LevelPopulator = gameObject.GetComponent<LevelPopulator>();
        }

        // Use this for initialization
        private void Start()
        {
            InfiniteLevelSetup();
        }

        public void SaveLevel()
        {
            var levelSaver = new SaveLevel();
            levelSaver.SaveLevelToJson(gameObject.GetComponent<GeneratorData>().ParentName);
        }

        public void ChangeLog()
        {
            LogGeneration = !LogGeneration;

            if (LogGeneration)
            {
                LogLocator.ProvideLog(new GenerationLog());
            }
            else
            {
                LogLocator.ProvideLog(new NullLog());
            }
        }


        private void InfiniteLevelSetup()
        {
            if (_infiniteLevel.Infinite)
            {
                SetupLevelParentObject();
                _infiniteLevel.Setup(_levelParentObject);
            }
        }

        public void SetupLevelParentObject()
        {
            _levelParentObject = GameObjectHelper.CreateEmptyPCGParent(gameObject.GetComponent<GeneratorData>().ParentName);
            LogLocator.GetLogger().LogLine("Level GameObject created");
        }

        public void Generate()
        {
            LogLocator.GetLogger().LogLine("GENERATION STARTET [ " + DateTime.Now + " ]");

            SetupLevelParentObject();
            StatsLocator.ProvideLevelPopulationStats(new LevelPopulationLog());
            StatsLocator.ProvideLevelGenerationLog(new LevelGenerationLog());
            StatsLocator.GetLevelGenerationLog().StartGenerationStopwatch();

            var stringParser = gameObject.GetComponent<StringParser>();
            var dictionary = stringParser.GenerateTokenDictionary(stringParser.InputList);
            var startString = stringParser.GetStartString();
            var codonList = stringParser.GetCodonList();

            var seed = stringParser.StartSeed == -1 ? (int) DateTime.Now.Ticks : stringParser.StartSeed;
            _trHolder.Setup(new LevelTokenReplacer(), new XXHash(seed), seed, dictionary, startString, codonList);
            _trHolder.StartTextReplacing();

            if (LevelPopulator.PopulateLevel)
            {
                LevelInstantiater.Setup(_trHolder.GetGeneratedLevel(), _levelParentObject, _trHolder._generationDuration,
                    _trHolder._usedCodonNumbersString);
                LevelPopulator.StartPopulation(new XXHash(seed), false, LevelPopulator.GrammarItems, GameObjectHelper.CreateEmptyPCGParent(gameObject.GetComponent<GeneratorData>().ParentName));
            }
            else
            {
                LevelInstantiater.Setup(_trHolder.GetGeneratedLevel(), _levelParentObject, _trHolder._generationDuration,
                    _trHolder._usedCodonNumbersString);
                LevelInstantiater.SetupLevelObjects();
            }

            StatsLocator.GetLevelGenerationLog().StopGenerationStopwatch();
            LogLocator.GetLogger().InsertNewSection("FINISHED");
            LogLocator.GetLogger().LogLine("GENERATION FINISHED [ " + DateTime.Now + " ]");
            LogLocator.GetLogger()
                .LogLine("Generation Duration:", StatsLocator.GetLevelGenerationLog().GenerationDuration, "ms");
            LogLocator.GetLogger().SaveLog();
        }

        // Update is called once per frame
        private void Update()
        {
            if (LogLocator.GetLogger() == null)
            {
                LogLocator.ProvideLog(new NullLog());
            }

            if (StatsLocator.GetLevelPopulationLog() == null)
            {
                StatsLocator.ProvideLevelPopulationStats(new LevelPopulationLog());
            }

            if (StatsLocator.GetLevelGenerationLog() == null)
            {
                StatsLocator.ProvideLevelGenerationLog(new LevelGenerationLog());
            }
        }

        public void LoadLevel()
        {
            var levelLoader = gameObject.GetComponent<LoadLevel>() ?? gameObject.AddComponent<LoadLevel>();

            levelLoader.LoadLevelFromJson();
        }
    }
}