using Assets.Scripts.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Craft
{
    public class PatternPrefabGenerator : MonoBehaviour
    {
        public string BaseFolder = "Blender Assets/";
        public string[] Patterns = { "long" };
        public string[] AnimatedPattern = {"horizontal"};
        public string OutFolder = "platforms/";
        public Material PatternMaterial;

        // Use this for initialization
        public void Generate()
        {
            foreach (var patternfolder in Patterns)
            {
                var patterns = PrefabMappingHelper.GetPathsForFilesInFolder(Application.dataPath + "/Resources/" + BaseFolder + patternfolder, BaseFolder + patternfolder, ".blend");
            
                foreach (var pattern in patterns)
                {
                    GameObject basegObj = CreateGameObject(pattern);
                    SaveAsPrefab(patternfolder, pattern, "", basegObj);

                    foreach (var animPat in AnimatedPattern)
                    {
                        if (animPat == "horizontal")
                        {
                            GameObject gObj = CreateGameObject(pattern);
                            GameObjectHelper.ChangeGameObjectLayerRecursive(gObj, "movablePlatform");
                            gObj.AddComponent<MoveHorizontal>();
                            SaveAsPrefab(patternfolder, pattern, "_H", gObj);
                        }
                        else if (animPat == "vertical")
                        {
                            GameObject gObj = CreateGameObject(pattern);
                            GameObjectHelper.ChangeGameObjectLayerRecursive(gObj, "movablePlatform");
                            gObj.AddComponent<MoveVertical>();
                            SaveAsPrefab(patternfolder, pattern, "_V", gObj);
                        }
                    }
                }
            }
        }

        private void SaveAsPrefab(string patternfolder, string pattern, string animation, GameObject gObj)
        {
#if UNITY_EDITOR

            var lastIndex = pattern.LastIndexOf("/");
            var prefix = "";

            if (lastIndex > 0)
            {
                prefix = pattern.Substring(lastIndex + 1);
            }

            var prefabPath = "Assets/Resources/Base Assets/" + patternfolder + OutFolder + prefix + animation + ".prefab";
            
            PrefabUtility.CreatePrefab(prefabPath, gObj);
#endif
            if (Application.isEditor)
            {
                DestroyImmediate(gObj);
            }
            else
            {
                Destroy(gObj);
            }
        }

        private GameObject CreateGameObject(string pattern)
        {
            var gObj = Instantiate(Resources.Load(pattern) as GameObject);
            if (Application.isEditor)
            {
                DestroyImmediate(gObj.GetComponent<Animator>());
            }
            else
            {
                DestroyImmediate(gObj.GetComponent<Animator>());
            }
            var collider = gObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            gObj.GetComponent<Renderer>().material = PatternMaterial;
            return gObj;
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}