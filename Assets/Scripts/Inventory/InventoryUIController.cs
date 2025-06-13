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
        public Transform slotParent;                 // 인벤토리 슬롯 부모 오브젝트(Grid Layout에 할당)
        public GameObject slotPrefab;                // 슬롯 프리팹

        private List<GameObject> slotObjects = new List<GameObject>();
        [SerializeField]private Player player;

        void Start()
        {
            RefreshInventoryUI();
            // 인벤토리/장비 변경 이벤트 구독 등도 추가 가능
        }

        // 인벤토리 데이터와 UI 동기화
        public void RefreshInventoryUI()
        {
            // 기존 슬롯 UI 삭제
            foreach (var obj in slotObjects)
                Destroy(obj);
            slotObjects.Clear();

            // 인벤토리 슬롯 수만큼 프리팹 생성/초기화
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                var slot = inventory.slots[i];
                var obj = Instantiate(slotPrefab, slotParent);
                slotObjects.Add(obj);

                // 아이콘, 수량, 상태 반영
                Image iconImg = obj.transform.Find("Icon").GetComponent<Image>();
                TextMeshProUGUI countText = obj.transform.Find("CountText").GetComponent<TextMeshProUGUI>();
                Image equippedMark = obj.transform.Find("EquippedMark")?.GetComponent<Image>();

                if (slot.itemData != null)
                {
                   // iconImg.sprite = slot.itemData.icon;
                    iconImg.color = Color.white;
                    countText.text = slot.count > 1 ? slot.count.ToString() : "";
                    if (equippedMark) equippedMark.enabled = slot.isEquipped;
                }
                else
                {
                    iconImg.sprite = null;
                    iconImg.color = new Color(1,1,1,0); // 투명
                    countText.text = "";
                    if (equippedMark) equippedMark.enabled = false;
                }

                // 클릭 이벤트 (람다 캡처 주의!)
                int idx = i;
                obj.GetComponent<Button>().onClick.AddListener(() => OnSlotClick(idx));
            }
        }

        // 슬롯 클릭 처리
        public void OnSlotClick(int idx)
        {
            var slot = inventory.slots[idx];
            if (slot.itemData == null) return;

            // 장비 타입(무기/방어구/악세사리)이면 장착
            if (slot.itemData.type == ItemType.Weapon ||
                slot.itemData.type == ItemType.Armor ||
                slot.itemData.type == ItemType.Accessory)
            {
                bool equipped = inventory.Equip(slot.itemData.id);
                if (equipped)
                {
                    Debug.Log($"{slot.itemData.name} 장착됨!");
                    // 플레이어에 스탯 갱신 이벤트 호출(필요 시)
                    player.OnStatsChanged();
                }
                else
                {
                    Debug.Log($"{slot.itemData.name} 장착 실패(조건 불충분)");
                }

                RefreshInventoryUI(); // 장착마크 갱신
            }
            else
            {
                Debug.Log($"{slot.itemData.name}은(는) 장착 불가 아이템");
            }
        }

    
    }
}
