using UnityEngine;

namespace IdleRPG
{
    public class SwordHitBox : MonoBehaviour
    {
        public int damage = 1;
        public MonoBehaviour owner; // 공격자, Player 또는 Monster

        void OnTriggerEnter(Collider other)
        {
            // 공격자가 본인한테 맞추지 않도록 체크
            if (other.gameObject == owner.gameObject)
                return;

            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}