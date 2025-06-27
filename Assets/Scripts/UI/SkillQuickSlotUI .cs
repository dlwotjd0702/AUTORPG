using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillQuickSlotUI : MonoBehaviour
{
    public Button[] slotButtons;                // 퀵슬롯 버튼 (4개)
    public Image[] slotImages;                  // 스킬 아이콘 (4개)
    public Image[] cooldownOverlays;            // 쿨타임 오버레이 (Image, Filled로 설정)
    public TextMeshProUGUI[] cooldownTexts;     // 쿨타임 남은 시간 (4개)

    public Button autoSkillButton;              // 전체 오토스킬 토글 버튼
    public Image autoSkillButtonIcon;           // 오토스킬 버튼 아이콘
    public Sprite autoOnIcon, autoOffIcon;      // 아이콘 스프라이트

    public Inventory.InventorySystem inventory;
    private SkillManager skillManager;

    [HideInInspector]
    public bool autoSkillEnabled = false;

    void Start()
    {
        skillManager = FindObjectOfType<SkillManager>();

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSkillSlotClicked(idx));
        }

        if (autoSkillButton)
            autoSkillButton.onClick.AddListener(ToggleAutoSkill);

        UpdateAutoSkillButtonIcon();
        Refresh();
    }

    void Update()
    {
        // 쿨타임 오버레이/텍스트 실시간 표시
        for (int i = 0; i < slotImages.Length; i++)
        {
            string skillId = inventory.equippedSkillIds[i];
            if (!string.IsNullOrEmpty(skillId) && skillManager.skillDict.TryGetValue(skillId, out var skill))
            {
                float cd = skill.CooldownTimer;
                float maxCd = skill.Data.Cooldown;
                bool isCooling = cd > 0.05f;

                cooldownOverlays[i].gameObject.SetActive(isCooling);
                cooldownTexts[i].gameObject.SetActive(isCooling);

                if (isCooling)
                {
                    cooldownOverlays[i].fillAmount = Mathf.Clamp01(cd / maxCd);
                    cooldownTexts[i].text = cd.ToString("0.0");
                }
            }
            else
            {
                cooldownOverlays[i].gameObject.SetActive(false);
                cooldownTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            string skillId = inventory.equippedSkillIds[i];
            if (!string.IsNullOrEmpty(skillId))
            {
                slotImages[i].sprite = inventory.GetIcon(skillId);
                slotImages[i].gameObject.SetActive(true);
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].gameObject.SetActive(false);
            }
        }
    }

    void OnSkillSlotClicked(int idx)
    {
        string skillId = inventory.equippedSkillIds[idx];
        if (!string.IsNullOrEmpty(skillId) && skillManager.skillDict.TryGetValue(skillId, out var skill))
        {
            if (skill.IsReady())
                skillManager.UseSkillById(skillId);
            else
                Debug.Log("쿨타임 중!");
        }
        else
        {
            Debug.LogWarning($"퀵슬롯 {idx}에 장착된 스킬이 없습니다.");
        }
    }

    void ToggleAutoSkill()
    {
        autoSkillEnabled = !autoSkillEnabled;
        UpdateAutoSkillButtonIcon();
    }

    void UpdateAutoSkillButtonIcon()
    {
        if (autoSkillButtonIcon)
        {
            autoSkillButtonIcon.sprite = autoSkillEnabled ? autoOnIcon : autoOffIcon;
            autoSkillButtonIcon.color = autoSkillEnabled ? Color.yellow : Color.gray;
        }
    }
}
