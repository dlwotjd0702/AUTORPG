using Firebase.Firestore;
using System;
using System.Collections.Generic;

[FirestoreData]
public class SaveData
{
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int exp { get; set; }
    [FirestoreProperty] public int expToLevelUp { get; set; }

    [FirestoreProperty] public int gold { get; set; }
    [FirestoreProperty] public int atkUpgradeLevel { get; set; }
    [FirestoreProperty] public int defUpgradeLevel { get; set; }
    [FirestoreProperty] public int hpUpgradeLevel { get; set; }
    [FirestoreProperty] public int atkSpdUpgradeLevel { get; set; }
    [FirestoreProperty] public int critRateUpgradeLevel { get; set; }
    [FirestoreProperty] public int critDmgUpgradeLevel { get; set; }
    [FirestoreProperty] public int dropRateUpgradeLevel { get; set; }

    // 웨이브/스테이지 정보
    [FirestoreProperty] public int currentStage { get; set; }
    [FirestoreProperty] public int currentWave { get; set; }
    [FirestoreProperty] public int maxClearedStage { get; set; }
    [FirestoreProperty] public int maxClearedWave { get; set; }
    [FirestoreProperty] public List<StageWaveRecord> clearedStageWave { get; set; } = new List<StageWaveRecord>();
    [FirestoreProperty] public int progressMode { get; set; } // 0: Repeat, 1: Advance

    // 인벤토리
    [FirestoreProperty] public List<InventorySlotSave> inventorySlots { get; set; } = new List<InventorySlotSave>();

    [FirestoreProperty] public string savedAt { get; set; }
    [FirestoreProperty] public string nickname { get; set; }
    
    [FirestoreProperty] public List<string> equippedSkillIds { get; set; } = new List<string>();

    // 저장 시점 기록 (이 메서드는 그대로 사용 가능)
    public void SetSaveTime()
    {
        savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[FirestoreData]
public class StageWaveRecord
{
    [FirestoreProperty] public int stage { get; set; }
    [FirestoreProperty] public int wave { get; set; }
}

[FirestoreData]
public class InventorySlotSave
{
    [FirestoreProperty] public string id { get; set; }
    [FirestoreProperty] public int count { get; set; }
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int awakenLevel { get; set; }
    [FirestoreProperty] public bool isOwned { get; set; }
    [FirestoreProperty] public bool isEquipped { get; set; }
}
