// ItemData.cs
using UnityEngine;

public enum ItemType // Você pode expandir isso conforme necessário
{
    Generic,
    KeyItem,
    Consumable,
    CombinableResource,
    Readable,
    Equipment // Como o Espelho do Véu
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Informações Básicas")]
    public string id = System.Guid.NewGuid().ToString(); // Identificador único
    public string itemName = "Novo Item";
    [TextArea(3, 5)]
    public string description = "Descrição do item.";
    public Sprite icon = null;
    public ItemType itemType = ItemType.Generic;
    public GameObject itemPrefab; // Prefab do item no mundo (se aplicável)

    [Header("Configurações")]
    public bool isStackable = true;
    public int maxStackSize = 99;
    public bool isUsable = false;
    public bool isCombinable = false;

    [Header("Efeitos (se usável)")]
    public bool affectsSanity = false;
    public float sanityChange = 0; // Positivo para restaurar, negativo para diminuir

    // Adicione mais propriedades conforme necessário, como:
    // public AudioClip useSound;
    // public string associatedPuzzleID; // Para itens de puzzle

    public virtual void Use()
    {
        Debug.Log($"Usando {itemName}.");
        // Lógica base de uso. Pode ser sobrescrita em classes filhas se precisar de comportamentos mais complexos.
        // Por exemplo, se for um Chá Calmante, poderia chamar uma função no Player para restaurar sanidade.
    }
}