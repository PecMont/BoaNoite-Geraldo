using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Openable : MonoBehaviour
    {
        public enum OpenType { Rotate, Slide }
        public int id;
        [Header("Configuração Geral")]
        public OpenType openType = OpenType.Rotate;
        public TMPro.TextMeshProUGUI feedbackText;
        public float smooth = 1.0f;
        public bool open;

        [Header("Configuração de Rotação (Porta)")]
        public Vector3 openRotation = new Vector3(0, -90, 0);
        private Vector3 closeRotation;

        [Header("Configuração de Movimento (Gaveta)")]
        public Vector3 openPositionOffset;
        private Vector3 closePosition;

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
            closeRotation = transform.localEulerAngles;
            closePosition = transform.localPosition;
            if (requiredItems == null || requiredItems.Count == 0)
            {
                isUnlocked = true;
            }
        }

        void Update()
        {
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

        void AnimatePosition()
        {
            if (open)
            {
                var targetPosition = closePosition + openPositionOffset;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 5 * smooth);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, closePosition, Time.deltaTime * 5 * smooth);
            }
        }

        public void OpenDoor()
        {
            if (GameProgression.instance.Progresso < requiredProgress)
            {
                if (GameProgression.instance.Progresso < 4 && GameProgression.instance.Progresso > 2 && id == 1)
                {
                    GameProgression.instance.AvancarProgresso();
                }
                else
                {
                    feedbackText.text = "Ainda preciso terminar minnha tarefa antes de abrir esta porta.";
                    Invoke("Limparfala", 3f);
                }
                return;
            }
            if (GameProgression.instance.Progresso < 9 && GameProgression.instance.Progresso > 7 && id == 4){
                GameProgression.instance.AvancarProgresso();
            }
            {
                GameProgression.instance.AvancarProgresso();
            }
            
            if (GameProgression.instance.Progresso < 8 && GameProgression.instance.Progresso > 5 && id == 3)
            {
                GameProgression.instance.AvancarProgresso();
            }

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
