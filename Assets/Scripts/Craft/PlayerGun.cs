using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class PlayerGun : MonoBehaviour
    {
        private bool _canShoot;
        public GameObject Bullet;
        public float Damage;
        public GameObject BulletSpawn;
        public float BulletSpeed = 0.5f;
        private float Counter;
        public float FireRate = 0.5f;
        public GameObject Gun;
        private Quaternion gunRotation;
        public float LerpSpeed;
        public Vector3 Offset = new Vector3(58.35f, 146.3f, -27.5f);
        private bool shootingActive;
        private Vector3 worldPos;
        private Vector3 _gunPosition;
        public AudioClip ShootClip;
        private AudioSource _audioSource;
        private InputManager _inputManager;

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _inputManager = GetComponent<InputManager>();
        }

        private void Update()
        {
            _gunPosition = BulletSpawn.transform.position;
        }

        private void LateUpdate()
        {
            if (_inputManager.GetInputAvailable() == false)
            {
                return;
            }

            if (Input.GetButtonDown("Fire") || Input.GetKeyDown(KeyCode.LeftShift))
            {
                Shoot();
                Counter = 0;
                shootingActive = true;
            }

            if (Input.GetButtonUp("Fire") || Input.GetKeyUp(KeyCode.LeftShift))
            {
                shootingActive = false;
            }

            if (shootingActive)
            {
                Counter += Time.deltaTime;

                if (Counter > FireRate)
                {
                    Shoot();
                    Counter = 0;
                }
            }

            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -Camera.main.transform.position.z;

            var cameraPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            worldPos = Vector3.Lerp(worldPos, new Vector3(cameraPos.x, cameraPos.y, 0), Time.deltaTime * LerpSpeed);

            var relativePos = worldPos - Gun.transform.position;
            var gunRotationAngle = Vector3.Angle(gameObject.transform.forward, relativePos);
            var cross = Vector3.Cross(new Vector3(1, 0, 0), relativePos);

            if (cross.z < 0)
            {
                gunRotationAngle = -gunRotationAngle;
            }

            if (gunRotationAngle < 85 && gunRotationAngle >= 0)
            {
                gunRotation = Quaternion.LookRotation(relativePos);
                _canShoot = true;
            }
            else if (gunRotationAngle > -70 && gunRotationAngle < 0)
            {
                gunRotation = Quaternion.LookRotation(relativePos);
                _canShoot = true;
            }
            else
            {
                gunRotation = Quaternion.Lerp(gunRotation, Quaternion.LookRotation(gameObject.transform.forward), Time.deltaTime * LerpSpeed);
                _canShoot = false;
            }

            Gun.transform.rotation = gunRotation;
            Gun.transform.Rotate(new Vector3(0, 1, 0), Offset.x);
            Gun.transform.Rotate(new Vector3(1, 0, 0), Offset.y);
            Gun.transform.Rotate(new Vector3(0, 0, 1), Offset.z);
        }

        private void Shoot()
        {
            if (_canShoot)
            {
                _audioSource.PlayOneShot(ShootClip, 1f);
                var bullet = Instantiate(Bullet, _gunPosition, Quaternion.identity) as GameObject;
                if (bullet)
                {
                    bullet.GetComponent<Bullet>().SetTarget(worldPos, BulletSpeed, Damage);
                }
            }
        }
    }
}