using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class ObjectPlacement : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            for (var i = 0; i < 14; i++)
            {
                var pos2d = Random.insideUnitCircle;
                var pos3d = new Vector3(pos2d.x, pos2d.y, 0);

                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject.AddComponent<RigidBodyToggler>();
                gameObject.transform.position = pos3d;
                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}