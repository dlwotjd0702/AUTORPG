using IdleRPG;
using UnityEngine;
using Stats;

public class AoeSkill : SkillBase
{
    private float aoeRadius = 6f; // 필요에 따라 조절

    public AoeSkill(EquipmentData data, SkillManager manager) : base(data, manager) { }

    protected override void UseSkill()
    {
        int hitCount = 0;
        foreach (var m in GameObject.FindObjectsOfType<Monster>())
        {
            float dist = Vector3.Distance(playerStats.transform.position, m.transform.position);
            if (dist <= aoeRadius)
            {
                float dmg = playerStats.FinalAttack * Data.SkillPower;
                m.TakeDamage(dmg);
                hitCount++;
            }
        }

        var effect = skillManager.GetEffectPrefab(Data.id);
        if (effect)
            Object.Instantiate(effect, playerStats.transform.position, Quaternion.identity);

        Debug.Log($"{Data.name} (범위 즉발): {hitCount}명 피해");
    }
}