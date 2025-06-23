using System;
using Combat;
using Inventory;
using UnityEngine;

namespace Stats
{
    public class PlayerStats : MonoBehaviour, ISaveable
    {
        [Header("연동 시스템")]
        public InventorySystem inventory;

        // 경험치/레벨
        [Header("레벨/경험치")]
        public int level = 1;
        public int exp = 0;
        public int expToLevelUp = 100;

        // Base 스탯
        [Header("Base Stat(최초/강화/레벨업 모두 반영)")]
        private int baseAttack = 5;
        private float baseAtkSpeed = 1.0f;
        private int baseDefense = 0;
        private int baseHp = 1000;
        private float baseCritRate = 0.05f;
        private float baseCritDmg = 1.5f;

        // 캐싱(자동 갱신)
        public float FinalAttack { get; private set; }
        public float FinalAtkSpeed;
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
            var (atkMul, atkSpdMul) = inventory.GetWeaponMultipliers();
            FinalAttack = Mathf.FloorToInt(baseAttack * atkMul);
            FinalAtkSpeed = baseAtkSpeed * atkSpdMul;

            var (defMul, hpMul) = inventory.GetArmorMultipliers();
            FinalDefense = Mathf.FloorToInt(baseDefense * defMul);
            FinalHp = Mathf.FloorToInt(baseHp * hpMul);

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
        }
        public void CollectSaveData(SaveData data)
        {
            data.level = level;
            data.exp = exp;
            data.expToLevelUp = expToLevelUp;
        }

        // ----------- 이벤트들 -------------
        public event Action OnStatsChanged = delegate { };
        public event Action OnExpChanged = delegate { };
        public event Action<int> OnLevelUp = delegate { };

        // ----------- [스킬 효과 직접 적용 부분] -------------
        public void AddPassiveSkillEffect(EquipmentData skill)
        {
            if (skill == null) return;
            if (skill.skillOwnedValue != null)
            {
                UpgradeAttack(Mathf.RoundToInt((float)skill.skillOwnedValue));
                UpgradeHp(Mathf.RoundToInt((float)skill.skillOwnedValue));
            }

            // 원하는 효과 추가 가능
        }

        public void ActivateSkill(EquipmentData skill)
        {
            if (skill == null) return;
            // 실전에서 원하는 액티브 효과를 여기서 처리
            // 예시: 체력 회복, 공격력 버프 등
            Debug.Log($"액티브 스킬 [{skill.name}] 발동! 효과치: {skill.skillPower}");
            // 필요시 직접 스탯에 적용:
            // UpgradeHp(Mathf.RoundToInt(skill.skillPower));
        }

        public void ResetAllUpgrades()
        {
            baseAttack = 5;
            baseAtkSpeed = 1.0f;
            baseDefense = 0;
            baseHp = 1000;
            baseCritRate = 0.05f;
            baseCritDmg = 1.5f;
        }
    }
}
