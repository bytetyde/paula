using System.IO;
using Assets.Scripts.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts.Level_Generator.Generator
{
    public class LoadLevel : MonoBehaviour {

        public void LoadLevelFromJson()
        {
#if UNITY_EDITOR
            var path = EditorUtility.OpenFilePanel("Save Raw Level Representation", Application.dataPath + "Levelgenerator Prefabs/Raw Level/", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                var levelParent = GameObjectHelper.CreateEmptyPCGParent(gameObject.GetComponent<GeneratorData>().ParentName);
                var fileText = File.ReadAllText(path);
                var linesInFile = fileText.Split('\n');

                foreach (var line in linesInFile)
                {
                    var obj = JsonUtility.FromJson<KeyValueString>(line);
                    if (obj == null)
                    {
                        continue;
                    }
                    var ressourceObj = Resources.Load("Levelgenerator Prefabs/" + obj.value);
                    if (ressourceObj)
                    {
                        GameObject gObj = (GameObject) Instantiate(ressourceObj, (Vector3) JsonUtility.FromJson<Vector3>(obj.key), new Quaternion());
                        gObj.name = obj.value;
                        gObj.transform.parent = levelParent.transform;
                    }
                }
            }
#endif
        }
    }
}
