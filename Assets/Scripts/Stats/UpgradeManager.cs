using System;
using UnityEngine;

namespace Stats
{
    public class UpgradeManager : MonoBehaviour
    {
        [Header("골드 및 연동")]
        public int gold = 0;
        public PlayerStats playerStats;
     

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
        [SerializeField] public float baseDropRate = 0.2f;    // 20% 기본
        [SerializeField] public float penaltyPerGrade = 0.05f; // 등급당 -5%
        [SerializeField] private int dropRateUpgradeLevel = 0; // 전체 드랍률 업
        public event Action OnGoldChanged = delegate { };

        // 비용 계산
        int CalcUpgradeCost(int baseCost, int level)
        {
            return Mathf.RoundToInt(baseCost * Mathf.Pow(1f + costIncreaseRate, level));
        }
        public int GetAttackUpgradeCost()    => CalcUpgradeCost(attackUpgradeCost, atkUpgradeLevel);
        public int GetDefenseUpgradeCost()   => CalcUpgradeCost(defenseUpgradeCost, defUpgradeLevel);
        public int GetHpUpgradeCost()        => CalcUpgradeCost(hpUpgradeCost, hpUpgradeLevel);
        public int GetAtkSpdUpgradeCost()    => CalcUpgradeCost(atkSpdUpgradeCost, atkSpdUpgradeLevel);
        public int GetCritRateUpgradeCost()  => CalcUpgradeCost(critRateUpgradeCost, critRateUpgradeLevel);
        public int GetCritDmgUpgradeCost()   => CalcUpgradeCost(critDmgUpgradeCost, critDmgUpgradeLevel);

   

        public int GetDropRateUpgradeCost()
        {
            // 드랍률 업그레이드 비용 (예시 공식)
            return 1000 * (dropRateUpgradeLevel + 1);
        }

        // 드랍률 계산
        public float GetCurrentDropRate()
        {
            // 업그레이드마다 +2% 추가 (예시)
            return baseDropRate + (dropRateUpgradeLevel * 0.02f);
        }
        public float GetDropChanceForGrade(int grade)
        {
            // 등급별 드랍확률(음수면 0, 등급 1~20)
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

        // 골드 추가 (외부 보상 등에서 호출)
        public void AddGold(int amount)
        {
            gold += amount;
            OnGoldChanged.Invoke();
            Debug.Log($"[UpgradeManager] 골드 획득: {amount} (현재 GOLD: {gold})");

        }
    }
}
