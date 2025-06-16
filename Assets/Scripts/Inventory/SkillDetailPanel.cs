using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Inventory
{
    public class SkillDetailPanel : MonoBehaviour
    {
        public Image iconImage;
        public TMP_Text nameText;
        public TMP_Text levelText;
        public TMP_Text descText;
        public TMP_Text costText;
        public Button enhanceBtn;
        public Button combineBtn;
        public Button[] equipButtons; // 4개(Inspector에 연결)

        private InventorySystem inventory;
        private string currentSkillId;

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
            costText.text = $"코스트: {skillData.cooldown ?? 0:F1}초";
            enhanceBtn.onClick.RemoveAllListeners();
            combineBtn.onClick.RemoveAllListeners();
            enhanceBtn.onClick.AddListener(OnEnhance);
            combineBtn.onClick.AddListener(OnCombine);

            for (int i = 0; i < equipButtons.Length; i++)
            {
                int idx = i;
                equipButtons[i].onClick.RemoveAllListeners();
                equipButtons[i].onClick.AddListener(() => OnEquip(idx));
            }
        }

        void OnEnhance()
        {
            // 강화 로직 구현 (강화시 skillData.grade++)
            inventory.EnhanceSkill(currentSkillId);
            ShowDetail(currentSkillId);
        }

        void OnCombine()
        {
            // 합성 로직 구현
            inventory.CombineSkill(currentSkillId);
            ShowDetail(currentSkillId);
        }

        void OnEquip(int slotIdx)
        {
            inventory.EquipSkill(slotIdx, currentSkillId);
            // 필요시: 인게임 HUD/슬롯 즉시 갱신 (이벤트, 델리게이트, 직접 호출 등)
            Debug.Log($"{currentSkillId}가 {slotIdx+1}번 슬롯에 장착됨");
        }
    }
}
