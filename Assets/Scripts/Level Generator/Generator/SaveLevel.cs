using System.IO;
using System.Text;
using Assets.Scripts.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Generator
{
    public class SaveLevel {

        public void SaveLevelToJson(string objName)
        {
#if UNITY_EDITOR
            var level = GameObject.Find(objName);
            var path = EditorUtility.SaveFilePanel("Save Raw Level Representation", "", "RawLevel", "txt");
        
            if (level)
            {
                StringBuilder sb = new StringBuilder();
                var childs = level.GetComponentsInChildren<Transform>();
                foreach (var child in childs)
                {
                    if (Resources.Load("Levelgenerator Prefabs/" + child.name))
                    {
                        sb.AppendLine(JsonUtility.ToJson(new KeyValueString(JsonUtility.ToJson(child.position), child.name)));
                    }
                }
                if (!string.IsNullOrEmpty(path))
                {
                    File.WriteAllText(path, sb.ToString());
                }
            }
#endif
        }
    }
}
