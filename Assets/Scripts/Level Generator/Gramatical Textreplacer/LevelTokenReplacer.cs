using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Logging;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generator.Gramatical_Textreplacer
{
    public class LevelTokenReplacer : ITokenReplacer
    {
        private int _seed;
        private List<string> _usedGenomes;
        private XXHash _xxHash;
        private Queue<int> genome;
        private int genomeCounter;

        public void QueueSetup(XXHash hash, int seed, List<string> usedGenomes, Queue<int> curGenome, int genomeCount)
        {
            _xxHash = hash;
            _seed = seed;
            _usedGenomes = usedGenomes;
            genome = curGenome;
            genomeCounter = genomeCount;

            LogLocator.GetLogger().InsertSeparator();
            LogLocator.GetLogger()
                .LogLine("Queue Settings", "Used Genomes: ", StringHelper.GetListEntriesAsString(usedGenomes),
                    "Generation Genomes: ", StringHelper.GetListEntriesAsString(curGenome.ToList()), "Genome Count: ",
                    genomeCount);
        }

        public void ReplaceToken(Dictionary<string, List<List<string>>> lvlGrammar, int recursionCount, List<string> list,
            string tokenToReplace)
        {
            LogLocator.GetLogger().LogLine("Token to Replace:", tokenToReplace);

            var index = list.IndexOf(tokenToReplace);
            LogLocator.GetLogger().LogLine("Index of Token:", index);

            var codonValue = GetNumberOfQueue();
            LogLocator.GetLogger().LogLine("Codon Value:", codonValue);

            list.RemoveAt(index);

            if (lvlGrammar[tokenToReplace].Count > 0)
            {
                var codon = codonValue%lvlGrammar[tokenToReplace].Count;

                var replacement = ParseTokenList(recursionCount, lvlGrammar[tokenToReplace][codon]);

                list.InsertRange(index, replacement);
                LogLocator.GetLogger()
                    .LogLine("Insert Replacement: ", StringHelper.GetListEntriesAsString(replacement), "at Index", index);
            }
        }

        public List<string> ParseTokenList(int recursionCount, List<string> list)
        {
            var tokenList = StringHelper.CloneStringList(list);
            LogLocator.GetLogger().InsertNewSection("Token Replacing started");
            ParseToken(recursionCount, tokenList);

            return tokenList;
        }

        public void ParseToken(int recursionCount, List<string> tokenList)
        {
            LogLocator.GetLogger().LogLine("Token Parsing Started");

            recursionCount++;
            for (var i = 0; i < tokenList.Count; i++)
            {
                var token = tokenList[i];
                LogLocator.GetLogger().LogLine("Replacement is", token);

                var tokenType = GetTokenType(token);
                switch (tokenType)
                {
                    case TokenType.Default:
                        LogLocator.GetLogger().LogLine("Token", token, "Type:", "Default");
                        break;
                    case TokenType.Recurrence:
                        LogLocator.GetLogger().LogLine("Token", token, "Type:", "Recurrence");
                        EvaluateRecurrenceToken(tokenList, token);
                        i--;
                        break;
                    case TokenType.Optional:
                        LogLocator.GetLogger().LogLine("Token", token, "Type:", "Optional");
                        EvaluateOptionalToken(tokenList, token);
                        i--;
                        break;
                    case TokenType.Undefined:
                        LogLocator.GetLogger().LogLine("Token", token, "Type:", "Undefined");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            for (var i = 0; i < tokenList.Count; i++)
            {
                if (IsOptionalToken(tokenList[i]) || IsRecurrenceToken(tokenList[i]))
                {
                    LogLocator.GetLogger().LogLine("Recursion Token Parsing Started");

                    if (recursionCount < 20)
                    {
                        ParseToken(recursionCount, tokenList);
                    }
                    else
                    {
                        LogLocator.GetLogger().InsertNewSection("Max Recursion size reached", "Stopped Parsing");

                        Debug.Log("Max Recursion size reached");
                    }
                }
            }
            recursionCount--;
        }

        public bool IsOptionalToken(string token)
        {
            return Regex.IsMatch(token, @"\[\s?\d+%.*?]");
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
            var number = GetCalculationNumberOfToken(token);

            for (var i = 0; i < number; i++)
            {
                tokenList.Insert(tokenList.IndexOf(token), StringHelper.GetChildTokenInToken(token));
            }
            LogLocator.GetLogger()
                .LogLine("Recurrence Token evaluated", "Count", number, "Token:", StringHelper.GetChildTokenInToken(token));

            tokenList.Remove(token);
        }

        public int GetCalculationNumberOfToken(string token)
        {
            var regex = new Regex(@"[[{]\s?\d*\s?[\*%]\s?");
            var match = regex.Match(token);

            return match.Success ? int.Parse(Regex.Replace(match.Value, @"[\D]", string.Empty)) : 0;
        }

        public bool IsRecurrenceToken(string token)
        {
            return Regex.IsMatch(token, @"{\s?\d+\*(.*?)}");
        }

        public TokenType GetTokenType(string token)
        {
            var openingBracket = token[0];
            var closingBracket = token[token.Length - 1];

            char[] optionalToken = {'[', ']'};
            char[] recurrenceToken = {'{', '}'};
            char[] defaultToken = {'<', '>'};

            if (optionalToken.Contains(openingBracket) && optionalToken.Contains(closingBracket))
            {
                return TokenType.Optional;
            }
            if (recurrenceToken.Contains(openingBracket) && recurrenceToken.Contains(closingBracket))
            {
                return TokenType.Recurrence;
            }
            if (defaultToken.Contains(openingBracket) && defaultToken.Contains(closingBracket))
            {
                return TokenType.Default;
            }
            return TokenType.Undefined;
        }

        public bool GrammarContainsKey(Dictionary<string, List<List<string>>> levelGrammar, string tokenString)
        {
            return levelGrammar.ContainsKey(tokenString);
        }

        public void EvaluateOptionalToken(List<string> tokenList, string token)
        {
            Random.seed = GetNumberOfQueue();
            var tokenNumber = GetCalculationNumberOfToken(token);
            var randomNumber = Random.Range(0, 100);
            if (randomNumber < tokenNumber)
            {
                tokenList.Insert(tokenList.IndexOf(token), StringHelper.GetChildTokenInToken(token));
            }
            LogLocator.GetLogger()
                .LogLine("Optional Token evaluated", "Random Number", randomNumber, "Token Probability Number", tokenNumber,
                    "Token:", StringHelper.GetChildTokenInToken(token));

            tokenList.Remove(token);
        }
    }
}