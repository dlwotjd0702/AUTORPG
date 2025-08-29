using IdleRPG;
using UnityEngine;
using Inventory;
using Stats;

public class RewardManager : MonoBehaviour
{
    public UpgradeManager upgradeManager;
    public InventorySystem inventorySystem;
    public GachaSystem gachaSystem;
    public PlayerGemManager gemManager;
    public void GrantReward(Player player, Monster monster)
    {
        // 경험치, 골드
        if (player && player.playerStats)
            player.playerStats.AddExp(monster.expReward);

        if (upgradeManager)
            upgradeManager.AddGold(monster.goldReward);

        Vector3 textPos = monster.transform.position + Vector3.up * 0f;

        // 골드
        if (monster.goldReward > 0)
            FloatingText3DPool.Instance.Spawn(textPos, $"+{monster.goldReward}G", new Color(1f, 0.85f, 0.2f)); // 노랑
        textPos = monster.transform.position + Vector3.up * -0.5f;
        // 경험치
        if (monster.expReward > 0)
            FloatingText3DPool.Instance.Spawn(textPos + Vector3.right * 0.6f, $"+{monster.expReward} Exp", Color.cyan);

        // --- 아이템 드랍 판정 ---
        if (gachaSystem != null && upgradeManager != null && inventorySystem != null)
        {
            int maxGrade = 20;
            for (int grade = maxGrade; grade >= 1; grade--)
            {
                float dropChance = upgradeManager.GetDropChanceForGrade(grade);
                if (Random.value < dropChance)
                {
                    string[] itemTypes = { "weapon", "armor", "ring", "skill" };
                    string type = itemTypes[Random.Range(0, itemTypes.Length)];
                    string itemId = $"{type}_{grade:D2}";

                    var data = inventorySystem.GetEquipmentData(itemId);
                    if (data != null)
                        inventorySystem.AddItem(data, 1);

                    // 필요시 시각적 이펙트/팝업 등 처리
                   // Debug.Log($"[드랍] {itemId} 획득!");
                    break; // 한 번만 드랍
                }
            }
        }
        float gemDropChance = 0.005f; // 0.5%
        if (UnityEngine.Random.value < gemDropChance)
        {
            gemManager.AddGems(1);
            Debug.Log("[Reward] 보석 드랍됨! 현재 보석: " + gemManager.Gems);
        }
    }
}