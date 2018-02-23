using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Common
{
    public static class StringHelper
    {
        public static List<string> GetMatchingStringPartsInString(string parts, string regexRule)
        {
            var gTpat = regexRule;
            var tR = new Regex(gTpat, RegexOptions.IgnoreCase);

            var matchesList = new List<string>();
            var tRresults = tR.Matches(parts);

            if (tRresults.Count <= 0) return matchesList;
            foreach (var match in tRresults)
            {
                matchesList.Add(match.ToString());
            }

            return matchesList;
        }

        public static int CalculateHash(string val)
        {
            return val.GetHashCode();
        }

        public static string GetListEntriesAsString<T>(List<T> list, string startAppendix = "", string endAppendix = "",
            string delimiter = " ")
        {
            var sb = new StringBuilder();

            for (var i = 0; i < list.Count; i++)
            {
                sb.Append(startAppendix);
                sb.Append(list[i]);
                sb.Append(endAppendix);
                if (i < list.Count - 1)
                {
                    sb.Append(delimiter);
                }
            }
            return sb.ToString();
        }

        public static string ExceptCharsFromString(string str, char[] chars)
        {
            var sb = new StringBuilder(str.Length);

            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (!chars.Contains(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static List<string> SeparateStringIntoPartsBySingleDelimiter(string str, char delimiter)
        {
            return str == null ? new List<string>() : str.Split(delimiter).ToList();
        }

        public static int ExtractIntValueFromString(string str)
        {
            var sb = new StringBuilder(str.Length);
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsDigit(c))
                    sb.Append(c);
            }

            return Convert.ToInt32(sb.ToString());
        }

        public static string StringToLowerCase(string str)
        {
            var input = str.ToLower();

            return input;
        }


        public static List<string> SeparateStringIntoPartsByDelimiterGroup(string inputString, string[] delimiters)
        {
            var list = inputString.Split(delimiters, StringSplitOptions.None).ToList();

            return list;
        }

        public static List<string> CloneStringList(List<string> list = null)
        {
            var copy = new List<string>();

            if (list == null)
            {
                return new List<string>();
            }

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    copy.Add(list[i]);
                }
            }
            return copy;
        }

        public static string GetFirstTokenInString(string input, int depth = 0)
        {
            var sbBuilder = new StringBuilder();
            var openBracketCounter = 0;
            char[] openingBracketChars = {'[', '{', '<'};
            char[] closingBracketChars = {']', '}', '>'};

            foreach (var c in input)
            {
                if (openingBracketChars.Contains(c))
                    openBracketCounter++;
                if (openBracketCounter > depth)
                    sbBuilder.Append(c);
                if (closingBracketChars.Contains(c))
                    openBracketCounter--;
                if (openBracketCounter == 0 && sbBuilder.Length > 0)
                    return sbBuilder.ToString();
            }
            return "";
        }

        public static string GetChildTokenInToken(string token)
        {
            return GetFirstTokenInString(token, 1);
        }

        public static List<string> GetAllTokensInString(string inputTokens)
        {
            var input = inputTokens;
            var tokenList = new List<string>();

            while (GetFirstTokenInString(input) != "")
            {
                var token = GetFirstTokenInString(input);
                tokenList.Add(token);

                var index = input.IndexOf(token);
                input = index < 0 ? input : input.Remove(index, token.Length);
            }

            return tokenList;
        }

        public static string GetFirstRegexMatchInString(string input, string regexRule)
        {
            var regex = new Regex(regexRule);

            return regex.Match(input).Value;
        }

        public static string GetTokenNameWithoutAttributes(string input)
        {
            var result = input;
            var attributes = GetMatchingStringPartsInString(input, @"\s[^<>]+?='.*?'");
            if (attributes.Count > 0)
            {
                result = input.Replace(GetListEntriesAsString(attributes, "", "", ""), "");
            }
            return result;
        }

        public static List<KeyValueString> GetTokenAttributes(string input)
        {
            var attributeList = new List<KeyValueString>();

            var attributes = GetMatchingStringPartsInString(input, @"\s[^<>]+?='.*?'");
            foreach (var attribute in attributes)
            {
                var key = GetFirstRegexMatchInString(attribute, @"\s[^<>=']+");
                var value = GetFirstRegexMatchInString(attribute, @"='.*?'");

                key = ExceptCharsFromString(key, new[] {' '});
                value = ExceptCharsFromString(value, new[] {' ', '\'', '='});
                attributeList.Add(new KeyValueString(key, value));
            }

            return attributeList;
        }

        public static bool HasTokenAttribute(string input, string attributeName, string attributeValue = "yes")
        {
            var attributes = GetTokenAttributes(input);

            return attributes.Any(attribute => attribute.key == attributeName && attribute.value == attributeValue);
        }
    }
}