using System.Collections;
using System.Collections.Generic;
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

        // Use this for initialization
        void Start()
        {
            // Pega o componente AudioSource automaticamente se não foi arrastado no Inspector
            if (asource == null) 
            {
                asource = GetComponent<AudioSource>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (open)
            {
                // Se a porta deve estar aberta, rotaciona para o DoorOpenAngle
                var target = Quaternion.Euler(0, DoorOpenAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
            }
            else
            {
                // Se a porta deve estar fechada, rotaciona para o DoorCloseAngle
                var target1 = Quaternion.Euler(0, DoorCloseAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
            }
        }

        public void OpenDoor()
        {
            open = !open; // Inverte o estado da porta (aberta/fechada)
            
            // Seleciona o clipe de áudio baseado no novo estado da porta
            if (open)
            {
                asource.clip = openDoor;
            }
            else
            {
                asource.clip = closeDoor;
            }

            // Toca o som, se um clipe estiver definido
            if (asource.clip != null)
            {
                asource.Play();
            }
        }
    }
}