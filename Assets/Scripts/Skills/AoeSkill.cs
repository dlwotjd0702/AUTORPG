using IdleRPG;
using UnityEngine;
using Stats;

public class AoeSkill : SkillBase
{
    public AoeSkill(EquipmentData data) : base(data) { }

    protected override void UseSkill()
    {
        // 전체 적 가져오기 (예: 태그가 "Enemy"인 오브젝트들)
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int hitCount = 0;
        foreach (var enemyObj in enemies)
        {
            var monster = enemyObj.GetComponent<Monster>();
            if (monster == null) continue;

            float dmg = (float)(playerStats.FinalAttack * Data.skillPower);
            monster.TakeDamage(dmg);
            hitCount++;

            // 얼음폭풍(빙결): skill_10만 추가 적용
            if (Data.id == "skill_10")
            {
                monster.ApplyCrowdControl("Freeze", 2f); // 직접 함수 구현 필요
            }
        }
        Debug.Log($"{Data.name}: 전체 적 {hitCount}명에게 {Data.skillPower}배 피해!");
    }
}