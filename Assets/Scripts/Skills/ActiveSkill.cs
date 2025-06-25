using IdleRPG;
using UnityEngine;
using Stats;

public class ActiveSkill : SkillBase
{
    public ActiveSkill(EquipmentData data) : base(data) { }

    protected override void UseSkill()
    {
        Debug.Log($"[액티브 스킬 사용] {Data.name} 발동! 계수: {Data.SkillPower}");

        // 플레이어 스탯(공격력 등) 활용해 타격 효과
        if (playerStats == null)
        {
            Debug.LogWarning("[ActiveSkill] PlayerStats 연결 안됨!");
            return;
        }

        // 예: 가장 가까운 몬스터 타격
        Monster nearest = FindNearestMonster(playerStats.transform.position, 10f);
        if (nearest != null)
        {
            float damage = playerStats.FinalAttack * Data.SkillPower;
            nearest.TakeDamage(damage);
            Debug.Log($"{Data.name} → {damage} 데미지!");
        }
        // 이펙트 연출 필요 시 추가
    }

    Monster FindNearestMonster(Vector3 pos, float radius)
    {
        float minDist = float.MaxValue;
        Monster nearest = null;
        var all = GameObject.FindObjectsOfType<Monster>();
        foreach (var m in all)
        {
            float d = Vector3.Distance(pos, m.transform.position);
            if (d < minDist && d <= radius)
            {
                minDist = d;
                nearest = m;
            }
        }
        return nearest;
    }
}