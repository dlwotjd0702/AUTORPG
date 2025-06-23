using UnityEngine;
using System.Collections.Generic;
using Inventory;
using Stats;

public class SkillManager : MonoBehaviour
{
    public InventorySystem inventory;
    public PlayerStats playerStats; // 플레이어 능력치, 필요시 Player 본체로 바꿔도 됨

    string[] equippedSkillIds;
    Dictionary<string, float> skillCooldowns = new();

    void Awake()
    {
        // 장착 스킬 목록 캐싱
        equippedSkillIds = inventory.equippedSkillIds;
        // 쿨타임 슬롯 준비
        for (int i = 0; i < equippedSkillIds.Length; i++)
        {
            string id = equippedSkillIds[i];
            if (!string.IsNullOrEmpty(id))
            {
                var data = inventory.GetEquipmentData(id);
                if (data != null && data.type == ItemType.skill && data.skillType == SkillType.Active)
                    skillCooldowns[id] = 0f;
            }
        }
        // 패시브 스킬은 시작 즉시 적용
        ApplyAllPassive();
    }

    void Update()
    {
        // 쿨타임 갱신
        foreach (var id in skillCooldowns.Keys)
        {
            if (skillCooldowns[id] > 0)
                skillCooldowns[id] -= Time.deltaTime;
        }
    }

    public bool UseSkill(int slotIdx)
    {
        if (slotIdx < 0 || slotIdx >= equippedSkillIds.Length)
            return false;
        string skillId = equippedSkillIds[slotIdx];
        if (string.IsNullOrEmpty(skillId))
            return false;
        var data = inventory.GetEquipmentData(skillId);
        if (data == null || data.type != ItemType.skill || data.skillType != SkillType.Active)
            return false;
        if (skillCooldowns[skillId] > 0)
            return false; // 쿨타임임

        // [1] 스킬 효과 발동
        ApplySkillEffect(data);

        // [2] 쿨타임 세팅
        skillCooldowns[skillId] = (float)data.cooldown;

        // [3] 필요시 UI에 쿨타임 반영(별도 호출)
        return true;
    }

    void ApplyAllPassive()
    {
        foreach (var id in equippedSkillIds)
        {
            if (string.IsNullOrEmpty(id)) continue;
            var data = inventory.GetEquipmentData(id);
            if (data != null && data.type == ItemType.skill && data.skillType == SkillType.Passive)
            {
                ApplySkillEffect(data);
            }
        }
    }

    void ApplySkillEffect(EquipmentData skill)
    {
        // 이 부분은 스킬 효과에 맞게 실제 플레이어/몬스터에 적용
        // 아래는 예시가 아니라, "직접 효과 호출"만 남겨둠

        if (skill.skillType == SkillType.Passive)
        {
            // ex) 패시브: 공격력, 방어력, HP, 기타 버프 적용
            playerStats.AddPassiveSkillEffect(skill);
        }
        else if (skill.skillType == SkillType.Active)
        {
            // ex) 액티브: 직접 데미지, 치유, 범위 등 효과
            playerStats.ActivateSkill(skill);
        }
    }

    // 쿨타임 확인용
    public float GetSkillCooldown(string skillId)
    {
        return skillCooldowns.TryGetValue(skillId, out var t) ? t : 0f;
    }
}
