using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Stats;

public class SkillManager : MonoBehaviour
{
    public InventorySystem inventory;
    public PlayerStats playerStats;

    // 현재 장착된 스킬만 관리
    public Dictionary<string, SkillBase> skillDict = new();

    private void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<InventorySystem>();
        if (!playerStats) playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var skill in skillDict.Values)
            skill.UpdateCooldown(dt);
    }

    public void UseSkillById(string skillId)
    {
        if (skillDict.TryGetValue(skillId, out var skill))
            skill.TryUseSkill();
        else
            Debug.LogWarning($"SkillManager: 스킬 ID '{skillId}'를 찾을 수 없습니다.");
    }

    // 장착/해제 메서드에서 호출!
    public void EquipSkillToSlot(int slotIdx, string skillId)
    {
        // 기존 슬롯에 뭔가 있었다면 제거
        string prevSkillId = inventory.equippedSkillIds[slotIdx];
        if (!string.IsNullOrEmpty(prevSkillId))
            skillDict.Remove(prevSkillId);

        // 새로운 스킬 추가
        var data = inventory.GetEquipmentData(skillId);
        if (data != null)
        {
            var skill = SkillFactory.Create(playerStats.gameObject, data);
            skillDict[skillId] = skill;
        }
    }

    // 필요시 전체 초기화 (예: 로딩, 세이브 불러오기 등)
    public void InitializeSkillsFromSlots()
    {
        skillDict.Clear();
        for (int i = 0; i < inventory.equippedSkillIds.Length; i++)
        {
            string id = inventory.equippedSkillIds[i];
            if (!string.IsNullOrEmpty(id))
                EquipSkillToSlot(i, id);
        }
    }
}