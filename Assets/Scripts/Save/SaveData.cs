using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // Player, Upgrade 등
    public int level;
    public int exp;
    public int expToLevelUp;

    public int gold;
    public int atkUpgradeLevel;
    public int defUpgradeLevel;
    public int hpUpgradeLevel;
    public int atkSpdUpgradeLevel;
    public int critRateUpgradeLevel;
    public int critDmgUpgradeLevel;
    public int dropRateUpgradeLevel;

    // 웨이브/스테이지 정보
    public int currentStage;
    public int currentWave;
    public int maxClearedStage;
    public int maxClearedWave;
    public List<StageWaveRecord> clearedStageWave = new List<StageWaveRecord>();
    public int progressMode; // 0: Repeat, 1: Advance

    // 인벤토리
    public List<InventorySlotSave> inventorySlots = new List<InventorySlotSave>();
}

[Serializable]
public class StageWaveRecord
{
    public int stage;
    public int wave;
}

[Serializable]
public class InventorySlotSave
{
    public string id;
    public int count;
    public int level;
    public int awakenLevel;
    public bool isOwned;
    public bool isEquipped;
}