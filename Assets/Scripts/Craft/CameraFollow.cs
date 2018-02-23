using Assets.Scripts.Common;
using Assets.Scripts.Level_Generator.Generator;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Craft
{
    public class CameraFollow : MonoBehaviour
    {
        private float _cameraDist;
        private float _xPosition;
        private float _xRotation;
        private float _yPosition;
        public Vector2 CameraDistanceHeights;
        private float _cameraDistanceValue;
        public GameObject LevelGenerator;
        public GameObject GameStatsManager;

        public float DeltaX;
        public float IngameLerpSpeedX = 10f;
        public float IngameLerpSpeedY = 4f;
        public float IngameLerpSpeedZ = 2f;
        public Vector2 MinMaxCameraDistance;
        public Vector2 MinMaxCameraRotationX;
        public float OverViewMargin;
        public Vector2 RotationHeights;
        private bool _sceneOverview;
        private GameObject _target;
        private GeneratorData _generatorData;
        public AudioMixer AudioMixer;
        private InputManager _playerInputManager;
        private GameStatsManager _gameStatsManager;

        private float _lerpSpeedX = 10f;
        private float _lerpSpeedY = 4f;
        private float _lerpSpeedZ = 2f;

        public CameraFollow(AudioMixer audioMixer)
        {
            AudioMixer = audioMixer;
        }

        // Use this for initialization
        private void Start()
        {
            _cameraDist = -MinMaxCameraDistance.x;
            transform.position = new Vector3(0,0, -MinMaxCameraDistance.x);
            _xPosition = 0;
            _yPosition = 0;

            _generatorData = LevelGenerator.GetComponent<GeneratorData>();
            _gameStatsManager = GameStatsManager.GetComponent<GameStatsManager>();
            _sceneOverview = true;
        }

        // Update is called once per frame
        public void LateUpdate()
        {
            var currentVolume = 0f;
            AudioMixer.GetFloat("Master", out currentVolume);

            if(_target == null) {
                _target = GameObject.Find("Player");
                _playerInputManager = _target.GetComponent<InputManager>();
            }

            if (_sceneOverview)
            {
                _gameStatsManager.StopStopwatch();

                if (_target != null)
                {
                    _playerInputManager.SetInputAvailable(false);
                }
                _lerpSpeedX = 2.5f;
                _lerpSpeedY = 1.5f;
                _lerpSpeedZ = 1.5f;
                ShowScene();
                AudioMixer.SetFloat("Master", Mathf.Lerp(currentVolume, -20f, Time.deltaTime));
            }
            else
            {
                AudioMixer.SetFloat("Master", Mathf.Lerp(currentVolume, 0f, Time.deltaTime));
                _gameStatsManager.StartStopwatch();

                if (_target != null)
                {
                    if (_target.transform.position.z - transform.position.z < (MinMaxCameraDistance.y + 1f))
                    {
                        _lerpSpeedX = IngameLerpSpeedX;
                        _lerpSpeedY = IngameLerpSpeedY;
                        _lerpSpeedZ = IngameLerpSpeedZ;
                        
                        _playerInputManager.SetInputAvailable(true);
                    }

                    FollowPlayer();
                }
            }
        }

        public void SetSceneOverView(bool value)
        {
            _sceneOverview = value;
        }

        private void ShowScene()
        {
            var distance = (_generatorData.Bounds.size.x + OverViewMargin)/Camera.main.aspect*0.5f/Mathf.Tan(Camera.main.fieldOfView*0.5f*Mathf.Deg2Rad);
            transform.position = Vector3.Lerp(transform.position, new Vector3(_generatorData.Center.x + _generatorData.ParentPosition.x, 0, -distance),Time.deltaTime*_lerpSpeedZ);
            var xRot = Mathf.LerpAngle(transform.localEulerAngles.x, 0, _lerpSpeedX * Time.deltaTime);

            transform.localEulerAngles = new Vector3(xRot, gameObject.transform.rotation.y, gameObject.transform.rotation.z);
        }

        private void FollowPlayer()
        {
            //Camera Window X Axis
            if (_target.transform.position.x > transform.position.x + DeltaX/2)
            {
                _xPosition = _target.transform.position.x - DeltaX/2;
            }
            if (_target.transform.position.x < transform.position.x - DeltaX/2)
            {
                _xPosition = _target.transform.position.x + DeltaX/2;
            }
            var xPos = new Vector3(Mathf.Lerp(transform.position.x, _xPosition, _lerpSpeedX*Time.deltaTime), transform.position.y, transform.position.z);
            transform.position = xPos;


            _yPosition = _target.transform.position.y + GameObjectHelper.Map(_target.transform.position.y, 0f, 15f, 2, -3f);
            
            var height = _target.transform.position.y;
            _xRotation = Map(height, RotationHeights.x, RotationHeights.y, MinMaxCameraRotationX.x, MinMaxCameraRotationX.y);

            //Camera Height - Distance
            var dist = Map(_target.transform.position.y, CameraDistanceHeights.x, CameraDistanceHeights.y, 0, 1);
            dist = Mathf.Sin(dist * Mathf.PI * 0.5f);

            _cameraDistanceValue = Map(dist, 0, 1, MinMaxCameraDistance.x, MinMaxCameraDistance.y);

            var toValue = new Vector3(transform.position.x, transform.position.y, -_cameraDistanceValue);
            _cameraDist = Vector3.Lerp(transform.position, toValue, _lerpSpeedZ*Time.deltaTime).z;

            var yPos = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, _yPosition, _lerpSpeedY*Time.deltaTime), transform.position.z);
            transform.position = yPos;

            //Camera X-Axis rotation
            var xRot = Mathf.LerpAngle(transform.localEulerAngles.x, _xRotation, _lerpSpeedX*Time.deltaTime);

            transform.localEulerAngles = new Vector3(xRot, gameObject.transform.rotation.y, gameObject.transform.rotation.z);

            transform.position = new Vector3(transform.position.x, transform.position.y, _cameraDist);
        }

        private float Map(float x, float inMin, float inMax, float outMin, float outMax)
        {
            return (x - inMin)*(outMax - outMin)/(inMax - inMin) + outMin;
        }
    }
}