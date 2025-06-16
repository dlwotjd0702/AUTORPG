using UnityEngine;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Linq;

namespace Inventory
{
    [DefaultExecutionOrder(-100)]
    [System.Serializable]
    public class InventorySystem : MonoBehaviour
    {
        public int slotCount = 100;
        public Item[] slots;

        // TSV 데이터 (도감)
        public List<EquipmentData> dataList = new List<EquipmentData>();
        public Dictionary<string, EquipmentData> dataDict = new Dictionary<string, EquipmentData>();

        // 아이콘 테이블 (SO)
        public EquipmentIconTableSO iconTableSO;
        
        public const int SkillSlotCount = 4;
        public string[] equippedSkillIds = new string[SkillSlotCount];

        private void Awake()
        {
            // 슬롯 초기화 (보유 슬롯)
            slots = new Item[slotCount];
            for (int i = 0; i < slotCount; i++)
                slots[i] = new Item();

            // TSV 파싱 (CsvHelper 사용)
            string path = Path.Combine(Application.streamingAssetsPath, "equipment.tsv");
            dataList = LoadFromTSVWithCsvHelper(path);

            dataDict.Clear();
            foreach (var data in dataList)
                dataDict[data.id] = data;
        }

        public List<EquipmentData> LoadFromTSVWithCsvHelper(string path)
        {
            var list = new List<EquipmentData>();
            if (!File.Exists(path))
            {
                Debug.LogError("TSV 파일이 존재하지 않습니다: " + path);
                return list;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
            };

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {
                list = new List<EquipmentData>(csv.GetRecords<EquipmentData>());
            }
            return list;
        }
        

        public EquipmentData GetEquipmentData(string id)
        {
            dataDict.TryGetValue(id, out var data);
            return data;
        }
        public Sprite GetIcon(string id)
        {
            return iconTableSO ? iconTableSO.GetSprite(id) : null;
        }
        public Item GetOwnedSlotById(string id)
        {
            foreach (var slot in slots)
                if (slot.itemData != null && slot.itemData.id == id && slot.isOwned)
                    return slot;
            return null;
        }

        // ===== [도감 기능] =====
        public List<EquipmentData> GetAllOfType(ItemType type)
        {
            return dataList.Where(x => x.type == type).ToList();
        }

        // ===== [보유 체크] =====
        public bool IsOwned(string id)
        {
            foreach (var slot in slots)
                if (slot.itemData != null && slot.itemData.id == id && slot.isOwned)
                    return true;
            return false;
        }
        public int GetOwnedCount(string id)
        {
            foreach (var slot in slots)
                if (slot.itemData != null && slot.itemData.id == id && slot.isOwned)
                    return slot.count;
            return 0;
        }

        // ===== [보유/획득 시스템(기존)] =====
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
        public bool AddItemById(string id, int amount = 1)
        {
            var data = GetEquipmentData(id);
            if (data == null)
            {
                Debug.LogError("[Inventory] 알 수 없는 아이템 id: " + id);
                return false;
            }
            return AddItem(data, amount);
        }

        // ===== [아래 기존 코드 동일] =====
        public bool Equip(string itemId)
        {
            var target = GetSlotById(itemId);
            if (target == null || !target.isOwned)
                return false;
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

        public (float atkMul, float atkSpdMul) GetWeaponMultipliers()
        {
            float atk = 0f, atkSpd = 0f;
            foreach (var slot in slots)
            {
                if (slot.itemData == null || slot.itemData.type != ItemType.Weapon)
                    continue;
                if (slot.isOwned)
                {
                    atk += slot.itemData.OwnedAtkPercent;
                    atkSpd += slot.itemData.OwnedAtkSpdPercent;
                }
                if (slot.isEquipped)
                {
                    atk += slot.itemData.EquipAtkPercent;
                    atkSpd += slot.itemData.EquipAtkSpdPercent;
                }
            }
            return (1f + atk, 1f + atkSpd);
        }

        public (float defMul, float hpMul) GetArmorMultipliers()
        {
            float def = 0f, hp = 0f;
            foreach (var slot in slots)
            {
                if (slot.itemData == null || slot.itemData.type != ItemType.Armor)
                    continue;
                if (slot.isOwned)
                {
                    def += slot.itemData.OwnedDefPercent;
                    hp += slot.itemData.OwnedHpPercent;
                }
                if (slot.isEquipped)
                {
                    def += slot.itemData.EquipDefPercent;
                    hp += slot.itemData.EquipHpPercent;
                }
            }
            return (1f + def, 1f + hp);
        }

        public (float critRateMul, float critDmgMul) GetAccessoryMultipliers()
        {
            float critRate = 0f, critDmg = 0f;
            foreach (var slot in slots)
            {
                if (slot.itemData == null || slot.itemData.type != ItemType.Accessory)
                    continue;
                if (slot.isOwned)
                {
                    critRate += slot.itemData.OwnedCritRatePercent;
                    critDmg += slot.itemData.OwnedCritDmgPercent;
                }
                if (slot.isEquipped)
                {
                    critRate += slot.itemData.EquipCritRatePercent;
                    critDmg += slot.itemData.EquipCritDmgPercent;
                }
            }
            return (1f + critRate, 1f + critDmg);
        }

        public float GetOwnedSkillValue()
        {
            float val = 0f;
            foreach (var slot in slots)
                if (slot.isOwned && slot.itemData.type == ItemType.Skill)
                    val += slot.itemData.SkillOwnedValue;
            return val;
        }
        public float GetEquippedSkillValue()
        {
            float val = 0f;
            foreach (var slot in slots)
                if (slot.isEquipped && slot.itemData.type == ItemType.Skill)
                    val += slot.itemData.SkillEquipValue;
            return val;
        }

        public Item GetSlotById(string id)
        {
            foreach (var slot in slots)
                if (!slot.IsEmpty && slot.itemData.id == id)
                    return slot;
            return null;
        }

        public static string GetNextId(string id)
        {
            int underscoreIdx = id.LastIndexOf('_');
            if (underscoreIdx < 0) return id;
            string prefix = id.Substring(0, underscoreIdx + 1);
            string numStr = id.Substring(underscoreIdx + 1);
            if (int.TryParse(numStr, out int num))
                return $"{prefix}{(num + 1):D2}";
            else
                return id;
        }

        public bool TryCombine(string itemId, int requireCount = 2)
        {
            int totalCount = 0;
            Item targetSlot = null;
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
            string nextId = GetNextId(baseData.id);
            EquipmentData newItemData = GetEquipmentData(nextId);

            if (newItemData == null)
            {
                Debug.LogError("[Combine] 다음 등급 id가 도감에 없음: " + nextId);
                return false;
            }

            RemoveItem(itemId, requireCount);
            AddItem(newItemData, 1);

            return true;
        }


        public bool RemoveItem(string id, int amount = 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemData.id == id)
                {
                    slots[i].count -= amount;
                    if (slots[i].count <= 0)
                        //slots[i].Clear();
                        return true;
                }
            }
            return false;
        }
        public void EquipSkill(int slotIdx, string skillId)
        {
            equippedSkillIds[slotIdx] = skillId;
            // 필요하면: 인게임 UI/슬롯 등과 연동(이벤트, 직접 호출 등)
        }

        public void EnhanceSkill(string skillId)
        {
            var item = GetSlotById(skillId);
            if (item != null) item.level++; // 강화 예시
        }

        public void CombineSkill(string skillId)
        {
            // Combine 로직: TryCombine 또는 맞는 함수 호출
            TryCombine(skillId, 2);
        }
    }
}
