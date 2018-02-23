using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class MoveVertical : MonoBehaviour
    {
        public float Speed = 1;
        public float MaxDistance = 2;
        public float CollisionCheckOffset = 0.7f;
        public float CollisionTolerance = 1.4f;

        private bool _up;
        private Vector3 _initialPos;
        private BoxCollider2D _collider2D;

        // Use this for initialization
        void Start()
        {
            _initialPos = gameObject.transform.position;
            _collider2D = gameObject.GetComponent<BoxCollider2D>();

            Speed = Random.Range(0.5f, 3f);
            MaxDistance = Random.Range(2f, 5f);
            CollisionCheckOffset = 2f;
            CollisionTolerance = .2f;
    }

        // Update is called once per frame
        void Update()
        {
            var pos = gameObject.transform.position;

            var topColliders = Physics2D.OverlapAreaAll(new Vector2(_collider2D.bounds.min.x + CollisionTolerance, _collider2D.bounds.max.y - CollisionTolerance + CollisionCheckOffset), new Vector2(_collider2D.bounds.max.x - CollisionTolerance, _collider2D.bounds.center.y));
            var bottomColliders = Physics2D.OverlapAreaAll(new Vector2(_collider2D.bounds.min.x + CollisionTolerance, _collider2D.bounds.center.y), new Vector2(_collider2D.bounds.max.x - CollisionTolerance, _collider2D.bounds.min.y - CollisionTolerance - CollisionCheckOffset));

            var relPos = pos - _initialPos;

            var collidesTop = false;
            var collidesBottom = false;

            foreach (var topcollider in topColliders)
            {
                if (topcollider.gameObject != gameObject && topcollider.gameObject.tag == "pcgplatform")
                {
                    collidesTop = true;
                }
            }
            foreach (var bottomcollider in bottomColliders)
            {
                if (bottomcollider.gameObject != gameObject && bottomcollider.gameObject.tag == "pcgplatform")
                {
                    collidesBottom = true;
                }
            }

            if (collidesTop || relPos.y > MaxDistance)
            {
                _up = false;
            }
            else if (collidesBottom || relPos.y < -MaxDistance)
            {
                _up = true;
            }

            if (_up)
            {
                gameObject.transform.position += Vector3.up * Speed * Time.deltaTime;
            }
            else
            {
                gameObject.transform.position += Vector3.down * Speed * Time.deltaTime;
            }
        }
    }
}
