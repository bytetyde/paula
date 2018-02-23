using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Logging;

namespace Assets.Scripts.Level_Generator.Gramatical_Textreplacer
{
    public class StringReplacer
    {
        private readonly List<string> _usedGenomes = new List<string>();
        private string _generationDuration;
        private int _seed;
        private string _usedCodonNumberString;
        private XXHash _xxHash;
        private int activeTraversals;
        private readonly Queue<int> genome = new Queue<int>();
        private int genomeCounter;
        private Dictionary<string, List<List<string>>> LevelGrammar;
        private readonly int recursionCounter = 0;
        private List<string> StringList;
        private ITokenReplacer TokenReplacer;

        public void Setup(ITokenReplacer tokenReplacer, XXHash xxHash, int seed, Dictionary<string, List<List<string>>> dict,
            List<string> strgList, List<int> codonList)
        {
            TokenReplacer = tokenReplacer;
            activeTraversals = -1;

            LevelGrammar = dict;
            StringList = StringHelper.CloneStringList(strgList);
            SetupStack(codonList);
            _xxHash = xxHash;
            _seed = seed;
        }

        public void SetupStack(List<int> integerList)
        {
            genome.Clear();
            foreach (var i in integerList)
            {
                genome.Enqueue(i);
            }
        }

        public void StartTextReplacing()
        {
            #region Logging Related

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            genomeCounter = 0;
            _usedGenomes.Clear();
            activeTraversals = 0;

            #endregion

            LogLocator.GetLogger().LogLine("Grammar Traversing started");
            TraverseList(StringList);

            stopwatch.Stop();
            _generationDuration = stopwatch.ElapsedMilliseconds + " ms";
            _usedCodonNumberString = StringHelper.GetListEntriesAsString(_usedGenomes);
        }

        public List<string> GetGeneratedLevel()
        {
            return StringList;
        }

        private void TraverseList(List<string> list)
        {
            activeTraversals++;
            for (var i = 0; i < list.Count; i++)
            {
                var tokenString = list[i];

                if (TokenReplacer.GrammarContainsKey(LevelGrammar, tokenString))
                {
                    LogLocator.GetLogger().LogLine("Token: " + tokenString + " is contained");

                    TokenReplacer.QueueSetup(_xxHash, _seed + i, _usedGenomes, genome, genomeCounter);
                    TokenReplacer.ReplaceToken(LevelGrammar, recursionCounter, list, tokenString);
                    TraverseList(list);
                    break;
                }

                LogLocator.GetLogger().LogLine("Token: " + tokenString + " is not contained");
            }
        }

        public string GetGenerationDuration()
        {
            return _generationDuration;
        }

        public string GetUsedCodonNumberString()
        {
            return _usedCodonNumberString;
        }
    }
}