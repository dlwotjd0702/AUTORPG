using System;
using Combat;
using Inventory;
using UnityEngine;

namespace Stats
{
    public class PlayerStats : MonoBehaviour,ISaveable
    {
        [Header("연동 시스템")]
        public InventorySystem inventory;

        // 경험치/레벨
        [Header("레벨/경험치")]
        public int level = 1;
        public int exp = 0;
        public int expToLevelUp = 100;

        // Base 스탯 (초기값은 인스펙터에서 할당 or 직접 지정)
        [Header("Base Stat(최초/강화/레벨업 모두 반영)")]
        private int baseAttack = 5;
        private float baseAtkSpeed = 1.0f;
        private int baseDefense = 0;
        private int baseHp = 1000;
        private float baseCritRate = 0.05f;   // 5%
        private float baseCritDmg = 1.5f;   

        // 캐싱(자동 갱신)
        public float FinalAttack { get; private set; }
        public float FinalAtkSpeed { get; private set; }
        public float FinalDefense { get; private set; }
        public float FinalHp { get; private set; }
        public float FinalCritRate { get; private set; }
        public float FinalCritDmg { get; private set; }

        private void Awake()
        {
            if (!inventory) inventory = GetComponent<InventorySystem>();
        }

        private void Start()
        {
            var save = SaveManager.pendingSaveData;
            if (save != null)
                ApplyLoadedData(save);
            inventory.OnInventoryChanged += RefreshStats;
        }

        public void AddExp(int amount)
        {
            exp += amount;
            while (exp >= expToLevelUp)
            {
                exp -= expToLevelUp;
                LevelUp();
            }
            OnExpChanged?.Invoke();
        }

        void LevelUp()
        {
            level++;
            expToLevelUp += 50;
            baseHp += 5;
            baseAttack += 1;
            baseDefense += 1;
            RefreshStats();
            OnLevelUp?.Invoke(level);
        }

        public void UpgradeAttack(int amount = 1)
        {
            baseAttack += amount;
            RefreshStats();
        }
        public void UpgradeDefense(int amount = 1)
        {
            baseDefense += amount;
            RefreshStats();
        }
        public void UpgradeHp(int amount = 10)
        {
            baseHp += amount;
            RefreshStats();
        }
        public void UpgradeAtkSpeed(float amount = 0.05f)
        {
            baseAtkSpeed += amount;
            RefreshStats();
        }
        public void UpgradeCritRate(float amount = 0.01f)
        {
            baseCritRate += amount;
            RefreshStats();
        }
        public void UpgradeCritDmg(float amount = 0.05f)
        {
            baseCritDmg += amount;
            RefreshStats();
        }

        public void RefreshStats()
        {
            // 무기: 공격력, 공격속도
            var (atkMul, atkSpdMul) = inventory.GetWeaponMultipliers();
            FinalAttack = Mathf.FloorToInt(baseAttack * atkMul);
            FinalAtkSpeed = baseAtkSpeed * atkSpdMul;

            // 방어구: 방어력, 체력
            var (defMul, hpMul) = inventory.GetArmorMultipliers();
            FinalDefense = Mathf.FloorToInt(baseDefense * defMul);
            FinalHp = Mathf.FloorToInt(baseHp * hpMul);

            // 악세: 크리확률, 크리뎀
            var (critRateMul, critDmgMul) = inventory.GetAccessoryMultipliers();
            FinalCritRate = baseCritRate * critRateMul;
            FinalCritDmg = baseCritDmg * critDmgMul;

            OnStatsChanged?.Invoke();
        }
        public void ApplyLoadedData(SaveData data)
        {
            if (data == null) return;
            level = data.level;
            exp = data.exp;
            expToLevelUp = data.expToLevelUp;
            // baseAttack, baseHp, baseDefense 등 필요시 data에서 추가
        }
        public void CollectSaveData(SaveData data)
        {
            data.level = level;
            data.exp = exp;
            data.expToLevelUp = expToLevelUp;
            // baseAttack, baseHp, baseDefense 등 필요시 data에 추가
        }

        // ----------- 이벤트들 -------------
        public event Action OnStatsChanged = delegate { };
        public event Action OnExpChanged = delegate { };
        public event Action<int> OnLevelUp = delegate { };
    }
}
