using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaResultPopupUI : MonoBehaviour
{
    public Transform resultSlotParent;    // 슬롯들이 들어갈 부모(그리드패널)
    public GameObject resultSlotPrefab;   // 슬롯 프리팹
    public Button closeButton;            // 닫기 버튼

    [Header("필요 시스템 참조")]
    public InventorySystem inventorySystem; // Inspector에서 연결

    private List<GameObject> slotPool = new();

    void Awake()
    {
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false); // 처음엔 비활성화
    }

    public void Show(List<EquipmentData> results)
    {
        gameObject.SetActive(true);

        // 기존 슬롯 숨기기
        foreach (var slot in slotPool)
            slot.SetActive(false);

        // 뽑은 결과만큼 슬롯 보여주기
        for (int i = 0; i < results.Count; i++)
        {
            GameObject slotObj;
            if (i < slotPool.Count)
                slotObj = slotPool[i];
            else
            {
                slotObj = Instantiate(resultSlotPrefab, resultSlotParent);
                slotPool.Add(slotObj);
            }
            slotObj.SetActive(true);

            // 슬롯에 데이터 세팅
            var icon = slotObj.transform.Find("Icon").GetComponent<Image>();

            // ⭐️ InventorySystem 통해 id로 스프라이트 조회
            icon.sprite = inventorySystem.GetIcon(results[i].id);
        
        }
    }
}