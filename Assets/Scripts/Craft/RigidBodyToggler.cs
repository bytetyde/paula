using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class RigidBodyToggler : MonoBehaviour
    {
        private int colliderCount;

        // Use this for initialization
        private void Start()
        {
        }

        private void OnCollisionEnter()
        {
            colliderCount++;
        }

        private void OnCollisionExit()
        {
            colliderCount--;
        }

        // Update is called once per frame
        private void Update()
        {
            if (colliderCount <= 0)
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}