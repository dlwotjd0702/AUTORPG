using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Inventory;

[Serializable]
public class GachaLevelInfo
{
    public int level = 1;
    public int exp = 0;
    public Dictionary<int, float> gradeRates = new();
}

public class GachaSystem : MonoBehaviour
{
    public int maxGrade = 20;
    public Dictionary<ItemType, GachaLevelInfo> gachaLevels = new();
    public InventorySystem inventorySystem;
    public PlayerGemManager gemManager;

    public int singleRollCost = 5;
    public int tenRollCost = 45;

    void Awake()
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            gachaLevels[type] = new GachaLevelInfo();
            UpdateGradeRates(type);
        }
    }

    public bool RollGacha(ItemType type, int count)
    {
        Debug.Log($"[Gacha] RollGacha 호출! type={type}, count={count}");

        int cost = (count == 10) ? tenRollCost : singleRollCost * count;
        if (!gemManager.SpendGems(cost))
        {
            Debug.Log($"[Gacha] 보석이 부족해서 뽑기 취소! (보유:{gemManager.Gems}/필요:{cost})");
            return false;
        }

        var info = gachaLevels[type];

        // 등급 확률을 for문 시작 전에 복사
        var ratesCopy = info.gradeRates.ToList();

        Debug.Log($"[Gacha] for문 진입! count={count}");
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"[Gacha] for문 내부, i={i}");

            int grade = RollGradeForGachaFromRates(ratesCopy);
            string itemId = $"{type}_{grade:D2}"; 
            Debug.Log($"[Gacha] 아이템 뽑기 시도: {itemId}");

            EquipmentData data = inventorySystem.GetEquipmentData(itemId);
            Debug.Log(data.type.ToString());
            inventorySystem.AddItem(data, 1);

          info.exp += 1;
            if (info.exp >= GetLevelUpExp(info.level))
            {
                info.exp = 0;
                info.level++;
                Debug.Log($"[Gacha] {type} 가챠 레벨업! 현재 Lv.{info.level}");
                UpdateGradeRates(type);
            }
        }
        Debug.Log("[Gacha] for문 종료");
        return true;
    }

    public int RollGradeForGachaFromRates(List<KeyValuePair<int, float>> ratesList)
    {
        float rand = UnityEngine.Random.value;
        float acc = 0f;
        foreach (var kvp in ratesList.OrderBy(k => k.Key))
        {
            acc += kvp.Value;
            if (rand < acc)
                return kvp.Key;
        }
        return 1;
    }


    public int GetLevelUpExp(int level)
    {
        return 20 + (level - 1) * 5;
    }

   

    // 등급 미만 가챠레벨에선 확률 0
    void UpdateGradeRates(ItemType type)
    {
        var info = gachaLevels[type];
        int level = info.level;
        float baseRate = 0.5f;
        Dictionary<int, float> rates = new();
        float sum = 0f;
        for (int grade = 1; grade <= maxGrade; grade++)
        {
            if (grade > 1 && level < grade)
            {
                rates[grade] = 0f;
            }
            else
            {
                float rate = baseRate / Mathf.Pow(2, grade - 1);
                if (grade > 1)
                    rate += (grade - 1) * level * 0.001f;
                rates[grade] = rate;
                sum += rate;
            }
        }
        foreach (var key in rates.Keys.ToList())
        {
            if (sum == 0)
                rates[1] = 1f;
            else
                rates[key] /= sum;
        }
        info.gradeRates = rates;
    }

    // 4개씩 5줄 (20등급)
    public string GetDropRateStringGrid(ItemType type)
    {
        var rates = gachaLevels[type].gradeRates;
        int gradesPerLine = 4;
        int lines = Mathf.CeilToInt(maxGrade / (float)gradesPerLine);

        string result = "";
        for (int l = 0; l < lines; l++)
        {
            var lineRates = new List<string>();
            for (int g = 1; g <= gradesPerLine; g++)
            {
                int grade = l * gradesPerLine + g;
                if (grade > maxGrade) break;
                if (rates.TryGetValue(grade, out float rate))
                    lineRates.Add($"{grade}등급: {(rate * 100f):0.###}%");
            }
            result += string.Join("    ", lineRates) + "\n";
        }
        result += $"(최대 {maxGrade}등급)";
        return result;
    }
}
