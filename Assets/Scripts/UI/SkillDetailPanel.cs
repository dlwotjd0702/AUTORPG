using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Inventory
{
    public class SkillDetailPanel : MonoBehaviour
    {
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI descText;
        public TextMeshProUGUI cooltimeText;
        public Button enhanceBtn;
        public Button combineBtn;
        public Button[] equipButtons; // 4개

        private InventorySystem inventory;
        private string currentSkillId;
        public GameObject equipDetailPanel;

        private void OnEnable()
        {
            equipDetailPanel.SetActive(false);
        }

        public void Init(InventorySystem inv)
        {
            inventory = inv;
        }

        public void ShowDetail(string skillId)
        {
            if (inventory == null) inventory = FindObjectOfType<InventorySystem>();
            currentSkillId = skillId;

            var skillData = inventory.GetEquipmentData(skillId);
            if (skillData == null) { gameObject.SetActive(false); return; }

            gameObject.SetActive(true);
            iconImage.sprite = inventory.GetIcon(skillId);
            nameText.text = skillData.name;
            levelText.text = $"Lv.{skillData.grade}";
            descText.text = skillData.description;
            cooltimeText.text = $"쿨타임: {skillData.cooldown ?? 0:F1}초";
            enhanceBtn.onClick.RemoveAllListeners();
            combineBtn.onClick.RemoveAllListeners();
            enhanceBtn.onClick.AddListener(OnEnhance);
            combineBtn.onClick.AddListener(OnCombine);

            for (int i = 0; i < equipButtons.Length; i++)
            {
                int idx = i;
                equipButtons[i].onClick.RemoveAllListeners();
                equipButtons[i].onClick.AddListener(() => OnEquip(idx));
                bool equipped = (inventory.equippedSkillIds[idx] == skillId);
                equipButtons[i].GetComponent<Image>().color = equipped ? Color.yellow : Color.white;
            }
        }

        void OnEnhance()
        {
            inventory.EnhanceSkill(currentSkillId);
            ShowDetail(currentSkillId);
        }
        void OnCombine()
        {
            inventory.CombineSkill(currentSkillId);
            ShowDetail(currentSkillId);
        }
        void OnEquip(int slotIdx)
        {
            inventory.EquipSkill(slotIdx, currentSkillId);
            FindObjectOfType<SkillQuickSlotUI>()?.Refresh();
            ShowDetail(currentSkillId);
            // 필요시 인게임 HUD/슬롯 동기화
        }
    }
}
