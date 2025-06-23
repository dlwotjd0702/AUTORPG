using UnityEngine;
public enum ItemType
{
    weapon,
    armor,
    ring,
    skill
}
public enum SkillType
{
    Active,
    Passive
}
    

    [System.Serializable]
    public class EquipmentData
    {
        public string id { get; set; }
        public ItemType type { get; set; }
        public string name { get; set; }
        public int grade { get; set; }

        public float? ownedAtkPercent { get; set; }
        public float? equipAtkPercent { get; set; }
        public float? ownedAtkSpdPercent { get; set; }
        public float? equipAtkSpdPercent { get; set; }
        public float? ownedDefPercent { get; set; }
        public float? equipDefPercent { get; set; }
        public float? ownedHpPercent { get; set; }
        public float? equipHpPercent { get; set; }
        public float? ownedCritRatePercent { get; set; }
        public float? equipCritRatePercent { get; set; }
        public float? ownedCritDmgPercent { get; set; }
        public float? equipCritDmgPercent { get; set; }
        public SkillType? skillType { get; set; }
        public float? skillOwnedValue { get; set; }
        public float? skillEquipValue { get; set; }
        public float? cooldown { get; set; }
        public int? skillPower { get; set; }
        public int? maxLevel { get; set; }
        public string description { get; set; }

        public EquipmentData Clone()
        {
            return (EquipmentData)this.MemberwiseClone();
        }

        // 사용 시, 안전하게 ?? 0f로 접근
        public float OwnedAtkPercent => ownedAtkPercent ?? 0f;
        public float EquipAtkPercent => equipAtkPercent ?? 0f;
        public float OwnedAtkSpdPercent => ownedAtkSpdPercent ?? 0f;
        public float EquipAtkSpdPercent => equipAtkSpdPercent ?? 0f;

        public float OwnedDefPercent => ownedDefPercent ?? 0f;
        public float EquipDefPercent => equipDefPercent ?? 0f;
        public float OwnedHpPercent => ownedHpPercent ?? 0f;
        public float EquipHpPercent => equipHpPercent ?? 0f;

        public float OwnedCritRatePercent => ownedCritRatePercent ?? 0f;
        public float EquipCritRatePercent => equipCritRatePercent ?? 0f;
        public float OwnedCritDmgPercent => ownedCritDmgPercent ?? 0f;
        public float EquipCritDmgPercent => equipCritDmgPercent ?? 0f;

        public float SkillOwnedValue => skillOwnedValue ?? 0f;
        public float SkillEquipValue => skillEquipValue ?? 0f;
        public float Cooldown => cooldown ?? 0f;
        public int SkillPower => skillPower ?? 0;
        public int MaxLevel => maxLevel ?? 0;
    }


    
    
    
