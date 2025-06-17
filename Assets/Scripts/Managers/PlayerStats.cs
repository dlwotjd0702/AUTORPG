using System;
using Inventory;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("연동 시스템")]
    public InventorySystem inventory;

    // 반드시 플레이어 컴포넌트를 참조
    public IdleRPG.Player player;

    // 캐싱(자동 갱신)
    public float FinalAttack { get; private set; }
    public float FinalAtkSpeed { get; private set; }
    public float FinalDefense { get; private set; }
    public float FinalHp { get; private set; }
    public float FinalCritRate { get; private set; }
    public float FinalCritDmg { get; private set; }

    private void Awake()
    {
        // 자동으로 Player 컴포넌트 찾아서 연결
        if (!player) player = GetComponent<IdleRPG.Player>();
        if (!inventory) inventory = GetComponent<InventorySystem>();
    }

    private void Start()
    {
        inventory.OnInventoryChanged += RefreshStats;
    }

    public void RefreshStats()
    {
        if (!player)
        {
            Debug.LogError("PlayerStats: Player 참조가 없습니다!");
            return;
        }
        if (!inventory)
        {
            Debug.LogError("PlayerStats: InventorySystem 참조가 없습니다!");
            return;
        }

        // 무기: 공격력, 공격속도
        var (atkMul, atkSpdMul) = inventory.GetWeaponMultipliers();
        FinalAttack = Mathf.FloorToInt(player.baseAttack * atkMul);
        FinalAtkSpeed = player.baseAtkSpeed * atkSpdMul;

        // 방어구: 방어력, 체력
        var (defMul, hpMul) = inventory.GetArmorMultipliers();
        FinalDefense = Mathf.FloorToInt(player.baseDefense * defMul);
        FinalHp = Mathf.FloorToInt(player.baseHp * hpMul);

        // 악세: 크리확률, 크리뎀
        var (critRateMul, critDmgMul) = inventory.GetAccessoryMultipliers();
        FinalCritRate = player.baseCritRate * critRateMul;
        FinalCritDmg = player.baseCritDmg * critDmgMul;

        OnStatsChanged();
    }

    public event System.Action OnStatsChanged = delegate { };
}