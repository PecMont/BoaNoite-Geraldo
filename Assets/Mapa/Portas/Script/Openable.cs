using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// O namespace foi mantido para compatibilidade com seu script de câmera.
// Se você mudar o nome do arquivo para Openable.cs, o script da câmera ainda encontrará o componente.
namespace DoorScript
{

    [RequireComponent(typeof(AudioSource))]
    public class Openable : MonoBehaviour // --- MUDANÇA: Nome da classe alterado para ser mais genérico.
    {
        // --- NOVO: Enum para escolher o tipo de movimento no Inspector ---
        public enum OpenType { Rotate, Slide }
        public int id; 
        [Header("Configuração Geral")]
        public OpenType openType = OpenType.Rotate; // Escolha entre Porta (Rotate) ou Gaveta (Slide)
        public TMPro.TextMeshProUGUI feedbackText;
        public float smooth = 1.0f;
        public bool open;

        [Header("Configuração de Rotação (Porta)")]
        public Vector3 openRotation = new Vector3(0, -90, 0); // Rotação da porta quando ABERTA
        private Vector3 closeRotation; // Rotação inicial da porta (fechada)

        // --- NOVO: Variáveis para o movimento da gaveta ---
        [Header("Configuração de Movimento (Gaveta)")]
        public Vector3 openPositionOffset; // O quanto a gaveta deve se mover a partir da posição inicial
        private Vector3 closePosition; // Posição inicial da gaveta (fechada)

        [Header("Áudio")]
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        [Header("Requisitos de Itens")]
        public List<ItemData> requiredItems;
        public int requiredProgress;
        private bool isUnlocked = false;

        void Start()
        {
            if (asource == null)
            {
                asource = GetComponent<AudioSource>();
            }

            // Armazena os estados iniciais (fechado) para ambos os tipos
            closeRotation = transform.localEulerAngles;
            closePosition = transform.localPosition; // Usamos localPosition para que o movimento seja relativo ao objeto pai

            if (requiredItems == null || requiredItems.Count == 0)
            {
                isUnlocked = true;
            }
        }

        void Update()
        {
            // --- MUDANÇA: Usa um switch para decidir qual animação executar ---
            switch (openType)
            {
                case OpenType.Rotate:
                    AnimateRotation();
                    break;
                case OpenType.Slide:
                    AnimatePosition();
                    break;
            }
        }

        // Lógica de animação para rotação (portas)
        void AnimateRotation()
        {
            if (open)
            {
                var target = Quaternion.Euler(openRotation);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                var target1 = Quaternion.Euler(closeRotation);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }
        }

        // --- NOVO: Lógica de animação para translação (gavetas) ---
        void AnimatePosition()
        {
            if (open)
            {
                // O alvo é a posição inicial + o deslocamento
                var targetPosition = closePosition + openPositionOffset;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 5 * smooth);
            }
            else
            {
                // O alvo é a posição inicial
                transform.localPosition = Vector3.Lerp(transform.localPosition, closePosition, Time.deltaTime * 5 * smooth);
            }
        }

        //
        // O RESTO DO CÓDIGO PERMANECE EXATAMENTE O MESMO
        // A lógica de destravar e interagir não precisa mudar.
        //

        public void OpenDoor() // O nome do método foi mantido para não quebrar seu outro script
        {
            // Verifica se o progresso do jogador é suficiente para destravar a porta
            if(GameProgression.instance.Progresso < requiredProgress)
            {
                
                    if(GameProgression.instance.Progresso < 4 && GameProgression.instance.Progresso > 2 && id == 1)
                        GameProgression.instance.Progresso++;         
                    else{
                        feedbackText.text = "Ainda preciso terminar minnha tarefa antes de abrir esta porta.";
                        Invoke("Limparfala", 3f);
                    }
                return;
            }
            if(GameProgression.instance.Progresso < 8 && GameProgression.instance.Progresso > 5 && id == 3)
                    GameProgression.instance.Progresso++;

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
                Debug.Log("VERIFICAÇÃO COMPLETA: Sucesso! O jogador tem TODOS os itens. Destrancando e abrindo.");
                isUnlocked = true;
                open = true;

                foreach (var itemRequerido in requiredItems)
                {
                    InventoryManager.Instance.RemoveItem(itemRequerido, 1);
                }

                Limparfala();
                feedbackText.text = "Destrancado!";
                Invoke("Limparfala", 2f);
                PlaySound();
            }
            else
            {
                Debug.Log("VERIFICAÇÃO COMPLETA: Falha! Faltam um ou mais itens. Continua trancado.");
                Limparfala();
                feedbackText.text = "Trancado!";
                Invoke("Limparfala", 1f);
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

    void Limparfala()
    {        
        feedbackText.text = "";
    }
    }
}