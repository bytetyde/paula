using System;
using Assets.Scripts.Common;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Craft
{
    public class BackgroundPrefabGenerator : MonoBehaviour
    {
        private string _basePath;
        public Vector2 MinMaxRandomYOffset;
        public Vector2 MinMaxRandomXOffset;
        public Vector2 MinMaxRandomZOffset;
        public bool AppendLights = true;
        public string BaseFolder = "BackgroundAssets/";

        private readonly Color[] colors =
        {
            new Color(255, 255, 255),
            new Color(230, 68, 68),
            new Color(90, 68, 255),
            new Color(68, 178, 255),
            new Color(68, 255, 222),
            new Color(68, 255, 68),
            new Color(200, 255, 68),
            new Color(255, 218, 68),
            new Color(255, 125, 68)
        };

        public string[] Levels = {"level1"};
        public GameObject Light;
        public string OutFolder = "background/";
        public float Propability = 30;
        public Material SpriteMaterial;

        // Use this for initialization
        public void Generate()
        {
            foreach (var level in Levels)
            {
                var sprites =
                    PrefabMappingHelper.GetPathsForFilesInFolder(Application.dataPath + "/Resources/" + BaseFolder + level,
                        "BackgroundAssets/" + level, ".png");
                var counter = 0;
                foreach (var sprite in sprites)
                {
                    Random.seed = (int)DateTime.Now.Ticks;

                    var xOffset = Random.Range(MinMaxRandomXOffset.x, MinMaxRandomXOffset.y);
                    var yOffset = Random.Range(MinMaxRandomYOffset.x, MinMaxRandomYOffset.y);
                    var zOffset = Random.Range(MinMaxRandomZOffset.x, MinMaxRandomZOffset.y);

                    var gObj = new GameObject(level + "_" + counter);
                    var spr = Resources.Load<Sprite>(sprite);
                    var spriteObj = new GameObject("building_" + counter);
                    var spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.material = SpriteMaterial;
                    spriteRenderer.sprite = spr;
                    var spriteBounds = spriteRenderer.bounds;
                    spriteObj.transform.position = new Vector3(spriteBounds.size.x/2 + xOffset, -spriteBounds.min.y + yOffset, + zOffset);
                    spriteRenderer.gameObject.transform.parent = gObj.transform;

                    if (AppendLights)
                    {
                        var rnd = Random.Range(0, 100);
                    
                        if (rnd > 100 - Propability)
                        {
                            var light = Instantiate(Light);
                            light.transform.parent = spriteObj.transform;

                            var leftCenterRight = Random.Range(0, 3);
                            var pos = new Vector3(0, 0, 0);
                            if (leftCenterRight == 0)
                            {
                                pos = new Vector3(spriteBounds.min.x, spriteBounds.min.y + 3,
                                    spriteBounds.max.z - 2);
                            }
                            else if (leftCenterRight == 1)
                            {
                                pos = new Vector3(spriteBounds.center.x, spriteBounds.min.y + 3,
                                    spriteBounds.max.z - 2);
                            }
                            else if (leftCenterRight == 2)
                            {
                                pos = new Vector3(spriteBounds.max.x, spriteBounds.min.y + 3,
                                    spriteBounds.max.z - 2);
                            }
    
                            light.transform.localPosition = pos;
                            light.GetComponent<Light>().color = colors[Random.Range(0, colors.Length - 1)];
                        }
                    }
#if UNITY_EDITOR
                    var prefabPath = "Assets/Resources/LevelGenerator Prefabs/" + OutFolder + level + "/" + level +
                                     "_building_" + counter + ".prefab";

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
                    counter++;
                }
            }
        }


        // Update is called once per frame
        private void Update()
        {
        }
    }
}