using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class Collectable : MonoBehaviour
    {
        private GameObject _gameStatsManagerObject;
        private GameStatsManager _gameStatsManager;
        private Vector3 _destinationPos;
        public float MinDistance = 1f;
        public float Speed = 10;
        private bool _hasDestination;
        public AudioClip AudioClip;
        private AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        // Use this for initialization
        void Start ()
        {
            _gameStatsManagerObject = GameObject.Find("GameStatsManager");
            _gameStatsManager = _gameStatsManagerObject.GetComponent<GameStatsManager>();

            _gameStatsManager.IncreaseMaxCollectableCount();
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll!= null && coll.gameObject.layer == LayerMask.NameToLayer("Player"))
            {    
                _audioSource.PlayOneShot(AudioClip, 1f);
                _hasDestination = true;
            }
        }

        void Update()
        {
            if (_hasDestination)
            {
                var screenPos = _gameStatsManager.CurrentCollectableText.GetComponent<RectTransform>().position;
                screenPos.z = -Camera.main.transform.position.z;

                var cameraPos = Camera.main.ScreenToWorldPoint(screenPos);
                _destinationPos = new Vector3(cameraPos.x, cameraPos.y, 0);

                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _destinationPos,
                    Speed*Time.deltaTime);
            }
            var distance = Vector3.Distance(gameObject.transform.position, _destinationPos);
            if (distance < MinDistance)
            {
                _gameStatsManager.IncreaseCurrentCollectableCount();
                Destroy(gameObject);
            }
        }
    }
}
