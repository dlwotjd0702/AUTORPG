using UnityEngine;
using Stats;

public abstract class SkillBase
{
    public EquipmentData Data { get; private set; }
    protected float cooldownTimer;
    protected PlayerStats playerStats;

    public SkillBase(EquipmentData data)
    {
        Data = data;
        cooldownTimer = 0f;
    }

    public void SetOwner(PlayerStats stats) => playerStats = stats;

    public bool IsReady() => cooldownTimer <= 0f;

    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= deltaTime;
    }

    public void TryUseSkill()
    {
        if (IsReady())
        {
            UseSkill();
            cooldownTimer = Data.Cooldown;
        }
        else
        {
            Debug.Log($"스킬 {Data.name} 쿨타임 중: {cooldownTimer:F1}초 남음");
        }
    }

    protected abstract void UseSkill();
}