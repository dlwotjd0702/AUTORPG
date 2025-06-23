using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GachaPanelUI : MonoBehaviour
{
    [Header("이 패널의 가챠 타입")]
    public ItemType panelType;

    [Header("UI 오브젝트")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI dropRateText;
    public Button singleRollButton;
    public Button tenRollButton;
    public Slider gachaExpSlider;
    public TextMeshProUGUI gachaExpText;

    [Header("가챠 시스템 연결")]
    public GachaSystem gachaSystem;
    public GachaResultPopupUI resultPopupUI; // <--- Inspector에서 연결

    void Start()
    {
        if (titleText) titleText.text = $"{panelType} 가챠";
        if (singleRollButton) singleRollButton.onClick.AddListener(RollOne);
        if (tenRollButton) tenRollButton.onClick.AddListener(RollTen);
        UpdatePanel();
    }

    void RollOne()
    {
        var results = gachaSystem.RollGachaAndGetResult(panelType, 1);
        if (results != null)
        {
            UpdatePanel();
            resultPopupUI.Show(results); // 결과 팝업 띄움
        }
    }

    void RollTen()
    {
        var results = gachaSystem.RollGachaAndGetResult(panelType, 10);
        if (results != null)
        {
            UpdatePanel();
            resultPopupUI.Show(results);
        }
    }

    void UpdatePanel()
    {
        if (dropRateText)
            dropRateText.text = gachaSystem.GetDropRateStringGrid(panelType);

        var info = gachaSystem.gachaLevels[panelType];
        int maxExp = gachaSystem.GetLevelUpExp(info.level);
        if (gachaExpSlider)
        {
            gachaExpSlider.maxValue = maxExp;
            gachaExpSlider.value = info.exp;
        }
        if (gachaExpText)
            gachaExpText.text = $"Lv.{info.level}  ({info.exp} / {maxExp})";
    }
}