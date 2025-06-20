// Nome do arquivo: Door.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open;
        public float smooth = 1.0f;
        float DoorOpenAngle = -90.0f; // Ângulo para a porta aberta
        float DoorCloseAngle = 0.0f;  // Ângulo para a porta fechada
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Requisitos")]
        // --- MUDANÇA 1: Trocar um único ItemData por uma Lista de ItemData ---
        public List<ItemData> requiredItems; // Lista de itens necessários para abrir
        private bool isUnlocked = false; // Garante que a porta seja destrancada apenas uma vez

        void Start()
        {
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
            if (open)
            {
                var target = Quaternion.Euler(0, DoorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                var target1 = Quaternion.Euler(0, DoorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }
        }

        // Dentro do seu script Door.cs, substitua o método OpenDoor por este:
public void OpenDoor()
{
    // Se a porta já está destrancada, ela pode abrir e fechar normalmente
    if (isUnlocked)
    {
        ToggleDoor();
        return;
    }

    // --- LÓGICA DE VERIFICAÇÃO ATUALIZADA ---
    Debug.Log("Tentando destravar a porta. Verificando itens necessários...");

    // Etapa 1: VERIFICAR se o jogador tem TODOS os itens da lista
    bool hasAllItems = true; // Começamos assumindo que o jogador tem tudo

    // Passamos por cada item na nossa lista de requisitos
    foreach (var itemRequerido in requiredItems)
    {
        // Se o item não for encontrado no inventário do jogador...
        if (!InventoryManager.Instance.HasItem(itemRequerido))
        {
            Debug.Log($"FALHA NA VERIFICAÇÃO: Jogador NÃO TEM o item '{itemRequerido.itemName}'.");
            hasAllItems = false; // ...marcamos que ele não tem tudo...
            break; // ...e paramos de verificar o resto, pois já sabemos que falhou.
        }
        else
        {
            Debug.Log($"SUCESSO NA VERIFICAÇÃO: Jogador TEM o item '{itemRequerido.itemName}'.");
        }
    }

    // Etapa 2: AGIR com base no resultado da verificação
    
    // Se, após o loop, a variável 'hasAllItems' ainda for 'true'...
    if (hasAllItems)
    {
        Debug.Log("VERIFICAÇÃO COMPLETA: Sucesso! O jogador tem TODOS os itens. Destrancando e abrindo a porta.");
        isUnlocked = true; // Destranca a porta permanentemente
        open = true;       // Força a porta a abrir na primeira vez

        // Etapa 3: Agora sim, REMOVEMOS todos os itens requeridos do inventário
        foreach (var itemRequerido in requiredItems)
        {
            InventoryManager.Instance.RemoveItem(itemRequerido, 1);
        }

        PlaySound(); // Toca o som de abrir
    }
    else // Se 'hasAllItems' se tornou 'false' em algum momento...
    {
        Debug.Log("VERIFICAÇÃO COMPLETA: Falha! Faltam um ou mais itens. A porta continua trancada.");
        // Aqui você pode tocar um som de "trancado" se quiser
        // if (feedbackLockedSound != null) asource.PlayOneShot(feedbackLockedSound);
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

        // Para facilitar os testes, você pode adicionar um OnMouseDown se não tiver um script de interação central
        private void OnMouseDown()
        {
            OpenDoor();
        }
    }
}