using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class Player : MonoBehaviour
    {
        public GameObject Healthbar;
        private UiBar _healthbar;
        public float CurHealth;
        public float MaxHealth;
        public float HealDelay;
        public float HealIncrease;
        private float _healDelay;
        private ResetPlayer _resetPlayer;
        public AudioClip AudioClip;
        private AudioSource _audioSource;

        private void Start()
        {
            _resetPlayer = GameObject.Find("ResetPlayer").GetComponent<ResetPlayer>();
            _healthbar = Healthbar.GetComponent<UiBar>();
            CurHealth = MaxHealth;
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _healthbar.SetValue(CurHealth, MaxHealth);

            _healDelay += Time.deltaTime;
            if (_healDelay > HealDelay)
            {
                CurHealth += HealIncrease*Time.deltaTime;
            }
            if (CurHealth > MaxHealth)
            {
                CurHealth = MaxHealth;
            }
            if (gameObject.transform.position.y < -7)
            {
                PlayerReset();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "enemy")
            {
                _audioSource.PlayOneShot(AudioClip);
                var enemy = other.gameObject.GetComponent<Enemy>();
                CurHealth -= enemy.Damage;
                enemy.DamageEnemy(100);
                _healDelay = 0;
                if (CurHealth <= 0)
                {
                    PlayerReset();
                }
            }
        }

        public void PlayerReset()
        {
            StartCoroutine(_resetPlayer.ResetPlayerObject());
            CurHealth = MaxHealth;
        }
    }
}