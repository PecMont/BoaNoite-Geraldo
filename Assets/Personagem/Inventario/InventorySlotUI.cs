// Scripts/InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI quantityText;

    private ItemData currentItemData; // Guarda o item que este slot está mostrando

    public void DisplayItem(InventorySlot inventoryItemSlot) // Recebe um InventorySlot do seu sistema lógico
    {
        if (inventoryItemSlot == null || inventoryItemSlot.itemData == null)
        {
            ClearSlot();
            return;
        }

        currentItemData = inventoryItemSlot.itemData;

        if (itemIconImage != null)
        {
            itemIconImage.sprite = inventoryItemSlot.itemData.icon; // Assume que seu ItemData tem um campo 'icon' do tipo Sprite
            itemIconImage.enabled = true;
        }

        if (quantityText != null)
        {
            if (inventoryItemSlot.quantity > 1 && inventoryItemSlot.itemData.isStackable)
            {
                quantityText.text = inventoryItemSlot.quantity.ToString();
                quantityText.enabled = true;
            }
            else
            {
                quantityText.enabled = false; // Não mostra quantidade se for 1 ou não empilhável
            }
        }
    }

    public void ClearSlot()
    {
        currentItemData = null; // Limpa o item atual

        if (itemIconImage != null)
        {
            itemIconImage.sprite = null;
            itemIconImage.enabled = false;
        }
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
    }

    public void OnSlotClicked()
    {
        // Se este slot não estiver vazio, informa o InventoryUIManager qual item foi selecionado
        if (currentItemData != null && InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.DisplayItemDetails(currentItemData);
        }
        // Opcional: Se o slot estiver vazio, você pode querer limpar o painel de detalhes
        else if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.ClearItemDetails();
        }
            
    }
}
