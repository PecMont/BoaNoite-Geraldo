// Scripts/InventorySlot.cs
// (Certifique-se que o nome do arquivo é InventorySlot.cs)

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

    // MÉTODO QUE ESTAVA FALTANDO OU INCORRETO:
    public void RemoveQuantity(int amount)
    {
        quantity -= amount;
        if (quantity < 0) // Opcional: garantir que a quantidade não fique negativa
        {
            quantity = 0;
        }
    }
}