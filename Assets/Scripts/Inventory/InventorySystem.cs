using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Linq;
using Combat;
using UnityEngine.Networking;

namespace Inventory
{
    [DefaultExecutionOrder(-100)]
    [System.Serializable]
    public class InventorySystem : MonoBehaviour, ISaveable
    {
        public int slotCount = 100;
        public Item[] slots;
        public event Action OnInventoryChanged;
        // TSV 데이터 (도감)
        public List<EquipmentData> dataList = new List<EquipmentData>();
        public Dictionary<string, EquipmentData> dataDict = new Dictionary<string, EquipmentData>();

        // 아이콘 테이블 (SO)
        public EquipmentIconTableSO iconTableSO;
        
        public const int SkillSlotCount = 6;
        public string[] equippedSkillIds = new string[SkillSlotCount];
        private SkillManager skillManager;

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

        private void Start()
        {
            
                var save = SaveManager.pendingSaveData;
                if (save != null)
                    ApplyLoadedData(save);
                
                skillManager = FindObjectOfType<SkillManager>();

        }

        public List<EquipmentData> LoadFromTSVWithCsvHelper(string path)
        {
            var list = new List<EquipmentData>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
           
            string tsvText = null;
            UnityWebRequest www = UnityWebRequest.Get(path);
            var request = www.SendWebRequest();
            while (!request.isDone) { } 
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("TSV 파일 읽기 실패 (Android): " + www.error);
                return list;
            }
            tsvText = www.downloadHandler.text;
            using (var reader = new StringReader(tsvText))
            {
                config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "\t",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim,
                    ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
                };
                using (var csv = new CsvReader(reader, config))
                {
                    list = new List<EquipmentData>(csv.GetRecords<EquipmentData>());
                }
            }

            // Windows 및 에디터 환경에서는 File.ReadAllText 사용
            if (!File.Exists(path))
            {
                Debug.LogError("TSV 파일이 존재하지 않습니다: " + path);
                return list;
            }

           

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
                    OnInventoryChanged?.Invoke();
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
                    OnInventoryChanged?.Invoke();
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
        public bool RemoveItem(string id, int amount = 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty && slots[i].itemData.id == id)
                {
                    slots[i].count -= amount;
                    if (slots[i].count <= 0)
                    {
                        slots[i].count = 0;
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
            return false;
        }

        // ===== [아래 기존 코드 동일] =====
        public bool Equip(string itemId)
        {
            var target = GetSlotById(itemId);
            if (target == null || !target.isOwned)
                return false;
            if (target.itemData.type == ItemType.weapon ||
                target.itemData.type == ItemType.armor ||
                target.itemData.type == ItemType.ring)
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
                if (slot.itemData == null || slot.itemData.type != ItemType.weapon)
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
                if (slot.itemData == null || slot.itemData.type != ItemType.armor)
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
                if (slot.itemData == null || slot.itemData.type != ItemType.ring)
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
                if (slot.isOwned && slot.itemData.type == ItemType.skill)
                    val += slot.itemData.SkillOwnedValue;
            return val;
        }
        public float GetEquippedSkillValue()
        {
            float val = 0f;
            foreach (var slot in slots)
                if (slot.isEquipped && slot.itemData.type == ItemType.skill)
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


        
        public void EquipSkill(int slotIdx, string skillId)
        {
            // 보유한 스킬만 장착 가능
            var slot = GetSlotById(skillId);
            if (slot == null || !slot.isOwned || slot.itemData.type != ItemType.skill)
                return;

            // 이미 다른 슬롯에 이 스킬이 박혀있으면 해당 슬롯 비우기
            for (int i = 0; i < equippedSkillIds.Length; i++)
                if (equippedSkillIds[i] == skillId)
                    equippedSkillIds[i] = null;

            // 지정한 슬롯에만 장착
            equippedSkillIds[slotIdx] = skillId;

            // ------ 여기서 스킬매니저도 갱신 ------
            var skillManager = FindObjectOfType<SkillManager>();
            if (skillManager != null)
                skillManager.EquipSkillToSlot(slotIdx, skillId);

            // 필요하면 인게임 UI/슬롯 등 동기화(이벤트 호출 등)
            OnInventoryChanged?.Invoke();
        }

        public List<EquipmentData> GetEquippedSkillDataList()
        {
            var list = new List<EquipmentData>();
            foreach (var id in equippedSkillIds)
            {
                if (!string.IsNullOrEmpty(id) && dataDict.TryGetValue(id, out var data))
                    list.Add(data);
            }
            return list;
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
        public void ForceRefresh()
        {
            OnInventoryChanged?.Invoke();
        }
        
        public void ApplyLoadedData(SaveData data)
        {
            if (data == null) return;

            // 기존 인벤토리 초기화
            for (int i = 0; i < slots.Length; i++)
                slots[i].Clear();

            // 저장된 인벤토리 복원
            if (data.inventorySlots != null)
            {
                foreach (var slotData in data.inventorySlots)
                {
                    var eqData = GetEquipmentData(slotData.id);
                    if (eqData != null)
                    {
                        AddItem(eqData, slotData.count);
                        var slot = GetOwnedSlotById(slotData.id);
                        if (slot != null)
                        {
                            slot.level = slotData.level;
                            slot.awakenLevel = slotData.awakenLevel;
                            slot.isOwned = slotData.isOwned;
                            slot.isEquipped = slotData.isEquipped;
                        }
                    }
                }
            }

            // --- [스킬 퀵슬롯 정보 복원] ---
            if (data.equippedSkillIds != null && data.equippedSkillIds.Count > 0)
            {
                for (int i = 0; i < equippedSkillIds.Length; i++)
                    equippedSkillIds[i] = (i < data.equippedSkillIds.Count) ? data.equippedSkillIds[i] : null;
            }

            // --- [스킬매니저 슬롯 연동 동기화] ---
            var skillManager = FindObjectOfType<SkillManager>();
            if (skillManager != null)
                skillManager.InitializeSkillsFromSlots();

            OnInventoryChanged?.Invoke();
        }

        public void CollectSaveData(SaveData data)
        {
            data.inventorySlots = new List<InventorySlotSave>();
            foreach (var slot in slots)
            {
                if (slot != null && slot.itemData != null)
                {
                    data.inventorySlots.Add(new InventorySlotSave
                    {
                        id = slot.itemData.id,
                        count = slot.count,
                        level = slot.level,
                        awakenLevel = slot.awakenLevel,
                        isOwned = slot.isOwned,
                        isEquipped = slot.isEquipped
                    });
                }
            }

            // --- [스킬 퀵슬롯 정보 저장] ---
            data.equippedSkillIds = new List<string>(equippedSkillIds);
        }
        
    }
}
