using System;
using Combat;
using TMPro;
using UnityEngine;

namespace Stats
{
    public class UpgradeManager : MonoBehaviour, ISaveable
    {
        [Header("골드 및 연동")]
        public int gold = 0;
        public PlayerStats playerStats;
        public TextMeshProUGUI goldText;
        [Header("강화 단계 (각 스탯별)")]
        [SerializeField] public int atkUpgradeLevel = 0;
        [SerializeField] public int defUpgradeLevel = 0;
        [SerializeField] public int hpUpgradeLevel = 0;
        [SerializeField] public int atkSpdUpgradeLevel = 0;
        [SerializeField] public int critRateUpgradeLevel = 0;
        [SerializeField] public int critDmgUpgradeLevel = 0;

        [Header("강화 비용 및 증가율")]
        [SerializeField] private int attackUpgradeCost = 100;
        [SerializeField] private int defenseUpgradeCost = 80;
        [SerializeField] private int hpUpgradeCost = 120;
        [SerializeField] private int atkSpdUpgradeCost = 200;
        [SerializeField] private int critRateUpgradeCost = 300;
        [SerializeField] private int critDmgUpgradeCost = 300;
        [SerializeField] private float costIncreaseRate = 0.1f; // 10%씩 증가

        [Header("드랍률 업그레이드")]
        [SerializeField] public float baseDropRate = 0.2f;
        [SerializeField] public float penaltyPerGrade = 0.05f;
        [SerializeField] public int dropRateUpgradeLevel = 0;
        public event Action OnGoldChanged = delegate { };

        // 비용 계산
        int CalcUpgradeCost(int baseCost, int level)
        {
            return Mathf.RoundToInt(baseCost * Mathf.Pow(1f + costIncreaseRate, level));
        }
        public int GetAttackUpgradeCost() => CalcUpgradeCost(attackUpgradeCost, atkUpgradeLevel);
        public int GetDefenseUpgradeCost() => CalcUpgradeCost(defenseUpgradeCost, defUpgradeLevel);
        public int GetHpUpgradeCost() => CalcUpgradeCost(hpUpgradeCost, hpUpgradeLevel);
        public int GetAtkSpdUpgradeCost() => CalcUpgradeCost(atkSpdUpgradeCost, atkSpdUpgradeLevel);
        public int GetCritRateUpgradeCost() => CalcUpgradeCost(critRateUpgradeCost, critRateUpgradeLevel);
        public int GetCritDmgUpgradeCost() => CalcUpgradeCost(critDmgUpgradeCost, critDmgUpgradeLevel);

        void Start()
        {
            var save = SaveManager.pendingSaveData;
            if (save != null)
                ApplyLoadedData(save);
            
            OnGoldChanged += UpdateGoldText;
            UpdateGoldText();
        }
        void UpdateGoldText()
        {
            if (goldText != null)
                goldText.text = $"Gold: {gold:N0}";
        }


        public int GetDropRateUpgradeCost()
        {
            return 1000 * (dropRateUpgradeLevel + 1);
        }

        public float GetCurrentDropRate()
        {
            return baseDropRate + (dropRateUpgradeLevel * 0.02f);
        }
        public float GetDropChanceForGrade(int grade)
        {
            float result = GetCurrentDropRate() - penaltyPerGrade * (grade - 1);
            return Mathf.Max(0f, result);
        }

        public bool UpgradeDropRate()
        {
            int cost = GetDropRateUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            dropRateUpgradeLevel++;
            OnGoldChanged.Invoke();
            Debug.Log($"[Upgrade] 드랍률 업! (현재 단계: {dropRateUpgradeLevel}, 비용: {cost}, 현재 전체 드랍률: {GetCurrentDropRate() * 100f:F1}%)");
            return true;
        }

        // ------- 실제 강화 메서드들 ---------
        public bool UpgradeAttack(int amount = 1)
        {
            int cost = GetAttackUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            atkUpgradeLevel++;
            OnGoldChanged.Invoke();
            playerStats.UpgradeAttack(amount);
            return true;
        }
        public bool UpgradeDefense(int amount = 1)
        {
            int cost = GetDefenseUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            defUpgradeLevel++;
            OnGoldChanged.Invoke();
            playerStats.UpgradeDefense(amount);
            return true;
        }
        public bool UpgradeHp(int amount = 10)
        {
            int cost = GetHpUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            hpUpgradeLevel++;
            OnGoldChanged.Invoke();
            playerStats.UpgradeHp(amount);
            return true;
        }
        public bool UpgradeAtkSpeed(float amount = 0.05f)
        {
            int cost = GetAtkSpdUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            atkSpdUpgradeLevel++;
            OnGoldChanged.Invoke();
            playerStats.UpgradeAtkSpeed(amount);
            return true;
        }
        public bool UpgradeCritRate(float amount = 0.01f)
        {
            int cost = GetCritRateUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            critRateUpgradeLevel++;
            OnGoldChanged.Invoke();
            playerStats.UpgradeCritRate(amount);
            return true;
        }
        public bool UpgradeCritDmg(float amount = 0.05f)
        {
            int cost = GetCritDmgUpgradeCost();
            if (gold < cost) return false;
            gold -= cost;
            critDmgUpgradeLevel++;
            OnGoldChanged.Invoke();
            playerStats.UpgradeCritDmg(amount);
            return true;
        }

        public void AddGold(int amount)
        {
            int finalAmount = amount;
            if (AdBuffManager.Instance != null && AdBuffManager.Instance.GoldBuffActive)
                finalAmount *= 2;
            gold += finalAmount;
            OnGoldChanged.Invoke();
        }

        public void ApplyLoadedData(SaveData data)
        {
            if (data == null) return;
            gold = data.gold;
            atkUpgradeLevel = data.atkUpgradeLevel;
            defUpgradeLevel = data.defUpgradeLevel;
            hpUpgradeLevel = data.hpUpgradeLevel;
            atkSpdUpgradeLevel = data.atkSpdUpgradeLevel;
            critRateUpgradeLevel = data.critRateUpgradeLevel;
            critDmgUpgradeLevel = data.critDmgUpgradeLevel;
            dropRateUpgradeLevel = data.dropRateUpgradeLevel;

            // 스탯 전체 초기화 후 강화 레벨만큼 적용 (PlayerStats에 ResetAllUpgrades 메서드 필요)
            if (playerStats != null)
            {
                playerStats.ResetAllUpgrades();
                for (int i = 0; i < atkUpgradeLevel; i++) playerStats.UpgradeAttack();
                for (int i = 0; i < defUpgradeLevel; i++) playerStats.UpgradeDefense();
                for (int i = 0; i < hpUpgradeLevel; i++) playerStats.UpgradeHp();
                for (int i = 0; i < atkSpdUpgradeLevel; i++) playerStats.UpgradeAtkSpeed();
                for (int i = 0; i < critRateUpgradeLevel; i++) playerStats.UpgradeCritRate();
                for (int i = 0; i < critDmgUpgradeLevel; i++) playerStats.UpgradeCritDmg();
            }
        }

        public void CollectSaveData(SaveData data)
        {
            data.gold = gold;
            data.atkUpgradeLevel = atkUpgradeLevel;
            data.defUpgradeLevel = defUpgradeLevel;
            data.hpUpgradeLevel = hpUpgradeLevel;
            data.atkSpdUpgradeLevel = atkSpdUpgradeLevel;
            data.critRateUpgradeLevel = critRateUpgradeLevel;
            data.critDmgUpgradeLevel = critDmgUpgradeLevel;
            data.dropRateUpgradeLevel = dropRateUpgradeLevel;
        }
    }
}
