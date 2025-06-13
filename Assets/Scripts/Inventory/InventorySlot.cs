using Inventory;

[System.Serializable]
public class InventorySlot
{
    public EquipmentData itemData;
    public int count;
    public int level;
    public int awakenLevel;
    public bool isOwned;
    public bool isEquipped;

    public bool IsEmpty => itemData == null;

    public void Clear()
    {
        itemData = null;
        count = 0;
        level = 0;
        awakenLevel = 0;
        isOwned = false;
        isEquipped = false;
    }
}