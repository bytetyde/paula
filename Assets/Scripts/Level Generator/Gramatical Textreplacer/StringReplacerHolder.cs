using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Logging;
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Gramatical_Textreplacer
{
    public enum TokenType
    {
        Default,
        Recurrence,
        Optional,
        Undefined
    }

    public class StringReplacerHolder : MonoBehaviour
    {
        private readonly StringReplacer _stringReplacer = new StringReplacer();
        [HideInInspector] public string _generationDuration;
        [HideInInspector] public string _usedCodonNumbersString;

        public bool logEvolution = false;
        public bool logEvolutionToFile = false;
        [HideInInspector] public List<string> StringList;
        [HideInInspector] public ITokenReplacer TokenReplacer;

        public void Setup(ITokenReplacer tokenReplacer, XXHash xxHash, int seed, Dictionary<string, List<List<string>>> dict,
            List<string> strgList, List<int> codonList)
        {
            _stringReplacer.Setup(tokenReplacer, xxHash, seed, dict, strgList, codonList);

            LogLocator.GetLogger().InsertNewSection("Text Replacing Setup");
            LogLocator.GetLogger().LogLine(tokenReplacer, dict, strgList, codonList);
        }

        public void StartTextReplacing()
        {
            LogLocator.GetLogger().InsertNewSection("Text Replacing Started");
            _stringReplacer.StartTextReplacing();
            LogLocator.GetLogger().InsertNewSection("Text Replacing finished in " + _stringReplacer.GetGenerationDuration());
            LogLocator.GetLogger().LogLine("Used Genomes: " + _stringReplacer.GetUsedCodonNumberString());
            LogLocator.GetLogger().InsertSeparator();
            _generationDuration = _stringReplacer.GetGenerationDuration();
            _usedCodonNumbersString = _stringReplacer.GetUsedCodonNumberString();
        }

        public List<string> GetGeneratedLevel()
        {
            return _stringReplacer.GetGeneratedLevel();
        }
    }
}