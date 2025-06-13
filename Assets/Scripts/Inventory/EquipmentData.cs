using UnityEngine;

namespace Inventory
{
    public enum ItemType { Weapon, Armor, Accessory, Skill, Consumable }
    public enum SkillType { Active, Passive }

    [System.Serializable]
    public class EquipmentData
    {
        public string id;
        public string name;
        public ItemType type;
        public int grade;

        // 무기(공격력, 공격속도)
        public float ownedAtkPercent, equipAtkPercent;      // 곱연산 %
        public float ownedAtkSpdPercent, equipAtkSpdPercent;

        // 방어구(방어력, 체력)
        public float ownedDefPercent, equipDefPercent;
        public float ownedHpPercent, equipHpPercent;

        // 악세(크리확률, 크리뎀)
        public float ownedCritRatePercent, equipCritRatePercent;
        public float ownedCritDmgPercent, equipCritDmgPercent;

        // --- 스킬 관련 생략 ---
        public SkillType skillType;
        public float skillOwnedValue;
        public float skillEquipValue;
        public float cooldown;
        public int skillPower;

        public int maxLevel;
        public string description;

        public EquipmentData Clone()
        {
            return (EquipmentData)this.MemberwiseClone();
        }
    }
    
    
}