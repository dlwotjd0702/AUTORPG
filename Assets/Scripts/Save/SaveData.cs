using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // 강화/레벨/드랍
    public int gold;
    public int level;
    public int exp;
    public int expToLevelUp;

    public int atkUpgradeLevel;
    public int defUpgradeLevel;
    public int hpUpgradeLevel;
    public int atkSpdUpgradeLevel;
    public int critRateUpgradeLevel;
    public int critDmgUpgradeLevel;

    public float baseDropRate;
    public float penaltyPerGrade;
    public int dropRateUpgradeLevel;

    // 인벤토리(슬롯단위)
    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();
}

[System.Serializable]
public class InventorySlotData
{
    public string id;
    public int count;
    public int level;
    public int awakenLevel;
    public bool isOwned;
    public bool isEquipped;
}