using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Stats;

public class SkillManager : MonoBehaviour
{
    public InventorySystem inventory;
    public PlayerStats playerStats;

    // 🟢 skill_01 ~ skill_20을 순서대로 드래그 (0 = skill_01, 19 = skill_20)
    public GameObject[] effectPrefabs = new GameObject[20];

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

    /// <summary>
    /// 스킬ID("skill_01"~"skill_20") → effectPrefabs[0~19] 자동 매칭
    /// </summary>
    public GameObject GetEffectPrefab(string skillId)
    {
        if (!string.IsNullOrEmpty(skillId) && skillId.StartsWith("skill_") && int.TryParse(skillId.Substring(6), out int num))
        {
            int idx = num - 1;
            if (idx >= 0 && idx < effectPrefabs.Length)
                return effectPrefabs[idx];
        }
        return null;
    }

    public void EquipSkillToSlot(int slotIdx, string skillId)
    {
        string prevSkillId = inventory.equippedSkillIds[slotIdx];
        if (!string.IsNullOrEmpty(prevSkillId))
            skillDict.Remove(prevSkillId);

        var data = inventory.GetEquipmentData(skillId);
        if (data != null)
        {
            var skill = SkillFactory.Create(playerStats.gameObject, data, this);
            skillDict[skillId] = skill;
        }
    }

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
