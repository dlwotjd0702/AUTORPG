using Inventory;

[System.Serializable]
public class InventorySlot
{
    public EquipmentData itemData;
    public int count = 0;
    public bool isOwned = false;
    public bool isEquipped = false;
    public bool IsEmpty => itemData == null;
    public void Clear()
    {
        itemData = null;
        count = 0;
        isOwned = false;
        isEquipped = false;
    }
}