
    [System.Serializable]
    public class Item // = InventorySlot
    {
        public EquipmentData itemData;
        public int count;
        public int level;
        public int awakenLevel;
        public bool isOwned=false;
        public bool isEquipped;

        public bool IsEmpty => itemData == null;

        public void Clear()
        {
            itemData = null;
            count = 0;
            level = 0;
            awakenLevel = 0;
            isEquipped = false;
        }

        // [강화/각성 배율 하드코딩, 필요시 따로 설정 가능]
        public const float ENHANCE_RATIO = 0.1f;   // 강화 1렙당 +10%
        public const float AWAKEN_RATIO = 0.2f;    // 각성 1렙당 +20%

        // ================ [보유/장착 효과 계산] ====================

        public float GetOwnedEffect(float baseValue)
        {
            // (1 + level * ENHANCE_RATIO + awakenLevel * AWAKEN_RATIO)
            return baseValue * (1f + level * ENHANCE_RATIO + awakenLevel * AWAKEN_RATIO);
        }
        public float GetEquippedEffect(float baseValue)
        {
            // 장착 효과도 동일하게 강화
            return baseValue * (1f + level * ENHANCE_RATIO + awakenLevel * AWAKEN_RATIO);
        }

        // 각 효과별 getter
        public float OwnedAtkPercent => GetOwnedEffect(itemData.OwnedAtkPercent);
        public float OwnedAtkSpdPercent => GetOwnedEffect(itemData.OwnedAtkSpdPercent);
        public float OwnedDefPercent => GetOwnedEffect(itemData.OwnedDefPercent);
        public float OwnedHpPercent => GetOwnedEffect(itemData.OwnedHpPercent);
        public float OwnedCritRatePercent => GetOwnedEffect(itemData.OwnedCritRatePercent);
        public float OwnedCritDmgPercent => GetOwnedEffect(itemData.OwnedCritDmgPercent);

        public float EquipAtkPercent => GetEquippedEffect(itemData.EquipAtkPercent);
        public float EquipAtkSpdPercent => GetEquippedEffect(itemData.EquipAtkSpdPercent);
        public float EquipDefPercent => GetEquippedEffect(itemData.EquipDefPercent);
        public float EquipHpPercent => GetEquippedEffect(itemData.EquipHpPercent);
        public float EquipCritRatePercent => GetEquippedEffect(itemData.EquipCritRatePercent);
        public float EquipCritDmgPercent => GetEquippedEffect(itemData.EquipCritDmgPercent);

        public float OwnedSkillValue => GetOwnedEffect(itemData.SkillOwnedValue);
        public float EquipSkillValue => GetEquippedEffect(itemData.SkillEquipValue);

        // 보유/장착 효과 텍스트 (상세패널 표시용)
        public string GetOwnedEffectText()
        {
            // 원하는 효과만 표기 (예시: 공격력, 방어력, 체력 등)
            string s = "";
            if (itemData.OwnedAtkPercent > 0) s += $"공격력 +{OwnedAtkPercent * 100f:F1}% ";
            if (itemData.OwnedAtkSpdPercent > 0) s += $"공속 +{OwnedAtkPercent * 100f:F1}% ";
            if (itemData.OwnedDefPercent > 0) s += $"방어력 +{OwnedDefPercent * 100f:F1}% ";
            if (itemData.OwnedHpPercent > 0) s += $"체력 +{OwnedHpPercent * 100f:F1}% ";
            if (itemData.OwnedCritRatePercent > 0) s += $"치명타확률 +{OwnedCritRatePercent * 100f:F1}% ";
            if (itemData.OwnedCritDmgPercent > 0) s += $"치명타피해 +{OwnedCritDmgPercent * 100f:F1}% ";
            if (itemData.SkillOwnedValue > 0) s += $"스킬효과 +{OwnedSkillValue:F1} ";
            return s.Trim();
        }
        public string GetEquippedEffectText()
        {
            string s = "";
            if (itemData.EquipAtkPercent > 0) s += $"공격력 +{EquipAtkPercent * 100f:F1}% ";
            if (itemData.EquipAtkSpdPercent > 0) s += $"공속 +{EquipAtkPercent * 100f:F1}% ";
            if (itemData.EquipDefPercent > 0) s += $"방어력 +{EquipDefPercent * 100f:F1}% ";
            if (itemData.EquipHpPercent > 0) s += $"체력 +{EquipHpPercent * 100f:F1}% ";
            if (itemData.EquipCritRatePercent > 0) s += $"치명타확률 +{EquipCritRatePercent * 100f:F1}% ";
            if (itemData.EquipCritDmgPercent > 0) s += $"치명타피해 +{EquipCritDmgPercent * 100f:F1}% ";
            if (itemData.SkillEquipValue > 0) s += $"스킬효과 +{EquipSkillValue:F1} ";
            return s.Trim();
        }
    }

