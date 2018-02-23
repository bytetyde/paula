using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class CollisionDetector : MonoBehaviour
    {

        public bool CollidesLeft;
        public bool CollidesRight;

        public GameObject TopLeft;
        public GameObject BottomLeft;

        public GameObject TopRight;
        public GameObject BottomRight;

        public void FixedUpdate()
        {
            CollidesLeft = false;
            CollidesRight = false;

            Collider2D[] backCollisions = Physics2D.OverlapAreaAll(TopLeft.transform.position, BottomLeft.transform.position);
            foreach (var col in backCollisions)
            {
                if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    CollidesRight = true;
                }
            }
            
            Collider2D[] frontCollisions = Physics2D.OverlapAreaAll(TopRight.transform.position, BottomRight.transform.position);
            foreach (var col in frontCollisions)
            {
                if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    CollidesLeft = true;
                }
            }
            
        }
    }
}
