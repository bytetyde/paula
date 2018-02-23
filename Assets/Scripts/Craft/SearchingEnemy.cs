using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Craft
{
    public class SearchingEnemy : MonoBehaviour
    {

        public Vector2 MinMaxSearchRadius;
        public float Speed;
        private bool _playerInRange;
        private GameObject _player;
        private bool _attack;
        public Vector3 Offset;
        private BoxCollider2D _boxCollider2D;
        private float _searchRadius;
        private Vector3 _startPos;
        public Vector2 MinMaxAmplitudeUp = new Vector2(0.05f, 0.15f);
        public Vector2 MinMaxAmplitudeRight = new Vector2(0.05f, 0.15f);
        public Vector2 MinMaxPeriodUp = new Vector2(0.8f, 1.5f);
        public Vector2 MinMaxPeriodRight = new Vector2(.08f, 1.5f);
        public GameObject CenterGameObject;
        private float _amplitudeUp;
        private float _amplitudeRight;
        private float _periodUp;
        private float _periodRight;

        // Use this for initialization
        void Start () {
            _player = GameObject.Find("Player");
            _boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
            _searchRadius = Random.Range(MinMaxSearchRadius.x, MinMaxSearchRadius.y);
            _startPos = transform.position;

            _amplitudeUp = Random.Range(MinMaxAmplitudeUp.x, MinMaxAmplitudeUp.y);
            _amplitudeRight = Random.Range(MinMaxAmplitudeRight.x, MinMaxAmplitudeRight.y);
            _periodUp = Random.Range(MinMaxPeriodUp.x, MinMaxPeriodUp.y);
            _periodRight = Random.Range(MinMaxPeriodRight.x, MinMaxPeriodRight.y);
    }
	
        // Update is called once per frame
        void Update ()
        {

            var playerCenter = new Vector3(_player.transform.position.x, _player.transform.position.y + 0.5f,_player.transform.position.z);
            var enemyCenter = CenterGameObject.transform.position;

            var dir = enemyCenter - playerCenter;
            _attack = false;

            if (Vector3.Distance(enemyCenter, playerCenter) < _searchRadius)
            {
                var diffTop = playerCenter + new Vector3(0, 0.5f, 0) - enemyCenter;

                _boxCollider2D.enabled = false;
                var coll = Physics2D.Raycast(enemyCenter, diffTop, diffTop.magnitude);
                _boxCollider2D.enabled = true;

                if (coll && coll.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    _attack = true;
                }
                
            }
            if (_attack)
            {
                gameObject.transform.position -= dir.normalized*Speed*Time.deltaTime;
                _startPos = gameObject.transform.position;
            }
            else
            {

                float thetaUp = Time.timeSinceLevelLoad/_periodUp;
                float distanceUp = _amplitudeUp*Mathf.Sin(thetaUp);
                transform.position = _startPos + Vector3.up*distanceUp;

                float thetaRight = Time.timeSinceLevelLoad/_periodRight;
                float distanceRight = _amplitudeRight*Mathf.Sin(thetaRight);
                transform.position = transform.position + Vector3.right*distanceRight;
            }
        }
    }
}
