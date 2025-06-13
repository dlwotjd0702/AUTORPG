using Inventory;
using UnityEngine;

public class ItemCombinator : MonoBehaviour
{
    public InventorySystem inventory;

    // 예시: 동일 id, 동일 grade 2개를 합성
    public bool TryCombine(string itemId)
    {
        return inventory.TryCombine(itemId, 2); // 필요한 개수(2개)를 인자로 넘김
    }
}