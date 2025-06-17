using UnityEngine;
using UnityEngine.UI;

public class SkillQuickSlotUI : MonoBehaviour
{
    public Button[] slotButtons;     // Inspector에서 Button 4개 연결
    public Image[] slotImages;       // Button의 자식 Image 연결
    public Inventory.InventorySystem inventory;

    void Start()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSkillSlotClicked(idx));
        }
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
            // 실제로 스킬 사용 로직 호출!
            Debug.Log($"스킬 {skillId} 사용!");
            // ex) PlayerSkillController.Instance.UseSkill(idx, skillId);
        }
    }
}