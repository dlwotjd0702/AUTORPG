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

        [Header("슬롯 프리팹 (CatalogSlotUI 필수)")]
        public GameObject slotPrefab;

        [Header("슬롯 풀(패널별)")]
        public List<GameObject> weaponSlots = new List<GameObject>();
        public List<GameObject> armorSlots = new List<GameObject>();
        public List<GameObject> accessorySlots = new List<GameObject>();
        public List<GameObject> skillSlots = new List<GameObject>();

        [Header("상세패널(무기/방어구/악세)")]
        public ItemDetailPanel itemDetailPanel; // 무기/방어구/악세서리용(공용 or 분리)
        [Header("상세패널(스킬)")]
        public SkillDetailPanel skillDetailPanel;

        private ItemType currentPanelType = ItemType.Weapon;
        
        void Start()
        {
            InitSlotPool(weaponContent, weaponSlots, inventory.GetAllOfType(ItemType.Weapon).Count, OnWeaponSlotClicked);
            InitSlotPool(armorContent, armorSlots, inventory.GetAllOfType(ItemType.Armor).Count, OnArmorSlotClicked);
            InitSlotPool(accessoryContent, accessorySlots, inventory.GetAllOfType(ItemType.Accessory).Count, OnAccessorySlotClicked);
            InitSlotPool(skillContent, skillSlots, inventory.GetAllOfType(ItemType.Skill).Count, OnSkillSlotClicked);

            inventory.AddItemById("weapon_01", 1);

            ShowPanel(ItemType.Weapon); // 무기부터 보이게
            inventory.OnInventoryChanged += () => ShowPanel(currentPanelType);
           
        }

        void InitSlotPool(Transform parent, List<GameObject> pool, int count, Action<string> onClickFunc)
        {
            pool.Clear();
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(slotPrefab, parent);
                pool.Add(obj);
                obj.SetActive(false);

                var slotUI = obj.GetComponent<CatalogSlotUI>();
                if (slotUI == null)
                    Debug.LogError("slotPrefab에 CatalogSlotUI 컴포넌트를 붙이세요!");

                slotUI.OnSlotClicked = null;
                slotUI.OnSlotClicked += onClickFunc;
            }
        }

        public void ShowPanel(ItemType type)
        {
            weaponPanel.SetActive(type == ItemType.Weapon);
            armorPanel.SetActive(type == ItemType.Armor);
            accessoryPanel.SetActive(type == ItemType.Accessory);
            skillPanel.SetActive(type == ItemType.Skill);

            // 상세 패널 숨기기(초기화)
            itemDetailPanel?.gameObject.SetActive(false);
            skillDetailPanel?.gameObject.SetActive(false);

            switch (type)
            {
                case ItemType.Weapon:
                    RefreshPanel(weaponSlots, inventory.GetAllOfType(ItemType.Weapon), type); break;
                case ItemType.Armor:
                    RefreshPanel(armorSlots, inventory.GetAllOfType(ItemType.Armor), type); break;
                case ItemType.Accessory:
                    RefreshPanel(accessorySlots, inventory.GetAllOfType(ItemType.Accessory), type); break;
                case ItemType.Skill:
                    RefreshPanel(skillSlots, inventory.GetAllOfType(ItemType.Skill), type); break;
            }
            currentPanelType = type;
        }

        void RefreshPanel(List<GameObject> slotObjs, List<EquipmentData> catalog, ItemType type)
        {
            for (int i = 0; i < slotObjs.Count; i++)
            {
                if (i < catalog.Count)
                {
                    slotObjs[i].SetActive(true);
                    var slotUI = slotObjs[i].GetComponent<CatalogSlotUI>();
                    var data = catalog[i];
                    bool owned = inventory.IsOwned(data.id);
                    int count = inventory.GetOwnedCount(data.id);
                    bool equipped = inventory.GetSlotById(data.id)?.isEquipped == true;

                    slotUI.SetSlot(
                        data.id,
                        inventory.GetIcon(data.id),
                        count,
                        equipped,
                        owned
                    );
                }
                else
                {
                    slotObjs[i].SetActive(false);
                }
            }
        }

        // 무기/방어구/악세 클릭 → 공용 상세패널
        void OnWeaponSlotClicked(string id)
        {
            itemDetailPanel.ShowDetail(id);
        }
        void OnArmorSlotClicked(string id)
        {
            itemDetailPanel.ShowDetail(id);
        }
        void OnAccessorySlotClicked(string id)
        {
            itemDetailPanel.ShowDetail(id);
        }
        // 스킬 클릭 → 별도 패널
        void OnSkillSlotClicked(string id)
        {
            skillDetailPanel.ShowDetail(id);
        }

        // 카테고리 버튼
        public void OnWeaponCategoryButtonClicked() { ShowPanel(ItemType.Weapon); }
        public void OnArmorCategoryButtonClicked() { ShowPanel(ItemType.Armor); }
        public void OnAccessoryCategoryButtonClicked() { ShowPanel(ItemType.Accessory); }
        public void OnSkillCategoryButtonClicked() { ShowPanel(ItemType.Skill); }

        public void testButtonClicked()
        {
            inventory.AddItemById("weapon_01", 1);
            inventory.AddItemById("armor_01", 1);
            inventory.AddItemById("ring_01", 1);
            inventory.AddItemById("skill_01", 1);
        }
    }
}
