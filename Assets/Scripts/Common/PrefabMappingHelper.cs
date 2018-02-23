using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;

namespace Assets.Scripts.Common
{
    public static class PrefabMappingHelper
    {
        public static List<string> SetupPrefabStringList(List<KeyValueTokenString> list)
        {
            var finalEntries = (from entry in list
                where entry.IsFinalString
                from stringGroup in StringHelper.SeparateStringIntoPartsBySingleDelimiter(entry.ValueString, '|')
                select StringHelper.GetMatchingStringPartsInString(stringGroup, @"<([^<>].*?)>")).ToList();

            return finalEntries.SelectMany(i => i).ToList();
        }

        public static List<TokenPrefabPair> GetPrefabsForTokens(List<string> tokens, string baseFolder)
        {
            var mappedList = new List<TokenPrefabPair>();

            foreach (var token in tokens)
            {
                var withoutWhiteSpaces = StringHelper.ExceptCharsFromString(token, new[] {' '});
                var prefabFolder = StringHelper.ExceptCharsFromString(withoutWhiteSpaces, new[] {'<', '>'});
                var prefabPath = GetPrefabPath(baseFolder, prefabFolder);
                var filePaths = GetPathsForFilesInFolder(prefabPath, prefabFolder, ".prefab");
                var pair = new TokenPrefabPair(withoutWhiteSpaces, filePaths);

                if (!mappedList.Contains(pair))
                {
                    mappedList.Add(pair);
                }
            }

            return mappedList;
        }

        public static List<string> GetPathsForFilesInFolder(string path, string relativePath,
            string fileExtension = ".prefab")
        {
            var filePaths = new List<string>();

            if (path == null) return filePaths;

            var d = new DirectoryInfo(path);
            var Files =
                d.GetFiles("*.*")
                    .Where(
                        file =>
                            string.Equals(file.Extension, fileExtension, StringComparison.InvariantCultureIgnoreCase));

            filePaths.AddRange(Files.Select(file => relativePath + "/" + Path.GetFileNameWithoutExtension(file.Name)));

            return filePaths;
        }

        public static string GetPrefabPath(string _baseFolder, string folder)
        {
            var path = _baseFolder;

            var p = path + folder;
            if (Directory.Exists(p))
            {
                path = path + folder;
            }
            else
            {
                return null;
            }

            return path;
        }
    }
}