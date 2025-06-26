using Stats;
using TMPro;
using UnityEngine;

public class CombatPowerUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI powerText;

    void Start()
    {
        playerStats.OnStatsChanged += UpdateCombatPower;
        UpdateCombatPower();
    }

    void UpdateCombatPower()
    {
        powerText.text = $"전투력: {playerStats.CombatPower:N0}";
    }
}