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

        [Header("슬롯 풀 개수(최대)")]
        public int slotCountPerPanel = 20;

        void Start()
        {
            InitSlotPool(weaponContent, weaponSlots, slotCountPerPanel);
            InitSlotPool(armorContent, armorSlots, slotCountPerPanel);
            InitSlotPool(accessoryContent, accessorySlots, slotCountPerPanel);
            InitSlotPool(skillContent, skillSlots, slotCountPerPanel);

            ShowPanel(ItemType.Weapon);
        }

        void InitSlotPool(Transform parent, List<GameObject> pool, int count)
        {
            pool.Clear();
            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(slotPrefab, parent);
                pool.Add(obj);
                obj.SetActive(false); // 시작 시 전부 숨김
                // 슬롯 클릭 이벤트 등록
                int idx = i;
                obj.GetComponent<Button>().onClick.AddListener(() => OnSlotButtonClick(parent, idx));
            }
        }

        // 분류 버튼 등에서 호출
        public void ShowPanel(ItemType type)
        {
            weaponPanel.SetActive(type == ItemType.Weapon);
            armorPanel.SetActive(type == ItemType.Armor);
            accessoryPanel.SetActive(type == ItemType.Accessory);
            skillPanel.SetActive(type == ItemType.Skill);

            switch (type)
            {
                case ItemType.Weapon:     RefreshWeaponPanel(); break;
                case ItemType.Armor:      RefreshArmorPanel(); break;
                case ItemType.Accessory:  RefreshAccessoryPanel(); break;
                case ItemType.Skill:      RefreshSkillPanel(); break;
            }
        }

        // 인벤토리에서 해당 타입만 필터링
        List<InventorySlot> GetSlotsByType(ItemType type)
        {
            List<InventorySlot> result = new List<InventorySlot>();
            foreach (var slot in inventory.slots)
                if (slot.itemData != null && slot.itemData.type == type)
                    result.Add(slot);
            return result;
        }

        void RefreshWeaponPanel()     => RefreshPanel(weaponSlots, GetSlotsByType(ItemType.Weapon));
        void RefreshArmorPanel()      => RefreshPanel(armorSlots, GetSlotsByType(ItemType.Armor));
        void RefreshAccessoryPanel()  => RefreshPanel(accessorySlots, GetSlotsByType(ItemType.Accessory));
        void RefreshSkillPanel()      => RefreshPanel(skillSlots, GetSlotsByType(ItemType.Skill));

        // 핵심: 슬롯 풀을 데이터만큼만 활성화하고 내용 갱신, 나머지는 숨김
        void RefreshPanel(List<GameObject> slotObjs, List<InventorySlot> slotDatas)
        {
            for (int i = 0; i < slotObjs.Count; i++)
            {
                if (i < slotDatas.Count)
                {
                    slotObjs[i].SetActive(true);
                    UpdateSlotUI(slotObjs[i], slotDatas[i]);
                }
                else
                {
                    slotObjs[i].SetActive(false);
                }
            }
        }

        void UpdateSlotUI(GameObject obj, InventorySlot slot)
        {
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var countText = obj.transform.Find("CountText").GetComponent<TextMeshProUGUI>();
            var equipMark = obj.transform.Find("EquippedMark")?.GetComponent<Image>();

            icon.sprite = inventory.GetIcon(slot.itemData.id);
            icon.color = Color.white;
            countText.text = slot.count > 1 ? slot.count.ToString() : "";
            if (equipMark) equipMark.enabled = slot.isEquipped;

            // 슬롯에 InventorySlot 참조 연결 (컴포넌트 추가도 가능)
            obj.GetComponent<SlotLink>()?.SetSlot(slot); // 확장 사용 가능
        }

        // 슬롯 버튼 클릭 처리 (parent로 어떤 패널인지 구분)
        void OnSlotButtonClick(Transform parent, int idx)
        {
            List<GameObject> slotPool = null;
            ItemType type = ItemType.Weapon;
            if (parent == weaponContent)      { slotPool = weaponSlots; type = ItemType.Weapon; }
            else if (parent == armorContent)  { slotPool = armorSlots; type = ItemType.Armor; }
            else if (parent == accessoryContent) { slotPool = accessorySlots; type = ItemType.Accessory; }
            else if (parent == skillContent)  { slotPool = skillSlots; type = ItemType.Skill; }
            else return;

            var slotDataList = GetSlotsByType(type);
            if (idx < slotDataList.Count)
            {
                var slot = slotDataList[idx];
                if (slot.itemData == null) return;

                // 무기/방어구/악세 장착처리
                if (slot.itemData.type == ItemType.Weapon ||
                    slot.itemData.type == ItemType.Armor ||
                    slot.itemData.type == ItemType.Accessory)
                {
                    bool equipped = inventory.Equip(slot.itemData.id);
                    if (equipped)
                    {
                        Debug.Log($"{slot.itemData.name} 장착됨!");
                        player?.OnStatsChanged();
                    }
                    else
                    {
                        Debug.Log($"{slot.itemData.name} 장착 실패(조건 불충분)");
                    }
                    // 해당 패널만 다시 갱신!
                    switch (type)
                    {
                        case ItemType.Weapon:     RefreshWeaponPanel(); break;
                        case ItemType.Armor:      RefreshArmorPanel(); break;
                        case ItemType.Accessory:  RefreshAccessoryPanel(); break;
                    }
                }
                else
                {
                    Debug.Log($"{slot.itemData.name}은(는) 장착 불가 아이템");
                }
            }
        }
    }

    // [선택사항] 슬롯과 InventorySlot을 연결하고 싶은 경우
    public class SlotLink : MonoBehaviour
    {
        public InventorySlot linkedSlot;
        public void SetSlot(InventorySlot slot) { linkedSlot = slot; }
    }
}
