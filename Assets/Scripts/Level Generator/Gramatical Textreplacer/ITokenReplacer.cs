using System.Collections.Generic;
using Assets.Scripts.Common;

namespace Assets.Scripts.Level_Generator.Gramatical_Textreplacer
{
    public interface ITokenReplacer
    {
        void QueueSetup(XXHash hash, int seed, List<string> _usedGenomes, Queue<int> genome, int genomeCounter);
        void ReplaceToken(Dictionary<string, List<List<string>>> lvlGrammar, int recursionCount, List<string> list, string tokenToReplace);
        void ParseToken(int recursionCount, List<string> tokenList);
        void EvaluateRecurrenceToken(List<string> tokenList, string token);
        int GetNumberOfQueue();
        int GetCalculationNumberOfToken(string token);
        bool IsOptionalToken(string token);
        bool IsRecurrenceToken(string token);
        bool GrammarContainsKey(Dictionary<string, List<List<string>>> levelGrammar, string tokenString);
        List<string> ParseTokenList(int recursionCount, List<string> list);
        TokenType GetTokenType(string token);
    }
}