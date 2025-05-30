// Scripts/CollectibleItem.cs
using UnityEngine;

[RequireComponent(typeof(Collider))] // Força ter um Collider
public class CollectibleItem : MonoBehaviour
{
    public ItemData itemData; // Aqui você vai arrastar o asset "PoeiraDeOssos_ItemData"
    public int quantity = 1;

    void Start()
    {
        // Garante que o collider seja um trigger para não bloquear o jogador
        // e para o OnTriggerEnter funcionar
        GetComponent<Collider>().isTrigger = true;
    }

    // Chamado quando outro Collider entra no trigger deste objeto
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem encostou é o jogador (você precisa dar a tag "Player" ao seu jogador)
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (itemData != null && InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(itemData, quantity);
            if (added)
            {
                Debug.Log($"Jogador coletou: {quantity}x {itemData.itemName}");
                Destroy(gameObject); // Destrói o objeto do mundo após coletar
            }
            // Se não adicionou, o InventoryManager já deu um Debug.LogWarning
        }
        else
        {
            if(itemData == null) Debug.LogError("ItemData não definido para este coletável!", gameObject);
            if(InventoryManager.Instance == null) Debug.LogError("InventoryManager não encontrado na cena!", gameObject);
        }
    }
}