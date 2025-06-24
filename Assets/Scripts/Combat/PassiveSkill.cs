using UnityEngine;
using Stats;

public class PassiveSkill : SkillBase
{
    public PassiveSkill(EquipmentData data) : base(data) { }

    protected override void UseSkill()
    {
        // 패시브는 직접 발동 X, 최초 적용만!
        if (playerStats == null) return;
        playerStats.AddPassiveSkillEffect(Data);
        Debug.Log($"[패시브 스킬 적용] {Data.name}: {Data.description}");
    }
}