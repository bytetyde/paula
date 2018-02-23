using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts.Craft
{
    public enum PatternPartType
    {
        ShortPlatform,
        TallPlatform,
        Decoration,
        Others,
        Enemy,
        Collectable
    }

    public class PatternGenerator : MonoBehaviour
    {
        public Dictionary<PatternPartType, string> AssetMapping = new Dictionary<PatternPartType, string>
        {
            {PatternPartType.ShortPlatform, "shortplatforms/"},
            {PatternPartType.TallPlatform, "tallplatforms/"},
            {PatternPartType.Enemy, "enemies/"},
            {PatternPartType.Collectable, "collectables/"},
            {PatternPartType.Decoration, "decoration/"},
            {PatternPartType.Others, "others/"}
        };

        public string BaseFolder = "Base Assets/";
        public string LongPatterns = "long";
        public string ShortPatterns = "short";
        public string TemplateName;
        public string TemplatePath = "/Resources/Base Assets/";

        public void CreatePatternPrefabs()
        {
            var dto = new PatternBlueprintDto();
            var data = dto.Load(TemplatePath + TemplateName + ".txt");

            foreach (var blueprint in data.BluePrints)
            {
                var parent = new GameObject(blueprint.Name);
                
                foreach (var patternPart in blueprint.PatternParts)
                {
                    var type = GetTypeForString(patternPart.Type);
                    var folder = AssetMapping[type];

                    try
                    {
                        var obj = Instantiate(Resources.Load(data.BaseFolderInput + "/" + folder + patternPart.Name)) as GameObject;

                        obj.transform.position = patternPart.Position;
                        if (obj.layer == LayerMask.NameToLayer("movablePlatform") && parent.layer != LayerMask.NameToLayer("movablePlatform"))
                        {
                            parent.layer = LayerMask.NameToLayer("movablePlatform");
                        }
                        obj.transform.parent = parent.transform;
                    }
                    catch (Exception)
                    {
                        Debug.Log("Can't instantiate object: " + patternPart.Name + " of " + blueprint.Name + " in " + data.BaseFolderInput + "/"  +folder);
                        throw;
                    }
                }

#if UNITY_EDITOR
                var prefabPath = "Assets/Resources/LevelGenerator Prefabs/patterns/" + blueprint.OutputFolder + "/" + blueprint.Name + ".prefab";
                if (!Directory.Exists("Assets/Resources/LevelGenerator Prefabs/patterns/" + blueprint.OutputFolder + "/"))
                {
                    Directory.CreateDirectory("Assets/Resources/LevelGenerator Prefabs/patterns/" + blueprint.OutputFolder + "/");
                }

                PrefabUtility.CreatePrefab(prefabPath, parent);
#endif
                if (Application.isEditor)
                {
                    DestroyImmediate(parent);
                }
                else
                {
                    Destroy(parent);
                }
            }
        }

        public PatternPartType GetTypeForString(string str)
        {
            switch (str) {
                case "tallplatform":
                    return PatternPartType.TallPlatform;
                case "shortplatform":
                    return PatternPartType.ShortPlatform;
                case "collectable":
                    return PatternPartType.Collectable;
                case "enemey":
                    return PatternPartType.Enemy;
                case "others":
                    return PatternPartType.Others;
                case "decoration":
                    return PatternPartType.Decoration;
            }
            throw new Exception ("No pattern type found for: " + str);
        }

        public void CreateTemplate()
        {
            var dto = new PatternBlueprintDto();
            dto.CreateTemplate(Application.dataPath + "/Resources/Base Assets/template" +
                               DateTime.Now.ToString("hmm") + ".txt");
        }
    }
}