using IdleRPG;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public int gold = 0;

    public Player player;
    public PlayerStats playerStats;

    private void Awake()
    {
        if (!player) player = FindObjectOfType<Player>();
        if (!playerStats) playerStats = player.GetComponent<PlayerStats>();
    }

    // 각 업그레이드별 단계(몇 번 강화했는지)
    private int atkUpgradeLevel = 0;
    private int defUpgradeLevel = 0;
    private int hpUpgradeLevel = 0;
    private int atkSpdUpgradeLevel = 0;
    private int critRateUpgradeLevel = 0;
    private int critDmgUpgradeLevel = 0;

    // 기본 비용 및 증가율
    private int attackUpgradeBaseCost = 100;
    private int defenseUpgradeBaseCost = 80;
    private int hpUpgradeBaseCost = 120;
    private int atkSpdUpgradeBaseCost = 200;
    private int critRateUpgradeBaseCost = 300;
    private int critDmgUpgradeBaseCost = 300;
    private float costIncreaseRate = 0.1f; // 20%씩 증가

    // ---------- 동적 비용 계산 메서드 ----------
    int CalcUpgradeCost(int baseCost, int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1f + costIncreaseRate, level));
    }

    // ---------- 업그레이드 메서드 ----------
    public bool UpgradeAttack(int amount = 1)
    {
        int cost = CalcUpgradeCost(attackUpgradeBaseCost, atkUpgradeLevel);
        if (gold < cost) return false;
        gold -= cost;
        atkUpgradeLevel++;
        player.baseAttack += amount;
        playerStats.RefreshStats();
        Debug.Log($"[GoldManager] 공격력 +{amount} 업! (강화단계: {atkUpgradeLevel}, 비용: {cost}, 현재공격력: {player.baseAttack})");
        return true;
    }
    public bool UpgradeDefense(int amount = 1)
    {
        int cost = CalcUpgradeCost(defenseUpgradeBaseCost, defUpgradeLevel);
        if (gold < cost) return false;
        gold -= cost;
        defUpgradeLevel++;
        player.baseDefense += amount;
        playerStats.RefreshStats();
        Debug.Log($"[GoldManager] 방어력 +{amount} 업! (강화단계: {defUpgradeLevel}, 비용: {cost}, 현재방어력: {player.baseDefense})");
        return true;
    }
    public bool UpgradeHp(int amount = 10)
    {
        int cost = CalcUpgradeCost(hpUpgradeBaseCost, hpUpgradeLevel);
        if (gold < cost) return false;
        gold -= cost;
        hpUpgradeLevel++;
        player.baseHp += amount;
        playerStats.RefreshStats();
        Debug.Log($"[GoldManager] 체력 +{amount} 업! (강화단계: {hpUpgradeLevel}, 비용: {cost}, 현재체력: {player.baseHp})");
        return true;
    }
    public bool UpgradeAtkSpeed(float amount = 0.05f)
    {
        int cost = CalcUpgradeCost(atkSpdUpgradeBaseCost, atkSpdUpgradeLevel);
        if (gold < cost) return false;
        gold -= cost;
        atkSpdUpgradeLevel++;
        player.baseAtkSpeed += amount;
        playerStats.RefreshStats();
        Debug.Log($"[GoldManager] 공격속도 +{amount} 업! (강화단계: {atkSpdUpgradeLevel}, 비용: {cost}, 현재공속: {player.baseAtkSpeed})");
        return true;
    }
    public bool UpgradeCritRate(float amount = 0.01f)
    {
        int cost = CalcUpgradeCost(critRateUpgradeBaseCost, critRateUpgradeLevel);
        if (gold < cost) return false;
        gold -= cost;
        critRateUpgradeLevel++;
        player.baseCritRate += amount;
        playerStats.RefreshStats();
        Debug.Log($"[GoldManager] 크리확률 +{amount * 100}% 업! (강화단계: {critRateUpgradeLevel}, 비용: {cost}, 현재크리: {player.baseCritRate * 100}%)");
        return true;
    }
    public bool UpgradeCritDmg(float amount = 0.05f)
    {
        int cost = CalcUpgradeCost(critDmgUpgradeBaseCost, critDmgUpgradeLevel);
        if (gold < cost) return false;
        gold -= cost;
        critDmgUpgradeLevel++;
        player.baseCritDmg += amount;
        playerStats.RefreshStats();
        Debug.Log($"[GoldManager] 크리뎀 +{amount * 100}% 업! (강화단계: {critDmgUpgradeLevel}, 비용: {cost}, 현재크뎀: {player.baseCritDmg * 100}%)");
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[GoldManager] 골드 획득: {amount} (현재 GOLD: {gold})");
    }
}
