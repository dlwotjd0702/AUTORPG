using UnityEngine;
using Stats;

public class SingleTargetDamageSkill : SkillBase
{
    public SingleTargetDamageSkill(EquipmentData data, SkillManager manager) : base(data, manager) { }

    protected override void UseSkill()
    {
        var target = FindNearestMonster(playerStats.transform.position, 10f);
        if (target == null) return;

        var projectilePrefab = skillManager.GetEffectPrefab(Data.id);         // 예: skill_01~skill_20
        var hitEffectPrefab  = skillManager.GetEffectPrefab("skill_21");      // 히트 이펙트 전용

        if (!projectilePrefab) return;

        var projectile = Object.Instantiate(projectilePrefab, playerStats.transform.position, Quaternion.identity);
        projectile.AddComponent<ProjectileMover>().Init(target.transform, () =>
        {
            float damage = playerStats.FinalAttack * Data.SkillPower;
            target.TakeDamage(damage);
            Debug.Log($"{Data.name} (투사체) → {target.name}에게 {damage} 데미지!");
        }, hitEffectPrefab);
    }
}