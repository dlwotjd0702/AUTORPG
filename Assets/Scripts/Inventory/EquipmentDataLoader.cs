using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Inventory;  // EquipmentData, ItemType

public static class EquipmentDataLoader
{
    // TSV 파일을 EquipmentData 리스트로 파싱
    public static List<EquipmentData> LoadFromTSV(string path)
    {
        var list = new List<EquipmentData>();

        // 경로 예시: Application.dataPath + "/Resources/Data/equipment.tsv"
        var lines = File.ReadAllLines(path);

        // 0번째 라인은 헤더, 1번부터 데이터
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var tokens = lines[i].Split('\t');

            // 필드 순서는 TSV와 일치하게!
            var data = new EquipmentData()
            {
                id = tokens[0],
                type = (ItemType)System.Enum.Parse(typeof(ItemType), tokens[1]),
                name = tokens[2],
                grade = int.Parse(tokens[3]),

                ownedAtkPercent = float.Parse(tokens[4]),
                equipAtkPercent = float.Parse(tokens[5]),
                ownedAtkSpdPercent = float.Parse(tokens[6]),
                equipAtkSpdPercent = float.Parse(tokens[7]),
                ownedDefPercent = float.Parse(tokens[8]),
                equipDefPercent = float.Parse(tokens[9]),
                ownedHpPercent = float.Parse(tokens[10]),
                equipHpPercent = float.Parse(tokens[11]),
                ownedCritRatePercent = float.Parse(tokens[12]),
                equipCritRatePercent = float.Parse(tokens[13]),
                ownedCritDmgPercent = float.Parse(tokens[14]),
                equipCritDmgPercent = float.Parse(tokens[15]),

                // 스킬 관련
                skillType = string.IsNullOrWhiteSpace(tokens[16]) ? SkillType.Active : (SkillType)System.Enum.Parse(typeof(SkillType), tokens[16]),
                skillOwnedValue = float.Parse(tokens[17]),
                skillEquipValue = float.Parse(tokens[18]),
                cooldown = float.Parse(tokens[19]),
                skillPower = int.Parse(tokens[20]),
                maxLevel = int.Parse(tokens[21]),
                description = tokens[22],
            };

            list.Add(data);
        }
        return list;
    }
}
