// Scripts/InventoryManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System; // Necessário para 'Action'

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<InventorySlot> items = new List<InventorySlot>();

    // IMPORTANTE: Define o número máximo de TIPOS diferentes de itens
    // que o inventário pode conter. Deve corresponder aos seus slots visuais.
    public int maxSlots = 10;

    public Action OnInventoryChanged; // Evento para notificar a UI

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Descomente se precisar que persista entre cenas
        }
    }

    public bool AddItem(ItemData itemToAdd, int amount = 1)
    {
        if (itemToAdd == null || amount <= 0)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ou quantidade inválida.");
            return false;
        }

        // 1. Tenta empilhar o item se ele já existir e for empilhável
        if (itemToAdd.isStackable)
        {
            InventorySlot existingSlot = items.FirstOrDefault(slot => slot.itemData.id == itemToAdd.id);
            if (existingSlot != null)
            {
                if (existingSlot.quantity + amount <= itemToAdd.maxStackSize)
                {
                    existingSlot.AddQuantity(amount);
                    Debug.Log($"Adicionado {amount}x {itemToAdd.itemName} (pilha existente). Total: {existingSlot.quantity}");
                    OnInventoryChanged?.Invoke();
                    return true;
                }
                else
                {
                    // Não pode empilhar a quantidade total no slot existente.
                    // Poderia adicionar o que cabe e tentar criar um novo slot para o restante,
                    // mas para simplificar, vamos dizer que falha se não couber tudo.
                    Debug.LogWarning($"Não há espaço para empilhar mais {amount} de {itemToAdd.itemName} no slot existente (max: {itemToAdd.maxStackSize}). Quantidade atual: {existingSlot.quantity}");
                    return false;
                }
            }
        }

        // 2. Se não pôde empilhar (item novo ou não empilhável), tenta adicionar a um novo slot LÓGICO
        if (items.Count < maxSlots)
        {
            int amountForNewSlot = amount;
            if (itemToAdd.isStackable)
            {
                // Se for um novo item empilhável, a quantidade não pode exceder o tamanho máximo da pilha
                amountForNewSlot = Mathf.Min(amount, itemToAdd.maxStackSize);
            }
            else if (amount > 1)
            {
                // Itens não empilháveis são adicionados um por um, ocupando um slot lógico cada.
                // Esta função adiciona apenas UM item não empilhável por vez a um novo slot.
                // Para adicionar múltiplos itens não empilháveis, chame AddItem múltiplas vezes.
                Debug.LogWarning($"Tentando adicionar {amount} de um item não empilhável ({itemToAdd.itemName}). Apenas 1 será adicionado a este novo slot. Chame AddItem novamente para os demais.");
                amountForNewSlot = 1;
            }

            items.Add(new InventorySlot(itemToAdd, amountForNewSlot));
            Debug.Log($"Adicionado {amountForNewSlot}x {itemToAdd.itemName} a um novo slot lógico.");
            OnInventoryChanged?.Invoke();
            return true;
        }
        else
        {
            Debug.LogWarning($"Inventário cheio de tipos de itens (maxSlots: {maxSlots})! Não foi possível adicionar o novo tipo de item: {itemToAdd.itemName}.");
            return false;
        }
    }

    public bool RemoveItem(ItemData itemToRemove, int amount = 1)
    {
        if (itemToRemove == null || amount <= 0) return false;

        InventorySlot slotToRemoveFrom = items.FirstOrDefault(slot => slot.itemData.id == itemToRemove.id);

        if (slotToRemoveFrom != null)
        {
            if (slotToRemoveFrom.quantity >= amount)
            {
                slotToRemoveFrom.RemoveQuantity(amount); // RemoveQuantity já deve impedir ficar < 0
                Debug.Log($"Removido {amount}x {itemToRemove.itemName}. Restante: {slotToRemoveFrom.quantity}");

                if (slotToRemoveFrom.quantity <= 0)
                {
                    items.Remove(slotToRemoveFrom);
                    Debug.Log($"{itemToRemove.itemName} removido completamente (slot lógico).");
                }
                
                OnInventoryChanged?.Invoke();
                return true;
            }
            else
            {
                Debug.LogWarning($"Quantidade insuficiente de {itemToRemove.itemName} para remover. Pedido: {amount}, Disponível: {slotToRemoveFrom.quantity}.");
                return false;
            }
        }
        Debug.LogWarning($"{itemToRemove.itemName} não encontrado no inventário para remoção.");
        return false;
    }

    // Método para verificar se o jogador possui uma certa quantidade de um item
    public bool HasItem(ItemData itemToCheck, int amountRequired = 1)
    {
        if (itemToCheck == null || amountRequired <= 0) return false;
        InventorySlot slot = items.FirstOrDefault(s => s.itemData.id == itemToCheck.id);
        return slot != null && slot.quantity >= amountRequired;
    }

    // (O método PrintInventory() pode ser mantido para debugging se você quiser)
    public void PrintInventory()
    {
        Debug.Log("--- Inventário Lógico Atual ---");
        if (items.Count == 0)
        {
            Debug.Log("Vazio.");
        }
        for(int i = 0; i < items.Count; i++)
        {
            Debug.Log($"Slot Lógico {i}: {items[i].itemData.itemName} (x{items[i].quantity})");
        }
        Debug.Log($"Total de tipos de itens: {items.Count}/{maxSlots}");
        Debug.Log("-----------------------------");
    }
}