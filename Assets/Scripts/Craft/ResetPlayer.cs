using System.Collections;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class ResetPlayer : MonoBehaviour
    {
        public GameObject HealthBarObject;
        public GameObject JetpackBarObject;
        public GameObject GameStatsManager;
        public GameObject Player;
        public GameObject PlayerParent;
        private UiBar _healthBar;
        private UiBar _jetpackBar;
        private GameStatsManager _gameStatsManager;
        private ParticleSystem _particleSystem;

        void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            _healthBar = HealthBarObject.GetComponent<UiBar>();
            _jetpackBar = JetpackBarObject.GetComponent<UiBar>();
            _gameStatsManager = GameStatsManager.GetComponent<GameStatsManager>();
        }

        public void ResetAll()
        {
            StartCoroutine(ResetPlayerObject());

            _gameStatsManager.ResetAll();
        }

        public IEnumerator ResetPlayerObject()
        {
            Player.transform.parent = PlayerParent.transform;
            Player.gameObject.transform.position = new Vector3(0, 0, 0);
            Player.GetComponent<InputManager>().SetRespawning(false);
            for (int i = 0; i < 5; i++)
            {
                _particleSystem.Emit(20);
                yield return new WaitForSeconds(.15f);
            }

            yield return new WaitForSeconds(0.5f);

            Player.GetComponent<InputManager>().SetRespawning(true);

            _healthBar.SetValue(100, 100);
            _jetpackBar.SetValue(100, 100);
        }
    }
}
