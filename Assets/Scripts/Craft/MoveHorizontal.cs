using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class MoveHorizontal : MonoBehaviour
    {
        public float Speed = 1;
        public float MaxDistance = 5;
        public float CollisionCheckOffset = 0.1f;
        public float CollisionTolerance = 0.1f;

        private bool _right;
        private Vector3 _initialPos;
        private BoxCollider2D _collider2D;

        // Use this for initialization
        void Start ()
        {
            _initialPos = gameObject.transform.position;
            _collider2D = gameObject.GetComponent<BoxCollider2D>();

            Speed = Random.Range(0.8f, 3f);
            MaxDistance = Random.Range(2f, 5f);
            CollisionCheckOffset = 0.1f;
            CollisionTolerance = 0.1f;
        }

        // Update is called once per frame
        void Update ()
        {
            var pos = gameObject.transform.position;

            var leftColliders = Physics2D.OverlapAreaAll(new Vector2(_collider2D.bounds.min.x + CollisionTolerance, _collider2D.bounds.max.y - CollisionTolerance), new Vector2(_collider2D.bounds.center.x, _collider2D.bounds.min.y - CollisionTolerance));
            var rightColliders = Physics2D.OverlapAreaAll(new Vector2(_collider2D.bounds.center.x, _collider2D.bounds.max.y - CollisionTolerance), new Vector2(_collider2D.bounds.max.x - CollisionTolerance, _collider2D.bounds.min.y - CollisionTolerance));

            var relPos = pos - _initialPos;

            var collidesRight = false;
            var collidesLeft = false;

            foreach (var rightCollider in rightColliders)
            {
                if (rightCollider.gameObject != gameObject && rightCollider.gameObject.tag == "pcgplatform")
                {
                    collidesRight = true;
                }
            }
            foreach (var leftCollider in leftColliders)
            {
                if (leftCollider.gameObject != gameObject && leftCollider.gameObject.tag == "pcgplatform")
                {
                    collidesLeft = true;
                }
            }

            if (collidesRight || relPos.x > MaxDistance)
            {
                _right = false;
            } else if (collidesLeft || relPos.x < -MaxDistance)
            {
                _right = true;
            }

            if (_right)
            {
                gameObject.transform.position += Vector3.right* Speed *  Time.deltaTime;
            }
            else
            {
                gameObject.transform.position += Vector3.left* Speed *  Time.deltaTime;
            }
        }
    }
}
