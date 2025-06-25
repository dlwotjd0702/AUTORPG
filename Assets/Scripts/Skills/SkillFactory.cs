using Stats;
using UnityEngine;

public static class SkillFactory
{
    // SkillManager를 넘기도록 구조 변경
    public static SkillBase Create(GameObject owner, EquipmentData data, SkillManager skillManager)
    {
        var playerStats = owner.GetComponent<PlayerStats>();
        if (playerStats == null)
            Debug.LogWarning("[SkillFactory] PlayerStats 컴포넌트가 없습니다!");

        SkillBase skill = data.id switch
        {
            "skill_01" or "skill_09" or "skill_16"
                => new SingleTargetDamageSkill(data, skillManager),
            "skill_03" or "skill_10"
                => new AoeSkill(data, skillManager),
            "skill_07"
                => new MultiHitSkill(data, 3, 1.0f, skillManager),
            "skill_02"
                => new BuffSkill(data, BuffSkill.BuffType.Attack, 0.3f, 5f, skillManager),
            "skill_04"
                => new BuffSkill(data, BuffSkill.BuffType.AtkSpeed, 0.3f, 5f, skillManager),
            "skill_08"
                => new BuffSkill(data, BuffSkill.BuffType.CritRate, 0.2f, 5f, skillManager),
            "skill_05" or "skill_06" or "skill_11" or "skill_13"
                => new PassiveSkill(data, skillManager),
            _ => throw new System.Exception("정의되지 않은 스킬 ID: " + data.id)
        };

        skill.SetOwner(playerStats);
        return skill;
    }
}