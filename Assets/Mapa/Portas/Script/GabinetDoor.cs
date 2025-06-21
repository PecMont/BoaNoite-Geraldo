// Nome do arquivo: CabinetDoor.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class CabinetDoor : MonoBehaviour
    {
        public bool open;
        public float smooth = 1.0f;
        
        [Header("Configuração da Direção")]
        // Controla se a porta abre para a esquerda (negativo) ou direita (positivo)
        public float openAngleMagnitude = 90.0f; // Ex: 90 para 90 graus de abertura
        public bool openClockwise = true; // Se true, abre no sentido horário (ângulo negativo se for rotação Y)

        private float targetOpenAngle;
        private float targetCloseAngle = 0.0f; // Ângulo para a porta fechada

        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Requisitos")]
        public List<ItemData> requiredItems; // Lista de itens necessários para abrir
        private bool isUnlocked = false; // Garante que a porta seja destrancada apenas uma vez

        [Header("Interação do Jogador")]
        public Transform playerTransform; // Arraste o GameObject do seu jogador aqui no Inspector
        public float interactionDistance = 3.0f; // Distância máxima para interagir com a porta

        void Start()
        {
            if (asource == null)
            {
                asource = GetComponent<AudioSource>();
            }

            // Define o ângulo de abertura com base na direção escolhida
            targetOpenAngle = openClockwise ? -openAngleMagnitude : openAngleMagnitude;

            // Se a lista de itens estiver vazia, a porta já começa destrancada
            if (requiredItems == null || requiredItems.Count == 0)
            {
                isUnlocked = true;
            }

            // Avisa se o playerTransform não foi configurado, útil para depuração
            if (playerTransform == null)
            {
                Debug.LogWarning("Player Transform não está atribuído no script CabinetDoor de " + gameObject.name + ". A interação por 'E' pode não funcionar.", this);
            }
        }

        void Update()
        {
            if (open)
            {
                var target = Quaternion.Euler(0, targetOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                var target1 = Quaternion.Euler(0, targetCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }

            // Lógica para detectar o input da tecla 'E' quando o jogador está próximo
            if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= interactionDistance)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoor(); // Chama o método para abrir/destravar a porta
                }
            }
        }

        public void OpenDoor()
        {
            // Se a porta já está destrancada, ela pode abrir e fechar normalmente
            if (isUnlocked)
            {
                ToggleDoor();
                return;
            }

            Debug.Log("Tentando destravar a porta. Verificando itens necessários...");

            bool hasAllItems = true;

            foreach (var itemRequerido in requiredItems)
            {
                if (!InventoryManager.Instance.HasItem(itemRequerido))
                {
                    Debug.Log($"FALHA NA VERIFICAÇÃO: Jogador NÃO TEM o item '{itemRequerido.itemName}'.");
                    hasAllItems = false;
                    break;
                }
                else
                {
                    Debug.Log($"SUCESSO NA VERIFICAÇÃO: Jogador TEM o item '{itemRequerido.itemName}'.");
                }
            }
            
            if (hasAllItems)
            {
                Debug.Log("VERIFICAÇÃO COMPLETA: Sucesso! O jogador tem TODOS os itens. Destrancando e abrindo a porta.");
                isUnlocked = true; // Destranca a porta permanentemente
                open = true;       // Força a porta a abrir na primeira vez

                foreach (var itemRequerido in requiredItems)
                {
                    InventoryManager.Instance.RemoveItem(itemRequerido, 1);
                }

                PlaySound();
            }
            else
            {
                Debug.Log("VERIFICAÇÃO COMPLETA: Falha! Faltam um ou mais itens. A porta continua trancada.");
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

        // O método OnMouseDown() foi removido para priorizar a interação via tecla 'E'.
    }
}