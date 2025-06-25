using UnityEngine;
using Stats;

public class SingleTargetDamageSkill : SkillBase
{
    public SingleTargetDamageSkill(EquipmentData data, SkillManager manager) : base(data, manager) { }

    protected override void UseSkill()
    {
        var target = FindNearestMonster(playerStats.transform.position, 10f);
        if (target == null) return;

        var projectilePrefab = skillManager.GetEffectPrefab(Data.id);
        if (!projectilePrefab) return;

        var projectile = Object.Instantiate(projectilePrefab, playerStats.transform.position, Quaternion.identity);
        projectile.AddComponent<ProjectileMover>().Init(target.transform, () =>
        {
            float damage = playerStats.FinalAttack * Data.SkillPower;
            target.TakeDamage(damage);
            Debug.Log($"{Data.name} (투사체) → {target.name}에게 {damage} 데미지!");
        });
    }
}