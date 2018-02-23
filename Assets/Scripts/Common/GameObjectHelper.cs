using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class GameObjectHelper
    {
        public static Bounds CalculateBoundsWithChildren(GameObject obj, string includeTag = "", string excludeTag = "")
        {
            var currentRotation = obj.transform.rotation;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            var bounds = new Bounds(obj.transform.position, Vector3.zero);

            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            {
                if (excludeTag == "" && renderer.gameObject.tag != excludeTag)
                {
                    if (includeTag == "" || renderer.gameObject.tag == includeTag)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            var localCenter = bounds.center - obj.transform.position;
            bounds.center = localCenter;

            obj.transform.rotation = currentRotation;

            return bounds;
        }


        public static Bounds CalculateGlobalBoundsOfChildren(GameObject obj, string includeTag = "", string excludeTag = "")
        {
            var currentRotation = obj.transform.rotation;
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            var bounds = new Bounds();
            var boundSet = false;

            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            {
                if (!boundSet)
                {
                    bounds = new Bounds(renderer.transform.position, Vector3.zero);
                    boundSet = true;
                }
                if (excludeTag == "" || renderer.gameObject.tag != excludeTag)
                {
                    if (includeTag == "" || renderer.gameObject.tag == includeTag)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            obj.transform.rotation = currentRotation;

            return bounds;
        }

        public static void ChangeGameObjectLayerRecursive(GameObject obj, string name)
        {
            ChangeGameObjectLayerRecursive(obj, LayerMask.NameToLayer(name));
        }

        public static void ChangeGameObjectLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                ChangeGameObjectLayerRecursive(child.gameObject, layer);
            }
        }

        public static GameObject CreateEmptyPCGParent(string name)
        {
            GameObject levelParent;
            levelParent = GameObject.Find(string.IsNullOrEmpty(name) ? "Default" : name);

            if (levelParent != null)
            {
                RemoveChildsFromObject(levelParent);
            }
            else
            {
                levelParent = string.IsNullOrEmpty(name) ? new GameObject("Default") : new GameObject(name);
            }
            return levelParent;
        }

        public static void RemoveChildsFromObject(GameObject objGameObject)
        {
            var childs = objGameObject.GetComponentsInChildren<Transform>();
            for (int index = childs.Length -1; index > 0; index--)
            {
                if (Application.isEditor)
                {
                    UnityEngine.Object.DestroyImmediate(childs[index].transform.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(childs[index].transform.gameObject);
                }
            }
        }

        public static float Map(float value, float istart, float istop, float ostart, float ostop)
        {
            return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
        }
    }
}