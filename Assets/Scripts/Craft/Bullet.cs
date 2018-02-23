using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class Bullet : MonoBehaviour
    {
        public float Damage;
        private float _speed;

        public void Update()
        {
            gameObject.transform.localPosition += -gameObject.transform.right * _speed;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.tag == "enemy")
            {
                collider.gameObject.GetComponent<Enemy>().DamageEnemy(Damage);
            }
            Destroy(gameObject);
        }

        public void SetTarget(Vector3 targetPos, float bulletSpeed, float damage)
        {
            gameObject.transform.LookAt(targetPos);
            gameObject.transform.Rotate(new Vector3(0, 1, 0), 90);
            _speed = bulletSpeed;
            Damage = damage;
            Destroy(gameObject, 5);
        }
    }
}