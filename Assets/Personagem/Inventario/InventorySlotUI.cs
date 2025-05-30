// Scripts/InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIconImage;
    public TextMeshProUGUI quantityText;

    public void DisplayItem(InventorySlot inventoryItemSlot) // Recebe um InventorySlot do seu sistema lógico
    {
        if (inventoryItemSlot == null || inventoryItemSlot.itemData == null)
        {
            ClearSlot();
            return;
        }

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
}