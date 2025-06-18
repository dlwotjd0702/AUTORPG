// 슬롯 프리팹에 붙는 UI 스크립트

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatalogSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text countText;
    public Image equippedMark;

    public string itemId;
    public Action<string> OnSlotClicked;

    public void SetSlot(string id, Sprite icon, int count, bool equipped, bool owned)
    {
        itemId = id;
        iconImage.sprite = icon;
        iconImage.color = owned ? Color.white : new Color(1, 1, 1, 0.3f);
        countText.text = count >= 0 ? $"{count}/2" : "";
        if (equippedMark) equippedMark.enabled = equipped;
    }

    // 반드시 Button OnClick에 연결!
    public void OnClick()
    {
        OnSlotClicked?.Invoke(itemId);
    }
}