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

        [Header("패널 GameObject")] public GameObject weaponPanel;
        public GameObject armorPanel;
        public GameObject accessoryPanel;
        public GameObject skillPanel;

        [Header("슬롯 프리팹 (CatalogSlotUI 필수)")] public GameObject slotPrefab;

        [Header("슬롯 풀(패널별)")] public List<GameObject> weaponSlots = new List<GameObject>();
        public List<GameObject> armorSlots = new List<GameObject>();
        public List<GameObject> accessorySlots = new List<GameObject>();
        public List<GameObject> skillSlots = new List<GameObject>();

        [Header("오른쪽 상세패널")] public ItemDetailPanel detailPanel;

        void Start()
        {
            InitSlotPool(weaponContent, weaponSlots, inventory.GetAllOfType(ItemType.Weapon).Count);
            InitSlotPool(armorContent, armorSlots, inventory.GetAllOfType(ItemType.Armor).Count);
            InitSlotPool(accessoryContent, accessorySlots, inventory.GetAllOfType(ItemType.Accessory).Count);
            InitSlotPool(skillContent, skillSlots, inventory.GetAllOfType(ItemType.Skill).Count);

            inventory.AddItemById("weapon_01", 1);

            ShowPanel(ItemType.Skill); // 무기부터 보이게
        }

        void InitSlotPool(Transform parent, List<GameObject> pool, int count)
        {
            pool.Clear();
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(slotPrefab, parent);
                pool.Add(obj);
                obj.SetActive(false);

                // 슬롯 프리팹에 반드시 CatalogSlotUI 붙어야 함
                var slotUI = obj.GetComponent<CatalogSlotUI>();
                if (slotUI == null)
                    Debug.LogError("slotPrefab에 CatalogSlotUI 컴포넌트를 붙이세요!");

                // 중복 리스너 방지
                slotUI.OnSlotClicked = null;
                slotUI.OnSlotClicked += OnCatalogSlotClicked;
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
                case ItemType.Weapon:
                    RefreshPanel(weaponSlots, inventory.GetAllOfType(ItemType.Weapon), type); break;
                case ItemType.Armor:
                    RefreshPanel(armorSlots, inventory.GetAllOfType(ItemType.Armor), type); break;
                case ItemType.Accessory:
                    RefreshPanel(accessorySlots, inventory.GetAllOfType(ItemType.Accessory), type); break;
                case ItemType.Skill:
                    RefreshPanel(skillSlots, inventory.GetAllOfType(ItemType.Skill), type); break;
            }
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

        void OnCatalogSlotClicked(string id)
        {
            // 오른쪽 상세 패널 활성화 및 정보 갱신
            detailPanel.ShowDetail(id);
        }
        
        // 무기 카테고리 버튼 클릭용
        public void OnWeaponCategoryButtonClicked()  { ShowPanel(ItemType.Weapon); }

        public void testButtonClicked()
        {
            inventory.AddItemById("weapon_01", 1);
            inventory.AddItemById("armor_01", 1);
            inventory.AddItemById("ring_01", 1);
            inventory.AddItemById("skill_01", 1);
        }

// 방어구 카테고리 버튼 클릭용
        public void OnArmorCategoryButtonClicked()   { ShowPanel(ItemType.Armor); }

// 악세서리 카테고리 버튼 클릭용
        public void OnAccessoryCategoryButtonClicked() { ShowPanel(ItemType.Accessory); }

// 스킬 카테고리 버튼 클릭용
        public void OnSkillCategoryButtonClicked()   { ShowPanel(ItemType.Skill); }

    }
    
    

}

