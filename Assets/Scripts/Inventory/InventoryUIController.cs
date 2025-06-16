using System;
using System.Collections.Generic;
using IdleRPG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryUIController : MonoBehaviour
    {
        public InventorySystem inventory;
        public Player player;

        [Header("패널별 Content(ScrollView의 Content)")]
        public Transform weaponContent;
        public Transform armorContent;
        public Transform accessoryContent;
        public Transform skillContent;

        [Header("패널 GameObject")]
        public GameObject weaponPanel;
        public GameObject armorPanel;
        public GameObject accessoryPanel;
        public GameObject skillPanel;

        [Header("슬롯 프리팹")]
        public GameObject slotPrefab;

        [Header("슬롯 풀(패널별)")]
        public List<GameObject> weaponSlots = new List<GameObject>();
        public List<GameObject> armorSlots = new List<GameObject>();
        public List<GameObject> accessorySlots = new List<GameObject>();
        public List<GameObject> skillSlots = new List<GameObject>();

        void Start()
        {
            // (슬롯 풀 초기화는 그대로)
            InitSlotPool(weaponContent, weaponSlots, inventory.GetAllOfType(ItemType.Weapon).Count);
            InitSlotPool(armorContent, armorSlots, inventory.GetAllOfType(ItemType.Armor).Count);
            InitSlotPool(accessoryContent, accessorySlots, inventory.GetAllOfType(ItemType.Accessory).Count);
            InitSlotPool(skillContent, skillSlots, inventory.GetAllOfType(ItemType.Skill).Count);

            // ===== 여기 추가! =====
            inventory.AddItemById("weapon_01", 1);

            ShowPanel(ItemType.Weapon); // 무기부터 보이게
        }

        void InitSlotPool(Transform parent, List<GameObject> pool, int count)
        {
            pool.Clear();
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(slotPrefab, parent);
                pool.Add(obj);
                obj.SetActive(false); // 시작 시 전부 숨김
                int idx = i;
                obj.GetComponent<Button>().onClick.AddListener(() => OnCatalogSlotClick(parent, idx));
            }
        }

        public void ShowPanel(ItemType type)
        {
            weaponPanel.SetActive(type == ItemType.Weapon);
            armorPanel.SetActive(type == ItemType.Armor);
            accessoryPanel.SetActive(type == ItemType.Accessory);
            skillPanel.SetActive(type == ItemType.Skill);

            switch (type)
            {
                case ItemType.Weapon:     RefreshPanel(weaponSlots, inventory.GetAllOfType(ItemType.Weapon), type); break;
                case ItemType.Armor:      RefreshPanel(armorSlots, inventory.GetAllOfType(ItemType.Armor), type); break;
                case ItemType.Accessory:  RefreshPanel(accessorySlots, inventory.GetAllOfType(ItemType.Accessory), type); break;
                case ItemType.Skill:      RefreshPanel(skillSlots, inventory.GetAllOfType(ItemType.Skill), type); break;
            }
        }

        void RefreshPanel(List<GameObject> slotObjs, List<EquipmentData> catalog, ItemType type)
        {
            for (int i = 0; i < slotObjs.Count; i++)
            {
                if (i < catalog.Count)
                {
                    slotObjs[i].SetActive(true);
                    UpdateCatalogSlotUI(slotObjs[i], catalog[i], type);
                }
                else
                {
                    slotObjs[i].SetActive(false);
                }
            }
        }

        void UpdateCatalogSlotUI(GameObject obj, EquipmentData data, ItemType type)
        {
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var countText = obj.transform.Find("CountText").GetComponent<TextMeshProUGUI>();
            var equipMark = obj.transform.Find("EquippedMark")?.GetComponent<Image>();

            icon.sprite = inventory.GetIcon(data.id);
            bool owned = inventory.IsOwned(data.id);
            int count = inventory.GetOwnedCount(data.id);
            icon.color = owned ? Color.white : new Color(1,1,1,0.3f); // 보유: 선명, 미보유: 불투명
            countText.text = $"{count}/2";
            if (equipMark) equipMark.enabled = owned && inventory.GetSlotById(data.id)?.isEquipped == true;

            // 슬롯에 EquipmentData 연결 (필요시)
            obj.GetComponent<SlotLink>()?.SetEquipment(data);
        }

        void OnCatalogSlotClick(Transform parent, int idx)
        {
            ItemType type;
            List<EquipmentData> catalog = null;
            if (parent == weaponContent)      { type = ItemType.Weapon;     catalog = inventory.GetAllOfType(type); }
            else if (parent == armorContent)  { type = ItemType.Armor;      catalog = inventory.GetAllOfType(type); }
            else if (parent == accessoryContent) { type = ItemType.Accessory; catalog = inventory.GetAllOfType(type); }
            else if (parent == skillContent)  { type = ItemType.Skill;      catalog = inventory.GetAllOfType(type); }
            else return;

            if (idx >= catalog.Count) return;
            var data = catalog[idx];
            bool owned = inventory.IsOwned(data.id);

            if (!owned)
            {
                Debug.Log($"{data.name}은(는) 아직 미보유.");
                // 팝업, 획득 경로 안내 등 추가 가능
                return;
            }

            // 장비라면 장착
            if (type == ItemType.Weapon || type == ItemType.Armor || type == ItemType.Accessory)
            {
                bool equipped = inventory.Equip(data.id);
                if (equipped)
                {
                    Debug.Log($"{data.name} 장착됨!");
                    player?.OnStatsChanged();
                }
                else
                {
                    Debug.Log($"{data.name} 장착 실패(조건 불충분)");
                }
                // 해당 패널만 다시 갱신
                ShowPanel(type);
            }
            else
            {
                // 스킬은 추후 추가
            }
        }
    }

    // 도감용 슬롯(EquipmentData 연결)
    public class SlotLink : MonoBehaviour
    {
        public EquipmentData linkedData;
        public void SetEquipment(EquipmentData data) { linkedData = data; }
    }
}
