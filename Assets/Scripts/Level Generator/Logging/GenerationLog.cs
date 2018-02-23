using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Assets.Scripts.Level_Generator.Logging
{
    public class GenerationLog : ILog
    {
        private readonly StringBuilder _fileLogBuilder = new StringBuilder();

        public void LogLine(params object[] data)
        {
            _fileLogBuilder.Append(GetPlainString(data));
            _fileLogBuilder.Append("\n");
        }

        public void SaveLog()
        {
#if UNITY_EDITOR
            var path = Application.dataPath + "/";
            if (!AssetDatabase.IsValidFolder(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(path + "/Generation-log.txt", _fileLogBuilder.ToString());
#endif
        }

        public void InsertSeparator()
        {
            LogLine("_________________________________________________________________________________________");
        }

        public void InsertNewSection(params object[] data)
        {
            LogLine("=========================================================================================");
            LogLine(data);
            LogLine("=========================================================================================");
        }

        public string GetPlainString(object[] data)
        {
            var strng = "";

            for (var i = 0; i < data.Length; i++)
            {
                strng += data[i].ToString();
                strng += " ";
            }
            return strng;
        }
    }
}