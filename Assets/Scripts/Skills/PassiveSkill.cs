using UnityEngine;
using Stats;

public class PassiveSkill : SkillBase
{
    public PassiveSkill(EquipmentData data) : base(data) { }

    protected override void UseSkill()
    {
        // 패시브 스킬은 직접 발동하지 않음.
        // 실제 효과는 PlayerStats.RefreshStats()에서 장착 패시브를 누적해서 자동 적용.
        Debug.Log($"[패시브 스킬] {Data.name} (Passive) — 효과는 RefreshStats에서 자동 누적됨");
    }
}