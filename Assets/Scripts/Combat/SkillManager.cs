using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class SkillManager : MonoBehaviour
{
    private InventorySystem inventory;
    private Dictionary<string, SkillBase> skillDict = new Dictionary<string, SkillBase>();

    private void Awake()
    {
        inventory = GetComponent<InventorySystem>();
    }

    public void InitializeSkills()
    {
        skillDict.Clear();
        foreach (var data in inventory.dataList)
        {
            if (data.type == ItemType.skill)
            {
                SkillBase skill;
                if (data.skillType == SkillType.Active)
                    skill = new ActiveSkill(data);
                else
                    skill = new PassiveSkill(data);

                skillDict[data.id] = skill;
            }
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var skill in skillDict.Values)
        {
            skill.UpdateCooldown(dt);
        }
    }

    public void UseSkillById(string skillId)
    {
        if (skillDict.TryGetValue(skillId, out var skill))
        {
            skill.TryUseSkill();
        }
        else
        {
            Debug.LogWarning($"SkillManager: 스킬 ID '{skillId}'를 찾을 수 없습니다.");
        }
    }
}