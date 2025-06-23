using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemDetailPanel : MonoBehaviour
    {
        public Image iconImage;
        public TMP_Text nameText;
        public TMP_Text countText;
        public TMP_Text levelText;
        public TMP_Text awakenText;
        public TMP_Text equippedText;
        public TMP_Text ownedEffectText;
        public TMP_Text equippedEffectText;
        public TMP_Text descriptionText;

        public Button enhanceBtn;
        public Button combineBtn;
        public Button awakenBtn;

        private InventorySystem inventory;
        private Item currentItem;
        private string currentId;
        public GameObject skillDetailPanel;

        private void OnEnable()
        {
            skillDetailPanel.SetActive(false);
        }

        public void Init(InventorySystem system)
        {
            inventory = system;
        }

        public void ShowDetail(string id)
        {
            if (inventory == null)
                inventory = FindObjectOfType<InventorySystem>();

            currentId = id;
            currentItem = inventory.GetSlotById(id);
            if (currentItem == null || !currentItem.isOwned)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);

            // 필드 값 갱신
            iconImage.sprite = inventory.GetIcon(currentItem.itemData.id);
            nameText.text = currentItem.itemData.name;
            countText.text = $"x{currentItem.count}/2";
            levelText.text = $"강화: +{currentItem.level}";
            awakenText.text = $"각성: {currentItem.awakenLevel}";
            equippedText.text = currentItem.isEquipped ? "착용중" : "";
            ownedEffectText.text = $"보유효과: {currentItem.GetOwnedEffectText()}";
            equippedEffectText.text = $"착용효과: {currentItem.GetEquippedEffectText()}";
            descriptionText.text = currentItem.itemData.description;

            enhanceBtn.interactable = true;
            combineBtn.interactable = currentItem.count >= 2;
            awakenBtn.interactable = true;

            enhanceBtn.onClick.RemoveAllListeners();
            combineBtn.onClick.RemoveAllListeners();
            awakenBtn.onClick.RemoveAllListeners();

            enhanceBtn.onClick.AddListener(OnEnhance);
            combineBtn.onClick.AddListener(OnCombine);
            awakenBtn.onClick.AddListener(OnAwaken);
        }

        void OnEnhance()
        {
            if (currentItem != null)
            {
                currentItem.level++;
                ShowDetail(currentId);
            }
        }
        void OnCombine()
        {
            if (currentItem != null && currentItem.count >= 2)
            {
                inventory.TryCombine(currentId, 2);
                ShowDetail(currentId);
            }
        }
        void OnAwaken()
        {
            if (currentItem != null)
            {
                currentItem.awakenLevel++;
                ShowDetail(currentId);
            }
        }
    }
}
