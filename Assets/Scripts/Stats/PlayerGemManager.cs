using Combat;
using TMPro;
using UnityEngine;

public class PlayerGemManager : MonoBehaviour, ISaveable
{
    public int Gems = 100000;

    public TextMeshProUGUI gemText; // UI에 연결

    private void Start()
    {
        RefreshUI();
    }

    public void AddGems(int amount)
    {
        Gems += amount;
        Debug.Log($"[Gem] 보석 {amount}개 획득! 현재 보석: {Gems}");
        RefreshUI();
    }

    public bool SpendGems(int amount)
    {
        if (Gems >= amount)
        {
            Gems -= amount;
            Debug.Log($"[Gem] 보석 {amount}개 사용! 현재 보석: {Gems}");
            RefreshUI();
            return true;
        }
        Debug.LogWarning("[Gem] 보석이 부족합니다!");
        return false;
    }

    public void RefreshUI()
    {
        if (gemText)
            gemText.text = $"젬 : <b>{Gems:N0}</b>";
    }

    // --- ISaveable 구현 ---
    public void CollectSaveData(SaveData data)
    {
        data.gems = Gems;
    }

    public void ApplyLoadedData(SaveData data)
    {
        Gems = data?.gems ?? 0;
        RefreshUI();
    }
}