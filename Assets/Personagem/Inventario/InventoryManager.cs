using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Usaremos para encontrar itens na lista
using System; // Para usar Action

public class InventoryManager : MonoBehaviour
{
    // Singleton: uma forma fácil de acessar o InventoryManager de qualquer outro script
    public static InventoryManager Instance { get; private set; }

    [Header("Configurações do Inventário")]
    public int maxSlots = 6;

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
                    Debug.LogWarning($"Não há espaço para empilhar mais {amount} de {itemToAdd.itemName} no slot existente.");
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

    public bool HasItem(ItemData itemToCheck, int amountRequired = 1)
{
    // Validação inicial para evitar erros
    if (itemToCheck == null || amountRequired <= 0)
    {
        return false;
    }

    // Usa LINQ (FirstOrDefault) para procurar na lista 'items' por um slot
    // cujo itemData tenha o mesmo 'id' do item que estamos procurando.
    InventorySlot slot = items.FirstOrDefault(s => s.itemData.itemName == itemToCheck.itemName);

    // Retorna true APENAS se ambas as condições forem verdadeiras:
    // 1. O slot foi encontrado (slot != null).
    // 2. A quantidade no slot encontrado é maior ou igual à quantidade necessária.
    return slot != null && slot.quantity >= amountRequired;
}
}