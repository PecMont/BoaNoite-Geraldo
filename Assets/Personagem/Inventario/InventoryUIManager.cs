// Scripts/InventoryUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public GameObject inventoryPanel;           // Arraste o seu InventoryPanel aqui
    public Transform slotsContainer;            // Arraste o seu SlotsContainer aqui
    public GameObject inventorySlotPrefab;      // Arraste o seu InventorySlotUIPrefab aqui

    public KeyCode inventoryToggleKey = KeyCode.I; // Tecla para abrir/fechar inventário

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private bool isInventoryOpen = false;

    [Header("Painel de Detalhes do Item")]
    public GameObject detailsPanel; // Opcional: para ativar/desativar o painel inteiro
    public Image selectedItemIcon;
    public TextMeshProUGUI selectedItemDescription;
    public GameObject descriptionBackground;
    public bool cursorLocked = true; // Se o cursor deve ser travado quando o inventário está aberto

    void Awake()
    {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        /// Limpa e esconde o painel de detalhes.
    public void ClearItemDetails()
    {
        Debug.Log("Limpando conteúdo do painel de detalhes.");

        if (descriptionBackground != null) 
        descriptionBackground.SetActive(false);

        
        // Esta parte do código, que limpa o conteúdo, está correta e deve permanecer.
        if (selectedItemIcon != null)
        {
            // Esconde o componente Image para que o ícone desapareça
            selectedItemIcon.enabled = false; 

            // Também garantimos que não há nenhuma sprite atribuída
            selectedItemIcon.sprite = null;
        }

        if (selectedItemDescription != null)
        {
            // Limpa o texto da descrição
            selectedItemDescription.text = ""; 
        }
    }

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false); // Garante que começa fechado

        // Prepara os slots visuais (pode criar com base no maxSlots do InventoryManager se quiser)
        // Por simplicidade, vamos assumir um número fixo ou que o InventoryManager.maxSlots exista
        int numberOfSlotsToDisplay = InventoryManager.Instance != null ? InventoryManager.Instance.maxSlots : 10; // Ou um valor fixo

        for (int i = 0; i < numberOfSlotsToDisplay; i++)
        {
            if (inventorySlotPrefab == null || slotsContainer == null)
            {
                Debug.LogError("InventorySlotPrefab ou SlotsContainer não está definido no InventoryUIManager!");
                return;
            }
            GameObject slotGO = Instantiate(inventorySlotPrefab, slotsContainer);
            InventorySlotUI slotUIScript = slotGO.GetComponent<InventorySlotUI>();
            if (slotUIScript != null)
            {
                slotUIs.Add(slotUIScript);
                slotUIScript.ClearSlot(); // Começa limpo
            }
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateUI; // Se inscreve para atualizações
        }
        UpdateUI(); // Atualização inicial
        ClearItemDetails();
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateUI; // Cancela inscrição
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(inventoryToggleKey))
        {
            ToggleInventory();
            controllCursor(!isInventoryOpen);
        }

    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Time.timeScale = 0f; // Pausa o jogo
            UpdateUI(); // Garante que a UI está atualizada ao abrir
            // Aqui você pode adicionar lógica para travar o cursor do mouse, etc.
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f; // Despausa o jogo
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }

    public void UpdateUI()
    {
        if (InventoryManager.Instance == null) return;

        List<InventorySlot> playerItems = InventoryManager.Instance.items;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < playerItems.Count)
            {
                slotUIs[i].DisplayItem(playerItems[i]);
            }
            else
            {
                slotUIs[i].ClearSlot();
            }
        }
    }
    public void DisplayItemDetails(ItemData item)
    {
        if (item == null)
        {
            ClearItemDetails();
            return;
        }

        if (detailsPanel != null) detailsPanel.SetActive(true);

        if (descriptionBackground != null) 
        descriptionBackground.SetActive(true);

        selectedItemIcon.sprite = item.icon;
        selectedItemIcon.enabled = (item.icon != null); // Só mostra a imagem se houver um ícone

        selectedItemDescription.text = item.description;
    }

    public void controllCursor(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}