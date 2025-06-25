using UnityEngine;
using Stats;

public class SingleTargetDamageSkill : SkillBase
{
    public SingleTargetDamageSkill(EquipmentData data) : base(data) { }

    protected override void UseSkill()
    {
        var target = FindNearestMonster(playerStats.transform.position, 10f);
        if (target != null)
        {
            float dmg = (float)(playerStats.FinalAttack * Data.skillPower);
            target.TakeDamage(dmg);
            Debug.Log($"{Data.name} → {target.name}에게 {dmg} 데미지!");
        }
    }
}