using UnityEngine;

namespace Assets.Scripts.Level_Generator.Generator
{
    public class GeneratorData : MonoBehaviour
    {
        public Bounds Bounds = new Bounds();
        public Vector3 Center = new Vector3(0, 0, 0);
        public string ParentName;
        public bool InfiniteGeneration = false;
        public bool TwoDimensional =  false;
        public float MinX = 0;
        public float MaxX = 0;
        public float MinY = 0;
        public float MaxY = 0;
        public int CellCount = 0;
        public Vector3 ParentPosition { get; set; }
    }
}