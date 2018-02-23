using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class Enemy : MonoBehaviour
    {
        public float CurHealth = 100;
        public float Damage = 50;
        public float MaxHealth = 100;
        private GameStatsManager _gameStatsManager;

        // Use this for initialization
        private void Start()
        {
            CurHealth = MaxHealth;
            _gameStatsManager = GameObject.Find("GameStatsManager").GetComponent<GameStatsManager>();
            _gameStatsManager.IncreaseMaxEnemyCount();
        }

        public void DamageEnemy(float value)
        {
            this.CurHealth -= value;
        }

        public void Update()
        {
            if (CurHealth <=0)
            {
                _gameStatsManager.IncreaseCurrentEnemyCount();
                Destroy(gameObject);
            }
        }

    }
}