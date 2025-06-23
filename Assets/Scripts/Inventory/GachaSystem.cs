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

    /// <summary>
    /// 뽑기 결과를 반환한다. 보석 부족시 null.
    /// </summary>
    public List<EquipmentData> RollGachaAndGetResult(ItemType type, int count)
    {
        List<EquipmentData> results = new();
        int cost = (count == 10) ? tenRollCost : singleRollCost * count;
        if (!gemManager.SpendGems(cost))
            return null;

        var info = gachaLevels[type];
        var ratesCopy = info.gradeRates.ToList();

        for (int i = 0; i < count; i++)
        {
            int grade = RollGradeForGachaFromRates(ratesCopy);
            string itemId = $"{type}_{grade:D2}";
            EquipmentData data = inventorySystem.GetEquipmentData(itemId);
            inventorySystem.AddItem(data, 1);
            results.Add(data);

            info.exp += 1;
            if (info.exp >= GetLevelUpExp(info.level))
            {
                info.exp = 0;
                info.level++;
                UpdateGradeRates(type);
            }
        }
        return results;
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
