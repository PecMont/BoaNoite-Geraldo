// Nome do arquivo: ConfigurableDoor.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adicione a namespace para evitar conflitos se você usar o script Door antigo no mesmo projeto
namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class ConfigurableDoor : MonoBehaviour
    {
        [Header("Controle da Porta")]
        public bool open = false; // Estado atual da porta (aberta/fechada)
        public float smooth = 1.0f; // Suavidade da animação de abrir/fechar

        // --- MUDANÇA PRINCIPAL ---
        // Em vez de ângulos fixos, usamos um Vector3 para definir a rotação de abertura no Inspector.
        // Exemplos: (0, -90, 0) para abrir para a esquerda, (0, 90, 0) para a direita, (-90, 0, 0) para abrir para cima.
        public Vector3 openRotationAngles = new Vector3(0, -90, 0);

        private Quaternion closedRotation; // Armazena a rotação inicial (fechada)
        private Quaternion openRotation;   // Armazena a rotação final (aberta)

        [Header("Feedback Visual e Áudio")]
        public TMPro.TextMeshProUGUI feedbackText; // Referência ao TextMeshProUGUI para feedback
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Requisitos de Itens")]
        public List<ItemData> requiredItems; // Lista de itens necessários para abrir
        private bool isUnlocked = false; // Garante que a porta seja destrancada apenas uma vez

        void Start()
        {
            // Captura a rotação inicial do objeto como a posição "fechada"
            closedRotation = transform.localRotation;
            
            // Calcula a rotação "aberta" com base nos ângulos fornecidos no Inspector
            openRotation = Quaternion.Euler(openRotationAngles);

            if (asource == null)
            {
                asource = GetComponent<AudioSource>();
            }

            // Se a lista de itens estiver vazia, a porta já começa destrancada
            if (requiredItems == null || requiredItems.Count == 0)
            {
                isUnlocked = true;
            }
        }

        void Update()
        {
            // Interpola suavemente para a rotação alvo (aberta ou fechada)
            if (open)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, openRotation, Time.deltaTime * 5 * smooth);
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRotation, Time.deltaTime * 5 * smooth);
            }
        }

        // Este método é chamado para interagir com a porta.
        // A lógica de verificação de itens permanece IDÊNTICA à do seu script original.
        public void OpenDoor()
        {
            // Se a porta já está destrancada, ela pode abrir e fechar normalmente
            if (isUnlocked)
            {
                ToggleDoor();
                return;
            }

            // Lógica de verificação de itens
            Debug.Log("Tentando destravar a porta. Verificando itens necessários...");
            bool hasAllItems = true;
            foreach (var itemRequerido in requiredItems)
            {
                if (!InventoryManager.Instance.HasItem(itemRequerido))
                {
                    hasAllItems = false;
                    break;
                }
            }

            // Se o jogador tem todos os itens
            if (hasAllItems)
            {
                Debug.Log("VERIFICAÇÃO COMPLETA: Sucesso! Destrancando e abrindo a porta.");
                isUnlocked = true;
                open = true; // Força a porta a abrir na primeira vez

                // Remove os itens do inventário
                foreach (var itemRequerido in requiredItems)
                {
                    InventoryManager.Instance.RemoveItem(itemRequerido, 1);
                }
                
                if (feedbackText != null)
                {
                    ClearFeedbackText();
                    feedbackText.text = "Porta destrancada!";
                    Invoke("ClearFeedbackText", 2f);
                }
                PlaySound();
            }
            else // Se faltam itens
            {
                Debug.Log("VERIFICAÇÃO COMPLETA: Falha! Faltam itens. A porta continua trancada.");
                if (feedbackText != null)
                {
                    ClearFeedbackText();
                    feedbackText.text = "Trancada!";
                    Invoke("ClearFeedbackText", 2f);
                }
                // Você pode adicionar um som de "trancado" aqui
            }
        }

        private void ToggleDoor()
        {
            open = !open;
            PlaySound();
        }

        private void PlaySound()
        {
            asource.clip = open ? openDoor : closeDoor;
            if (asource.clip != null)
            {
                asource.Play();
            }
        }

        private void ClearFeedbackText()
        {
            if (feedbackText != null)
            {
                feedbackText.text = "";
            }
        }
    }

    // Você precisará ter a definição da classe ItemData e do InventoryManager em algum lugar do seu projeto.
    // Exemplo (apenas para o código compilar):
    // public class ItemData : ScriptableObject { public string itemName; }
    // public class InventoryManager { public static InventoryManager Instance; public bool HasItem(ItemData item) { return true; } public void RemoveItem(ItemData item, int count) {} }
}