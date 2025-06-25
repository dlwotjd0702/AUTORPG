using System.Collections;
using UnityEngine;
using Stats;

public class BuffSkill : SkillBase
{
    public enum BuffType { Attack, AtkSpeed, CritRate }
    private BuffType buffType;
    private float buffValue;
    private float duration;

    public BuffSkill(EquipmentData data, BuffType type, float value, float dur) : base(data)
    {
        buffType = type; buffValue = value; duration = dur;
    }

    protected override void UseSkill()
    {
        playerStats.StartCoroutine(BuffRoutine());
    }

    private IEnumerator BuffRoutine()
    {
        ApplyBuff(true);
        yield return new WaitForSeconds(duration);
        ApplyBuff(false);
    }

    private void ApplyBuff(bool apply)
    {
        float v = apply ? buffValue : -buffValue;
        switch (buffType)
        {
            case BuffType.Attack:    playerStats.TempAttackBuff    += v; break;
            case BuffType.AtkSpeed:  playerStats.TempAtkSpeedBuff  += v; break;
            case BuffType.CritRate:  playerStats.TempCritRateBuff  += v; break;
        }
        playerStats.RefreshStats();
        if (apply) Debug.Log($"[버프 ON] {Data.name} ({buffType}) +{buffValue}");
        else Debug.Log($"[버프 OFF] {Data.name} ({buffType})");
    }
}