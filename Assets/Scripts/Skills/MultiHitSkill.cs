using System.Collections;
using UnityEngine;
using Stats;

public class MultiHitSkill : SkillBase
{
    int hitCount;
    float dmgRatio;

    public MultiHitSkill(EquipmentData data, int count, float ratio, SkillManager manager) : base(data, manager)
    {
        hitCount = count;
        dmgRatio = ratio;
    }

    protected override void UseSkill()
    {
        playerStats.StartCoroutine(MultiHit());
    }

    private IEnumerator MultiHit()
    {
        for (int i = 0; i < hitCount; i++)
        {
            var target = FindNearestMonster(playerStats.transform.position, 10f);
            if (target != null)
            {
                float dmg = playerStats.FinalAttack * dmgRatio;
                target.TakeDamage(dmg);
                Debug.Log($"[연속 공격 {i + 1}/{hitCount}] {dmg} 데미지");
            }
            yield return new WaitForSeconds(0.2f); // 타격 간격
        }
    }
}