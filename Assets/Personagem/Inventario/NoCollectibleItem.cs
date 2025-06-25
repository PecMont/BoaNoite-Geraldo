using UnityEngine;

public class NoCollectibleItem : MonoBehaviour
{
    public ItemData itemData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public string Use()
    {
        if(itemData != null && InventoryManager.Instance != null)
        {
            string a = itemData.description; // Exibe a descrição do item no feedbackText
            return a;
        }
        else
        {
            Debug.LogError("InventoryManager não encontrado na cena!", gameObject);
            return "Erro: InventoryManager não encontrado!";
        }
    }
}
