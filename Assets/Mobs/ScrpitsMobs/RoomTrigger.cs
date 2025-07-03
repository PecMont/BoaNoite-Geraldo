// Nome do arquivo: RoomTrigger.cs
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Crianca crianca; // Arraste o GameObject da Criança aqui no Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            crianca.OnPlayerEnterRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            crianca.OnPlayerExitRoom();
        }
    }
}