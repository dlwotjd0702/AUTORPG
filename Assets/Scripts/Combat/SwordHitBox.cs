using Combat;
using UnityEngine;

namespace IdleRPG
{
    public class SwordHitBox : MonoBehaviour
    {
        public MonoBehaviour owner;

        void OnTriggerEnter(Collider other)
        {
            // 공격자가 본인한테 맞추지 않도록 체크
            if (other.gameObject == owner.gameObject)
                return;

            // --- 플레이어 무기: 몬스터만 공격 ---
            if (owner is Player)
            {
                var monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    float curDamage = (owner as IAttackStat)?.GetAttackPower() ?? 0f;
                    monster.TakeDamage(curDamage);
                }
            }
            // --- 몬스터 무기: 플레이어만 공격 ---
            else if (owner is Monster)
            {
                var player = other.GetComponent<Player>();
                if (player != null)
                {
                    float curDamage = (owner as IAttackStat)?.GetAttackPower() ?? 0f;
                    player.TakeDamage(curDamage);
                }
            }
            // (기타 타겟은 무시)
        }
    }
}