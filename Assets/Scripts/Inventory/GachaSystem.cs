using UnityEngine;
using Inventory;
using TMPro; // TextMeshPro
using UnityEngine.UI;
using System.Collections.Generic;

public class GachaSystem : MonoBehaviour
{
    public InventorySystem inventorySystem;

    // UI
    public GameObject popupPanel;
    public TMP_Text popupItemNameText;
    public Image popupItemIcon;

    public int maxGrade = 20;
    public float baseGachaRate = 0.6f;       // 가챠 기본 등급확률
    public float penaltyPerGrade = 0.05f;    // 등급당 하락
    public string[] typePrefixes = { "weapon", "armor", "ring", "skill" };

    // 등급판정(가챠 전용)
    public int RollGradeForGacha()
    {
        for (int grade = maxGrade; grade >= 1; grade--)
        {
            float chance = Mathf.Max(baseGachaRate - penaltyPerGrade * (grade - 1), 0f);
            if (Random.value < chance)
                return grade;
        }
        return 1; // 실패시 최소 1등급
    }

    // 실제 뽑기 (등급, 타입)
    public string RollItemIdGacha()
    {
        string type = typePrefixes[Random.Range(0, typePrefixes.Length)];
        int grade = RollGradeForGacha();
        return $"{type}_{grade:D2}";
    }

    // 1회 뽑기 (UI, 인벤토리 처리)
    public void RollGachaAndShowPopup()
    {
        string itemId = RollItemIdGacha();
        var data = inventorySystem.GetEquipmentData(itemId);
        if (data != null)
        {
            inventorySystem.AddItem(data, 1);
            ShowPopupPanel(data);
        }
        else
        {
            ShowPopupPanel(null);
        }
    }

    // 10회 뽑기 (모두 인벤토리 추가, 결과 반환)
    public List<EquipmentData> RollGacha10()
    {
        List<EquipmentData> results = new List<EquipmentData>();
        for (int i = 0; i < 10; i++)
        {
            string itemId = RollItemIdGacha();
            var data = inventorySystem.GetEquipmentData(itemId);
            if (data != null)
            {
                inventorySystem.AddItem(data, 1);
                results.Add(data);
            }
        }
        // 10회 뽑기 UI(리스트 표시 등)는 별도 구현
        return results;
    }

    // 팝업
    public void ShowPopupPanel(EquipmentData data)
    {
        if (!popupPanel) return;
        popupPanel.SetActive(true);

        if (data != null)
        {
            popupItemNameText.text = data.name;
            popupItemIcon.sprite = inventorySystem.GetIcon(data.id);
            popupItemIcon.enabled = true;
        }
        else
        {
            popupItemNameText.text = "아이템 없음!";
            popupItemIcon.enabled = false;
        }

        CancelInvoke(nameof(HidePopupPanel));
        Invoke(nameof(HidePopupPanel), 2f);
    }
    public void HidePopupPanel()
    {
        if (popupPanel) popupPanel.SetActive(false);
    }
}
