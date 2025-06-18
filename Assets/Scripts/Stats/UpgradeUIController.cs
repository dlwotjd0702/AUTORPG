using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Stats
{
    public class UpgradeUIController : MonoBehaviour
    {
        [Header("연동 매니저")]
        public UpgradeManager upgradeManager;

        [Header("골드 표시")]
        public TextMeshProUGUI goldText;

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
            goldText.text         = $"GOLD : {upgradeManager.gold}";
            atkCostText.text      = $"{upgradeManager.atkUpgradeLevel}Level 비용: {upgradeManager.GetAttackUpgradeCost()}";
            defCostText.text      = $"{upgradeManager.defUpgradeLevel}Level 비용: {upgradeManager.GetDefenseUpgradeCost()}";
            hpCostText.text       = $"{upgradeManager.hpUpgradeLevel}Level 비용: {upgradeManager.GetHpUpgradeCost()}";
            atkSpdCostText.text   = $"{upgradeManager.atkSpdUpgradeLevel}Level 비용: {upgradeManager.GetAtkSpdUpgradeCost()}";
            critRateCostText.text = $"{upgradeManager.critRateUpgradeLevel}Level 비용: {upgradeManager.GetCritRateUpgradeCost()}";
            critDmgCostText.text  = $"{upgradeManager.critDmgUpgradeLevel}Level 비용: {upgradeManager.GetCritDmgUpgradeCost()}";
            dropRateCostText.text     = $"드랍률: {upgradeManager.GetCurrentDropRate() * 100f:F1} 비용: {upgradeManager.GetDropRateUpgradeCost()}%";
      
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

        // 골드가 외부에서 바뀌었을 때도 RefreshUI를 호출해야 한다면,
        // UpgradeManager에서 골드가 바뀌면 이벤트를 발행하고, 여기서 구독해서 RefreshUI()를 실행해주면 된다.
    }
}
