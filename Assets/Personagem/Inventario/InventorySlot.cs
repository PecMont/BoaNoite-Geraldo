// Scripts/InventorySlot.cs
[System.Serializable] // Permite que seja visto no Inspector e salvo
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public InventorySlot(ItemData item, int amount)
    {
        itemData = item;
        quantity = amount;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
}