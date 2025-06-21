using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public int maxGrade = 20;
    public Dictionary<ItemType, GachaLevelInfo> gachaLevels = new();

    void Awake()
    {
        // 모든 타입별 가챠 레벨 정보 초기화
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            gachaLevels[type] = new GachaLevelInfo();
            UpdateGradeRates(type);
        }
    }

    public void RollGacha(ItemType type, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int grade = RollGradeForGacha(type);
            string itemId = $"{type.ToString().ToLower()}_{grade:D2}";
            // 인벤토리 등 추가처리...
            // inventorySystem.AddItem(data, 1);
            // ...
            // 가챠 횟수 및 레벨업 처리
            var info = gachaLevels[type];
            info.rolls++;
            if (info.rolls >= GetLevelUpThreshold(info.level))
            {
                info.level++;
                info.rolls = 0;
                UpdateGradeRates(type);
            }
        }
    }

    public int RollGradeForGacha(ItemType type)
    {
        var rates = gachaLevels[type].gradeRates;
        float rand = UnityEngine.Random.value;
        float acc = 0f;
        foreach (var kvp in rates.OrderByDescending(k => k.Key))
        {
            acc += kvp.Value;
            if (rand < acc)
                return kvp.Key;
        }
        return 1;
    }

    // 등급별 드랍률 표기용
    public string GetDropRateString(ItemType type)
    {
        var rates = gachaLevels[type].gradeRates;
        return string.Join("\n", rates.OrderByDescending(k => k.Key)
            .Select(kvp => $"{kvp.Key}등급: {(kvp.Value * 100f):0.###}%"));
    }

    // 레벨업 기준 (예시)
    int GetLevelUpThreshold(int level)
    {
        // 예: 10회 → 레벨1, 20회 → 레벨2, 40회 → 레벨3, ...
        return 10 * (int)Mathf.Pow(2, level - 1);
    }

    // 가챠 레벨에 따라 드랍률 재계산
    void UpdateGradeRates(ItemType type)
    {
        var info = gachaLevels[type];
        int level = info.level;
        float baseRate = 0.5f;
        Dictionary<int, float> rates = new();

        float sum = 0f;
        for (int grade = 1; grade <= maxGrade; grade++)
        {
            float rate = baseRate / Mathf.Pow(2, grade - 1);

            // 등급별 가챠레벨 보너스(예시, 고등급일수록 더 크게 증가)
            if (grade > 1)
            {
                // 레벨이 오를 때마다 1등급에서 각 고등급으로 (등급-1)*level*0.002 보정치 분배
                rate += (grade - 1) * level * 0.002f;
            }
            rates[grade] = rate;
            sum += rate;
        }
        // 전체를 1(100%)로 정규화
        foreach (var key in rates.Keys.ToList())
            rates[key] /= sum;

        info.gradeRates = rates;
    }
}
