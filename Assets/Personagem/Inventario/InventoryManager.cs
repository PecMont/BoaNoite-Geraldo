// Scripts/InventoryManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Usaremos para encontrar itens na lista

public class InventoryManager : MonoBehaviour
{
    // Singleton: uma forma fácil de acessar o InventoryManager de qualquer outro script
    public static InventoryManager Instance { get; private set; }

    public List<InventorySlot> items = new List<InventorySlot>();
    // public int maxSlots = 5; // Você pode definir um limite de tipos diferentes de itens depois

    void Awake()
    {
        // Configuração do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destrói duplicatas
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Descomente se quiser que o inventário persista entre cenas
        }
    }

    public bool AddItem(ItemData itemToAdd, int amount = 1)
    {
        if (itemToAdd == null || amount <= 0) return false;

        // Tenta encontrar um slot existente para itens empilháveis
        if (itemToAdd.isStackable)
        {
            InventorySlot existingSlot = items.FirstOrDefault(slot => slot.itemData.id == itemToAdd.id);
            if (existingSlot != null)
            {
                if (existingSlot.quantity + amount <= itemToAdd.maxStackSize)
                {
                    existingSlot.AddQuantity(amount);
                    Debug.Log($"Adicionado {amount}x {itemToAdd.itemName}. Total: {existingSlot.quantity}");
                    PrintInventory(); // Mostra o inventário no console
                    return true;
                }
                else
                {
                    // Se não couber tudo no slot existente, você pode adicionar o restante a um novo slot
                    // ou simplesmente não adicionar se não houver espaço. Por ora, vamos simplificar.
                    Debug.LogWarning($"Não há espaço para empilhar mais {amount} de {itemToAdd.itemName} no slot existente.");
                    // Poderia tentar adicionar o excesso a um novo slot aqui
                }
            }
        }

        // Adiciona a um novo slot (se não for empilhável, ou se for empilhável e não existir/slot cheio)
        // if (items.Count < maxSlots) // Se você for usar limite de slots diferentes
        // {
            items.Add(new InventorySlot(itemToAdd, amount));
            Debug.Log($"Adicionado {amount}x {itemToAdd.itemName} a um novo slot.");
            PrintInventory(); // Mostra o inventário no console
            return true;
        // }

        // Debug.LogWarning($"Inventário cheio (tipos de itens)! Não foi possível adicionar {itemToAdd.itemName}.");
        // return false;
    }

    // Método para mostrar o inventário no console (para teste)
    public void PrintInventory()
    {
        Debug.Log("--- Inventário Atual ---");
        if (items.Count == 0)
        {
            Debug.Log("Vazio.");
        }
        foreach (InventorySlot slot in items)
        {
            Debug.Log($"{slot.itemData.itemName} (x{slot.quantity})");
        }
        Debug.Log("------------------------");
    }
}
