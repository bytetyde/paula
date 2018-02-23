using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Populator;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Craft
{
    public class MainGenerator : MonoBehaviour
    {
        public List<GameObject> Generators;
        public GameObject LengthSlider;
        public GameObject LayerSlider;
        private Stopwatch _stopwatch;
        public Text HashInputText;
        private int _hashValue;
        private List<KeyValuePair<string, int>> _initialPopulationGroups = new List<KeyValuePair<string, int>>();

        void Awake()
        {
            var levelpop = Generators.Find(item => item.gameObject.name == "Levelgenerator").GetComponent<LevelPopulator>();
            foreach (var item in levelpop.GrammarItems)
            {
                _initialPopulationGroups.Add(new KeyValuePair<string, int>(item.Key, item.TargetObjectCount));
            }
        }

        public void Start()
        {
            _stopwatch = new Stopwatch();
            Application.targetFrameRate = 60;
            Generate();
        }

        public void Generate()
        {
            var length = LengthSlider.GetComponent<Slider>().value;
            var layerCount = LayerSlider.GetComponent<Slider>().value;

            _hashValue = HashInputText.text.ToLower() == ""
                ? (int) DateTime.Now.Ticks
                : StringHelper.CalculateHash(HashInputText.text);

            _stopwatch.Reset();
            _stopwatch.Start();
            GameObject.Find("ResetPlayer").gameObject.GetComponent<ResetPlayer>().ResetAll();
            foreach (var generator in Generators)
            {
                var grammar = generator.GetComponent<StringParser>();
                var populator = generator.GetComponent<LevelPopulator>();
                grammar.StartSeed = (int)_hashValue;
            
                switch (generator.gameObject.name)
                {
                    case "Levelgenerator":
                        grammar.StartString = "<left-restriction><start>{" + length +"*<section>}<end><right-restriction>";
                        SetPopulationCount(populator.GrammarItems, (int)length);
                        var tokenString = grammar.InputList.FirstOrDefault(item => item.KeyString == "<section>");
                        if (tokenString != null)
                            tokenString.ValueString = GetSectionLayer((int)layerCount);
                        break;
                    case "Level_1_Background_Generator":
                        grammar.StartString = "{" + (14 + Mathf.RoundToInt(length * 4.35f)) + "*<image>}";
                        break;
                    case "Level_2_Background_Generator":
                        grammar.StartString = "{" + (12 + Mathf.RoundToInt(length * 3.4f)) + "*<image>}";
                        break;
                    case "Level_3_Background_Generator":
                        grammar.StartString = "{" + (11 + Mathf.RoundToInt(length * 3.05f)) + "*<image>}";
                        break;
                    case "Level_4_Background_Generator":
                        grammar.StartString = "{" + (11 + Mathf.RoundToInt(length * 2.50f)) + "*<image>}";
                        break;
                    case "Level_5_Background_Generator":
                        grammar.StartString = "{" + (11 + Mathf.RoundToInt(length * 2.2f)) + "*<image>}";
                        break;
                    default:
                        break;
                }
                generator.GetComponent<LevelGenerator>().Generate();

            }
            _stopwatch.Stop();
        }

        private string GetSectionLayer(int count)
        {
            switch (count)
            {
                case 1:
                    return "<one-pattern-cell> | <one-way-compound>";
                case 2:
                    return "<one-pattern-cell> | <two-pattern-cell> | <two-way-compound> | <one-way-compound>";
                case 3:
                    return "<one-pattern-cell> | <two-pattern-cell> | <three-pattern-cell> | <three-way-compound> | <two-way-compound> | <one-way-compound>";
                case 4:
                    return "<one-pattern-cell> | <two-pattern-cell> | <three-pattern-cell>  |<four-pattern-cell> | <three-way-compound> | <two-way-compound> | <one-way-compound>";
                default:
                    return "<one-pattern-cell> | <two-pattern-cell> | <three-pattern-cell>  |<four-pattern-cell> | <three-way-compound> | <two-way-compound> | <one-way-compound>";
            }
        }

        private void SetPopulationCount(List<KeyValueCurveGroup> grammarItems, int length)
        {
            foreach (var group in grammarItems)
            {
                var initalVal = _initialPopulationGroups.Where(item => item.Key == group.Key).Select(item => item.Value).FirstOrDefault();
                group.TargetObjectCount = Mathf.RoundToInt(initalVal * GameObjectHelper.Map(length, LengthSlider.GetComponent<Slider>().minValue, LengthSlider.GetComponent<Slider>().maxValue, 0.5f, 1.5f));
            }   
        }

        public TimeSpan GetStopwatchValue()
        {
            return _stopwatch.Elapsed;
        }

        public string GetHashValue()
        {
            return _hashValue.ToString();
        }
    }
}
