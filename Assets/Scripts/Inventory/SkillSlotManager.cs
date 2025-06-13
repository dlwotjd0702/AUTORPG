using Inventory;
using UnityEngine;

public class SkillSlotManager : MonoBehaviour
{
    public InventorySystem inventory;
    public int skillSlotCount = 6;
    public InventorySlot[] skillSlots;

    private void Awake()
    {
        skillSlots = new InventorySlot[skillSlotCount];
        for (int i = 0; i < skillSlotCount; i++)
            skillSlots[i] = new InventorySlot();
    }

    // 슬롯번호, 스킬id로 장착 (보유중인 스킬만)
    public bool EquipSkill(int slotIdx, string skillId)
    {
        var slot = inventory.GetSlotById(skillId);
        if (slot == null || !slot.isOwned || slot.itemData.type != ItemType.Skill)
            return false;

        skillSlots[slotIdx].itemData = slot.itemData;
        skillSlots[slotIdx].isOwned = true;
        skillSlots[slotIdx].isEquipped = true;
        return true;
    }
}