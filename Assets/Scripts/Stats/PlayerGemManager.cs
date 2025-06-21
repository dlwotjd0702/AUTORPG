using UnityEngine;

public class PlayerGemManager : MonoBehaviour
{
    public int Gems  = 100000;

    public void AddGems(int amount)
    {
        Gems += amount;
        Debug.Log($"[Gem] 보석 {amount}개 획득! 현재 보석: {Gems}");
        // UI 갱신 이벤트 등 호출 가능
    }

    public bool SpendGems(int amount)
    {
        if (Gems >= amount)
        {
            Gems -= amount;
            Debug.Log($"[Gem] 보석 {amount}개 사용! 현재 보석: {Gems}");
            // UI 갱신 이벤트 등 호출 가능
            return true;
        }
        Debug.LogWarning("[Gem] 보석이 부족합니다!");
        return false;
    }
}