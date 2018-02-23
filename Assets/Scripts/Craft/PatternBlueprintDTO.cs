using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Assets.Scripts.Craft
{
    public class PatternBlueprintDto
    {
        public PatternBluePrintData Load(string path)
        {
            var fileText = File.ReadAllText(path);
            
            return JsonUtility.FromJson<PatternBluePrintData>(fileText);            
        }

        public void CreateTemplate(string path)
        {
            var data = new PatternBluePrintData(new List<PatternDataObject>(), "in_folder", "out_folder");
            data.BluePrints.Add(new PatternDataObject("tmpName", new List<PatternPartDataObject>(), "outfolder"));
            var dataObj = data.BluePrints.First();
            dataObj.PatternParts.Add(new PatternPartDataObject("tmpName", "tallplatform", new Vector3()));

            var jsonText = JsonUtility.ToJson(data, true);

            File.WriteAllText(path, jsonText);
        }
    }
    
    [Serializable]
    public class PatternBluePrintData
    {
        public string BaseFolderInput;
        public string BaseFolderOutput;
        public List<PatternDataObject> BluePrints;
        
        public PatternBluePrintData(List<PatternDataObject> list, string baseIn, string baseOut)
        {
            BluePrints = list;
            BaseFolderInput = baseIn;
            BaseFolderOutput = baseOut;
        }
    }

    [Serializable]
    public class PatternDataObject
    {
        public string Name;
        public string OutputFolder;
        public List<PatternPartDataObject> PatternParts;

        public PatternDataObject(string name, List<PatternPartDataObject> list, string outfolder)
        {
            Name = name;
            PatternParts = list;
            OutputFolder = outfolder;
        }
    }

    [Serializable]
    public class PatternPartDataObject
    {
        public string Name;
        public string Type;
        public Vector3 Position;

        public PatternPartDataObject(string name, string type, Vector3 pos)
        {
        Name = name;
        Type = type;
        Position = pos;
        }
    }
}