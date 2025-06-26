using System.Collections;
using UnityEngine;

public class BuffSkill : SkillBase
{
    public enum BuffType { Attack, AtkSpeed, CritRate }
    private BuffType buffType;
    private float buffValue;
    private float duration;

    public BuffSkill(EquipmentData data, BuffType type, float value, float dur, SkillManager manager)
        : base(data, manager)
    {
        buffType = type; buffValue = value; duration = dur;
    }

    protected override void UseSkill()
    {
        playerStats.StartCoroutine(BuffRoutine());
        // 이펙트 (플레이어 머리 위)
        var effect = skillManager.GetEffectPrefab(Data.id);
        if (effect)
        {
            var go = Object.Instantiate(effect, playerStats.transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
            go.transform.SetParent(playerStats.transform);
        }
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
            case BuffType.Attack: playerStats.TempAttackBuff += v; break;
            case BuffType.AtkSpeed: playerStats.TempAtkSpeedBuff += v; break;
            case BuffType.CritRate: playerStats.TempCritRateBuff += v; break;
        }
        playerStats.RefreshStats();
    }
}