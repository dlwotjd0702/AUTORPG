using System.Collections.Generic;
using Combat;
using UnityEngine;
using Inventory;
using Stats;

public class SkillManager : MonoBehaviour
{
    public InventorySystem inventory;
    public PlayerStats playerStats;

    public GameObject[] effectPrefabs = new GameObject[21];
    public Dictionary<string, SkillBase> skillDict = new();

    private SkillQuickSlotUI slotUI;
    private float[] readySkillDelay;
    private bool isSkillCasting = false;
    private float castingTimer = 0f;
    private float skillGlobalDelay = 0f;

    private void Awake()
    {
        if (!inventory) inventory = FindObjectOfType<InventorySystem>();
        if (!playerStats) playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        slotUI = FindObjectOfType<SkillQuickSlotUI>();
        int slotLen = inventory.equippedSkillIds.Length;
        readySkillDelay = new float[slotLen];
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var skill in skillDict.Values)
            skill.UpdateCooldown(dt);

        // 오토스킬 전체 적용
        if (slotUI != null && slotUI.autoSkillEnabled)
        {
            if (skillGlobalDelay > 0f)
            {
                skillGlobalDelay -= dt;
                return;
            }
            if (isSkillCasting)
            {
                castingTimer -= dt;
                if (castingTimer <= 0f)
                    isSkillCasting = false;
                else
                    return;
            }
            for (int i = 0; i < inventory.equippedSkillIds.Length; i++)
            {
                string skillId = inventory.equippedSkillIds[i];
                if (string.IsNullOrEmpty(skillId)) continue;
                if (!skillDict.TryGetValue(skillId, out var skill)) continue;
                if (!skill.IsReady()) continue;

                // 준비된 스킬의 지연
                if (readySkillDelay[i] > 0f)
                {
                    readySkillDelay[i] -= dt;
                    continue;
                }

                skill.TryUseSkill();
                isSkillCasting = true;
                castingTimer = 0.5f;
                skillGlobalDelay = 0.5f;

                // 사용된 슬롯만 0, 나머지는 0.5초로 초기화
                for (int j = 0; j < readySkillDelay.Length; j++)
                    readySkillDelay[j] = (j == i) ? 0f : 0.5f;

                break; // 한 번에 하나만 발동
            }
        }
        else
        {
            isSkillCasting = false;
            castingTimer = 0f;
            skillGlobalDelay = 0f;
            if (readySkillDelay != null)
                for (int j = 0; j < readySkillDelay.Length; j++)
                    readySkillDelay[j] = 0f;
        }
    }

    public void UseSkillById(string skillId)
    {
        if (skillDict.TryGetValue(skillId, out var skill))
            skill.TryUseSkill();
        else
            Debug.LogWarning($"SkillManager: 스킬 ID '{skillId}'를 찾을 수 없습니다.");
    }

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
        var slotUI = GameObject.FindObjectOfType<SkillQuickSlotUI>();
        if (slotUI != null)
            slotUI.Refresh();
    }
}
