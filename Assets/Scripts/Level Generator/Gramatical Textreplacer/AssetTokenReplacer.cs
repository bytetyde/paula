using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Logging;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generator.Gramatical_Textreplacer
{
    public class AssetTokenReplacer : ITokenReplacer
    {
        private int _seed;
        private List<string> _usedGenomes;
        private XXHash _xxHash;
        private Queue<int> genome;
        private int genomeCounter;
        private bool removeAfterNextToken;

        public void QueueSetup(XXHash hash, int seed, List<string> usedGenomes, Queue<int> curGenome, int genomeCount)
        {
            _xxHash = hash;
            _seed = seed;
            _usedGenomes = usedGenomes;
            genome = curGenome;
            genomeCounter = genomeCount;
        }

        public void ReplaceToken(Dictionary<string, List<List<string>>> lvlGrammar, int recursionCount, List<string> list,
            string tokenToReplace)
        {
            LogLocator.GetLogger().InsertNewSection("Token Replacing started");
            var index = list.IndexOf(tokenToReplace);
            LogLocator.GetLogger().LogLine("Index of Token:", index);
            var codonValue = GetNumberOfQueue();
            LogLocator.GetLogger().LogLine("Codon Value:", codonValue);

            list.RemoveAt(index);
            var token = StringHelper.GetTokenNameWithoutAttributes(tokenToReplace);

            if (lvlGrammar[token].Count > 0)
            {
                var codon = codonValue%lvlGrammar[token].Count;

                var replacement = lvlGrammar[token][codon];

                //Removes the position token if the <spawn-position> token has no attribute stackable, 
                //which was determined in the replacement before and set with the removeAfterNextToken boolean 
                if (token == "<spawn-position>" && removeAfterNextToken)
                {
                    LogLocator.GetLogger().LogLine("Spawnposition:", lvlGrammar[tokenToReplace][codon], "was removed");
                    lvlGrammar[tokenToReplace].RemoveAt(codon);
                    removeAfterNextToken = false;
                }

                //Go trough token in replacement and set removeAfterNextToken for the <spawn-position> token
                //The next time <spawn-position> will be replaced with a position and the position will be removed if removeAfterNextToken is true
                for (var i = 0; i < replacement.Count; i++)
                {
                    var item = replacement[i];

                    if (StringHelper.GetTokenNameWithoutAttributes(item) == "<spawn-position>")
                    {
                        LogLocator.GetLogger().LogLine("Token:", item, "is <spwan-position>");

                        if (!StringHelper.HasTokenAttribute(item, "stackable"))
                        {
                            LogLocator.GetLogger()
                                .LogLine("Token has no Attribute \"stackable\"",
                                    "spawn-position will be removed after next spawn-position replacement");
                            removeAfterNextToken = true;
                        }
                        else
                        {
                            removeAfterNextToken = false;
                        }
                    }
                    else
                    {
                        LogLocator.GetLogger().LogLine("Token:", item, "is Custom Token", "nothing was exchanged");
                    }
                }

                list.InsertRange(index, replacement);
                LogLocator.GetLogger()
                    .LogLine("Insert Token: ", StringHelper.GetListEntriesAsString(replacement), "at Index", index);
            }
        }

        public List<string> ParseTokenList(int recursionCount, List<string> list)
        {
            throw new NotImplementedException();
        }

        public void ParseToken(int recursionCount, List<string> tokenList)
        {
            throw new NotImplementedException();
        }

        public bool IsOptionalToken(string token)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfQueue()
        {
            var hash = _xxHash.GetHash(_seed);
            Random.seed = (int) hash;

            var codonInteger = genome.Dequeue();
            var codonValue = codonInteger == -1 ? Random.Range(0, 100) : codonInteger;
            _usedGenomes.Add(codonValue.ToString());
            genome.Enqueue(codonInteger);
            genomeCounter++;

            return codonValue;
        }

        public void EvaluateRecurrenceToken(List<string> tokenList, string token)
        {
            throw new NotImplementedException();
        }

        public int GetCalculationNumberOfToken(string token)
        {
            throw new NotImplementedException();
        }

        public bool IsRecurrenceToken(string token)
        {
            throw new NotImplementedException();
        }

        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        public bool GrammarContainsKey(Dictionary<string, List<List<string>>> levelGrammar, string tokenString)
        {
            return levelGrammar.ContainsKey(StringHelper.GetTokenNameWithoutAttributes(tokenString));
        }
    }
}