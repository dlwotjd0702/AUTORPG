using UnityEngine;
using UnityEngine.UI;

public class SkillQuickSlotUI : MonoBehaviour
{
    public Button[] slotButtons;    
    public Image[] slotImages;      
    public Inventory.InventorySystem inventory;
    private SkillManager skillManager;

    void Start()
    {
        skillManager = FindObjectOfType<SkillManager>();

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSkillSlotClicked(idx));
        }

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            string skillId = inventory.equippedSkillIds[i];
            slotImages[i].sprite = inventory.GetIcon(skillId);
            slotImages[i].gameObject.SetActive(!string.IsNullOrEmpty(skillId));
        }
    }

    void OnSkillSlotClicked(int idx)
    {
        string skillId = inventory.equippedSkillIds[idx];
        if (!string.IsNullOrEmpty(skillId))
        {
            if(skillManager != null)
            {
                skillManager.UseSkillById(skillId);
            }
            else
            {
                Debug.LogWarning("SkillManager를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"퀵슬롯 {idx}에 장착된 스킬이 없습니다.");
        }
    }
}