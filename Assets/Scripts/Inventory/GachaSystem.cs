using Inventory;
using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public EquipmentData[] gachaPool; // 뽑기 풀(아이템 데이터 배열)

    // 인덱스/확률/랜덤 등은 상황 따라 구현
    public void RollGacha()
    {
        int rand = Random.Range(0, gachaPool.Length);
        EquipmentData result = gachaPool[rand];
        inventorySystem.AddItem(result, 1);
        Debug.Log($"[가챠] {result.name} 획득!");
    }

    // 확률/등급별/아이템 타입별 뽑기 등 추가 가능
}