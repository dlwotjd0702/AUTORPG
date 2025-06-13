using UnityEngine;

namespace Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        public int slotCount = 100;
        public InventorySlot[] slots;

        private void Awake()
        {
            slots = new InventorySlot[slotCount];
            for (int i = 0; i < slotCount; i++)
                slots[i] = new InventorySlot();
        }

        // --------- 장비/스킬 획득 ---------
        public bool AddItem(EquipmentData item, int amount = 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemData.id == item.id)
                {
                    slots[i].count += amount;
                    slots[i].isOwned = true;
                    return true;
                }
            }
            // 빈칸에 신규
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                {
                    slots[i].itemData = item;
                    slots[i].count = amount;
                    slots[i].isOwned = true;
                    return true;
                }
            }
            return false; // 인벤토리 풀
        }

        // --------- 장비 장착(종류별 1개만) ---------
        public bool Equip(string itemId)
        {
            var target = GetSlotById(itemId);
            if (target == null || !target.isOwned)
                return false;

            // 종류별 1개만 장착
            if (target.itemData.type == ItemType.Weapon ||
                target.itemData.type == ItemType.Armor ||
                target.itemData.type == ItemType.Accessory)
            {
                foreach (var slot in slots)
                    if (slot.isEquipped && slot.itemData.type == target.itemData.type)
                        slot.isEquipped = false;
                target.isEquipped = true;
                return true;
            }
            return false;
        }

        public bool Unequip(ItemType type)
        {
            foreach (var slot in slots)
                if (slot.isEquipped && slot.itemData.type == type)
                {
                    slot.isEquipped = false;
                    return true;
                }
            return false;
        }

        // ============= [1] 무기: 공격력, 공격속도 =============
        public (float atkMul, float atkSpdMul) GetWeaponMultipliers()
        {
            float atk = 0f, atkSpd = 0f;
            foreach (var slot in slots)
            {
                if (slot.itemData == null || slot.itemData.type != ItemType.Weapon)
                    continue;
                if (slot.isOwned)
                {
                    atk += slot.itemData.ownedAtkPercent;
                    atkSpd += slot.itemData.ownedAtkSpdPercent;
                }
                if (slot.isEquipped)
                {
                    atk += slot.itemData.equipAtkPercent;
                    atkSpd += slot.itemData.equipAtkSpdPercent;
                }
            }
            return (1f + atk, 1f + atkSpd);
        }

        // ============= [2] 방어구: 방어력, 체력 =============
        public (float defMul, float hpMul) GetArmorMultipliers()
        {
            float def = 0f, hp = 0f;
            foreach (var slot in slots)
            {
                if (slot.itemData == null || slot.itemData.type != ItemType.Armor)
                    continue;
                if (slot.isOwned)
                {
                    def += slot.itemData.ownedDefPercent;
                    hp  += slot.itemData.ownedHpPercent;
                }
                if (slot.isEquipped)
                {
                    def += slot.itemData.equipDefPercent;
                    hp  += slot.itemData.equipHpPercent;
                }
            }
            return (1f + def, 1f + hp);
        }

        // ============= [3] 악세: 크리확률, 크리뎀 =============
        public (float critRateMul, float critDmgMul) GetAccessoryMultipliers()
        {
            float critRate = 0f, critDmg = 0f;
            foreach (var slot in slots)
            {
                if (slot.itemData == null || slot.itemData.type != ItemType.Accessory)
                    continue;
                if (slot.isOwned)
                {
                    critRate += slot.itemData.ownedCritRatePercent;
                    critDmg  += slot.itemData.ownedCritDmgPercent;
                }
                if (slot.isEquipped)
                {
                    critRate += slot.itemData.equipCritRatePercent;
                    critDmg  += slot.itemData.equipCritDmgPercent;
                }
            }
            return (1f + critRate, 1f + critDmg);
        }

        // --------- 스킬 효과 집계(더하기) ---------
        public float GetOwnedSkillValue()
        {
            float val = 0f;
            foreach (var slot in slots)
                if (slot.isOwned && slot.itemData.type == ItemType.Skill)
                    val += slot.itemData.skillOwnedValue;
            return val;
        }
        public float GetEquippedSkillValue()
        {
            float val = 0f;
            foreach (var slot in slots)
                if (slot.isEquipped && slot.itemData.type == ItemType.Skill)
                    val += slot.itemData.skillEquipValue;
            return val;
        }

        // --------- 기타 ---------
        public InventorySlot GetSlotById(string id)
        {
            foreach (var slot in slots)
                if (!slot.IsEmpty && slot.itemData.id == id)
                    return slot;
            return null;
        }

        // --------- 합성 ---------
        public bool TryCombine(string itemId, int requireCount = 2)
        {
            int totalCount = 0;
            InventorySlot targetSlot = null;
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemData.id == itemId)
                {
                    targetSlot = slots[i];
                    totalCount += slots[i].count;
                }
            }
            if (targetSlot == null || totalCount < requireCount) return false;

            EquipmentData baseData = targetSlot.itemData;
            EquipmentData newItem = baseData.Clone();
            newItem.grade += 1;
            newItem.name = baseData.name + " +" + newItem.grade;
            // 실제 퍼센트값은 외부 데이터베이스에서 참조하는 게 더 확장성 좋음

            RemoveItem(itemId, requireCount);
            AddItem(newItem, 1);

            return true;
        }

        // --------- 아이템 제거 ---------
        public bool RemoveItem(string id, int amount = 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemData.id == id)
                {
                    slots[i].count -= amount;
                    if (slots[i].count <= 0)
                        slots[i].Clear();
                    return true;
                }
            }
            return false;
        }
    }
}
