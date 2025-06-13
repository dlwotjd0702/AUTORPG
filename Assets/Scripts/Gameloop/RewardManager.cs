using IdleRPG;
using UnityEngine;
using UnityEngine.Serialization;

public class RewardManager : MonoBehaviour
{
    [FormerlySerializedAs("goldManager")] public UpgradeManager upgradeManager;
    public GachaSystem gachaSystem;

    public void GrantReward(Player player, Monster monster)
    {
        Debug.Log($"[RewardManager] GrantReward: Monster {monster.name} 보상 지급! EXP: {monster.expReward}, GOLD: {monster.goldReward}, 장비ID: {monster.equipmentDropId}, 스킬ID: {monster.skillDropId}");
        // 경험치
        player.AddExp(monster.expReward);

        // 골드
        upgradeManager.AddGold(monster.goldReward);

        // 장비/스킬 도감 해금 (확률/드롭 등)
        /*if (!string.IsNullOrEmpty(monster.equipmentDropId))
            gachaSystem.TryUnlockEquipment(monster.equipmentDropId);

        if (!string.IsNullOrEmpty(monster.skillDropId))
            gachaSystem.TryUnlockSkill(monster.skillDropId);*/

        // 추가 보상 등 확장 가능
    }
}