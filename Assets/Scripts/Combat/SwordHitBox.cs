using Combat;
using UnityEngine;
using Stats; // PlayerStats 네임스페이스가 이거면 추가

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
            if (owner is Player player)
            {
                var monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    var playerStats = player.GetComponent<PlayerStats>();
                    if (playerStats == null)
                        return;

                    // --- 크리티컬 판정 ---
                    bool isCritical = Random.value < playerStats.FinalCritRate;
                    float damage = playerStats.FinalAttack;
                    if (isCritical)
                        damage *= playerStats.FinalCritDmg;

                    monster.TakeDamage(damage, isCritical);
                }
            }
            // --- 몬스터 무기: 플레이어만 공격 ---
            else if (owner is Monster monster)
            {
                player = other.GetComponent<Player>();
                if (player != null)
                {
                    float curDamage = (monster as IAttackStat)?.GetAttackPower() ?? 0f;
                    player.TakeDamage(curDamage);
                }
            }
            // (기타 타겟은 무시)
        }
    }
}