using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Usaremos para encontrar itens na lista
using System; // Para usar Action

public class InventoryManager : MonoBehaviour
{
    // Singleton: uma forma fácil de acessar o InventoryManager de qualquer outro script
    public static InventoryManager Instance { get; private set; }

    [Header("Configurações do Inventário")]
    public int maxSlots = 20;

    public List<InventorySlot> items = new List<InventorySlot>();
    
    // Evento para notificar mudanças no inventário
    public event Action OnInventoryChanged;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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
                    OnInventoryChanged?.Invoke(); // Notifica mudança
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
        if (items.Count < maxSlots) // Verifica limite de slots
        {
            items.Add(new InventorySlot(itemToAdd, amount));
            Debug.Log($"Adicionado {amount}x {itemToAdd.itemName} a um novo slot.");
            PrintInventory(); // Mostra o inventário no console
            OnInventoryChanged?.Invoke(); // Notifica mudança
            return true;
        }

        Debug.LogWarning($"Inventário cheio! Não foi possível adicionar {itemToAdd.itemName}.");
        return false;
    }

    public bool RemoveItem(ItemData itemToRemove, int amount = 1)
    {
        if (itemToRemove == null || amount <= 0) return false;

        InventorySlot slot = items.FirstOrDefault(s => s.itemData.id == itemToRemove.id);
        if (slot != null)
        {
            if (slot.quantity >= amount)
            {
                slot.RemoveQuantity(amount);
                if (slot.quantity <= 0)
                {
                    items.Remove(slot);
                }
                Debug.Log($"Removido {amount}x {itemToRemove.itemName}");
                OnInventoryChanged?.Invoke(); // Notifica mudança
                return true;
            }
        }
        return false;
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