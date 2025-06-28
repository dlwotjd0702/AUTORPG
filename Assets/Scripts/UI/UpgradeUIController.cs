using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Stats
{
    public class UpgradeUIController : MonoBehaviour
    {
        [Header("연동 매니저")]
        public UpgradeManager upgradeManager;

        

        [Header("공격력 강화")]
        public TextMeshProUGUI atkCostText;
        public Button atkUpgradeButton;

        [Header("방어력 강화")]
        public TextMeshProUGUI defCostText;
        public Button defUpgradeButton;

        [Header("체력 강화")]
        public TextMeshProUGUI hpCostText;
        public Button hpUpgradeButton;

        [Header("공격속도 강화")]
        public TextMeshProUGUI atkSpdCostText;
        public Button atkSpdUpgradeButton;

        [Header("크리확률 강화")]
        public TextMeshProUGUI critRateCostText;
        public Button critRateUpgradeButton;

        [Header("크리뎀 강화")]
        public TextMeshProUGUI critDmgCostText;
        public Button critDmgUpgradeButton;

        [Header("드랍률 강화")]
        public TextMeshProUGUI dropRateCostText;
        public Button dropRateUpgradeButton;

        void Start()
        {
            atkUpgradeButton.onClick.AddListener(OnAttackUpgradeClicked);
            defUpgradeButton.onClick.AddListener(OnDefenseUpgradeClicked);
            hpUpgradeButton.onClick.AddListener(OnHpUpgradeClicked);
            atkSpdUpgradeButton.onClick.AddListener(OnAtkSpdUpgradeClicked);
            critRateUpgradeButton.onClick.AddListener(OnCritRateUpgradeClicked);
            critDmgUpgradeButton.onClick.AddListener(OnCritDmgUpgradeClicked);
            dropRateUpgradeButton.onClick.AddListener(OnDropRateUpgradeClicked);
            upgradeManager.OnGoldChanged += RefreshUI;
            RefreshUI();
        }

        void RefreshUI()
        {
            
            atkCostText.text      = $"Attack {upgradeManager.atkUpgradeLevel}Level 비용: {upgradeManager.GetAttackUpgradeCost()}gold";
            defCostText.text      = $"Defense {upgradeManager.defUpgradeLevel}Level 비용: {upgradeManager.GetDefenseUpgradeCost()}gold";
            hpCostText.text       = $"Hp {upgradeManager.hpUpgradeLevel}Level 비용: {upgradeManager.GetHpUpgradeCost()}gold";
            atkSpdCostText.text   = $"AtkSpd {upgradeManager.atkSpdUpgradeLevel}Level 비용: {upgradeManager.GetAtkSpdUpgradeCost()}gold";
            critRateCostText.text = $"CritRate {upgradeManager.critRateUpgradeLevel}Level 비용: {upgradeManager.GetCritRateUpgradeCost()}gold";
            critDmgCostText.text  = $"CritDmg {upgradeManager.critDmgUpgradeLevel}Level 비용: {upgradeManager.GetCritDmgUpgradeCost()}gold";
            dropRateCostText.text     = $"드랍률: {upgradeManager.GetCurrentDropRate() * 100f:F1} 비용: {upgradeManager.GetDropRateUpgradeCost()}gold";
      
        }

        void OnAttackUpgradeClicked()
        {
        
            if (upgradeManager.UpgradeAttack())
                RefreshUI();
            Debug.Log("11");
        }
        void OnDefenseUpgradeClicked()
        {
            if (upgradeManager.UpgradeDefense())
                RefreshUI();
        }
        void OnHpUpgradeClicked()
        {
            if (upgradeManager.UpgradeHp())
                RefreshUI();
        }
        void OnAtkSpdUpgradeClicked()
        {
            if (upgradeManager.UpgradeAtkSpeed())
                RefreshUI();
        }
        void OnCritRateUpgradeClicked()
        {
            if (upgradeManager.UpgradeCritRate())
                RefreshUI();
        }
        void OnCritDmgUpgradeClicked()
        {
            if (upgradeManager.UpgradeCritDmg())
                RefreshUI();
        }
        void OnDropRateUpgradeClicked()
        {
            if (upgradeManager.UpgradeDropRate())
                RefreshUI();
        }

        
        public void OnCloseButton()
        {
            gameObject.SetActive(false);
        }
        public void OnOpenButton()
        {
            gameObject.SetActive(true);
        }
    }
}
