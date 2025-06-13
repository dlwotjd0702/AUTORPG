using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class CollectionBook : MonoBehaviour
{
    private HashSet<string> obtainedIds = new HashSet<string>();
    public List<EquipmentData> collectionList;

    // 도감 등록
    public void Register(EquipmentData data)
    {
        if (obtainedIds.Add(data.id))
            collectionList.Add(data);
    }

    // 보유 여부
    public bool Has(string id) => obtainedIds.Contains(id);
}