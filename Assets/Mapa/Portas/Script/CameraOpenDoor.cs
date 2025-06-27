using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoorScript; // Importa o namespace necessário

namespace CameraDoorScript
{
    public class CameraOpenDoor : MonoBehaviour 
    {
        public float DistanceOpen = 3;
        public GameObject text;

        void Start () 
        {
            if (text != null)
            {
                text.SetActive(false);
            }
        }
        
        void Update () 
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, DistanceOpen)) 
            {
                // --- MUDANÇA AQUI ---
                // Verifica se o objeto atingido tem o componente Openable
                if (hit.transform.GetComponent<Openable>()) 
                {
                    if (text != null) text.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // --- MUDANÇA AQUI ---
                        // Chama o método OpenDoor() do componente Openable
                        hit.transform.GetComponent<Openable>().OpenDoor();
                    }
                }
                else
                {
                    if (text != null) text.SetActive(false);
                }
            }
            else
            {
                if (text != null) text.SetActive(false);
            }
        }
    }
}