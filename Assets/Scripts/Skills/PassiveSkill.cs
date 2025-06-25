using UnityEngine;
using Stats;

public class PassiveSkill : SkillBase
{
    public PassiveSkill(EquipmentData data, SkillManager manager) : base(data, manager) { }

    protected override void UseSkill()
    {
        // 패시브는 RefreshStats에서 자동 적용, 이펙트 필요시 여기에 추가
    }
}