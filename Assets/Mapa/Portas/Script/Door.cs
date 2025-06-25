using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI feedbackText;

        public bool open;
        public float smooth = 1.0f;

        // --- MUDANÇA 1: Substituir os ângulos fixos por um Vector3 público ---
        [Header("Configuração da Abertura")]
        public Vector3 openRotation = new Vector3(0, -90, 0); // Define a rotação da porta quando ABERTA
        private Vector3 closeRotation; // Armazena a rotação inicial da porta (fechada)

        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Requisitos")]
        public List<ItemData> requiredItems;
        private bool isUnlocked = false;

        void Start()
        {
            if (asource == null)
            {
                asource = GetComponent<AudioSource>();
            }
            
            // --- MUDANÇA 2: Armazenar a rotação inicial como a rotação "fechada" ---
            // Isso garante que a porta sempre fechará na posição em que começou no cenário.
            closeRotation = transform.localEulerAngles;

            if (requiredItems == null || requiredItems.Count == 0)
            {
                isUnlocked = true;
            }
        }

        void Update()
        {
            // --- MUDANÇA 3: Usar as novas variáveis de rotação ---
            if (open)
            {
                // Usa o Vector3 'openRotation' para definir o alvo
                var target = Quaternion.Euler(openRotation);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                // Usa o Vector3 'closeRotation' para definir o alvo
                var target1 = Quaternion.Euler(closeRotation);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }
        }

        // O resto do seu código permanece exatamente o mesmo, pois a lógica de
        // verificação de itens não precisa ser alterada.
        
        public void OpenDoor()
        {
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
                isUnlocked = true;
                open = true;

                foreach (var itemRequerido in requiredItems)
                {
                    InventoryManager.Instance.RemoveItem(itemRequerido, 1);
                }
                
                ClearFeedbackText();
                feedbackText.text = "Porta destrancada e aberta!!!";
                Invoke("ClearFeedbackText", 2f);
                PlaySound();
            }
            else
            {
                Debug.Log("VERIFICAÇÃO COMPLETA: Falha! Faltam um ou mais itens. A porta continua trancada.");
                ClearFeedbackText();
                feedbackText.text = "Porta trancada!!!";
                Invoke("ClearFeedbackText", 1f);
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
            feedbackText.text = "";
            feedbackText.color = Color.white;
            feedbackText.fontSize = 30;
            feedbackText.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }
}