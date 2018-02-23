using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class RotateAroundAxis : MonoBehaviour
    {

        public Vector3 RotationAxis;
        public float RotationSpeed;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
            gameObject.transform.Rotate(new Vector3(RotationAxis.x * RotationSpeed, RotationAxis.y * RotationSpeed, RotationAxis.z * RotationSpeed));
        }
    }
}
