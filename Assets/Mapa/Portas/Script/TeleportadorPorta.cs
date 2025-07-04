using UnityEngine;
using System.Collections;
using DoorScript;

public class TeleportadorPorta : MonoBehaviour
{
    [Header("Configurações do Teleporte")]
    public Transform pontoDeSaida;
    public float atrasoAntesDoTeleporte = 1.0f;
    public string tagDoJogador = "Player";

    [Header("Portas Físicas Associadas")]
    public Openable portaFisicaEntrada;
    public Openable portaFisicaSaida;

    [Header("Configurações de Olhar na Saída (Opcional)")]
    public Transform olharParaEsteAlvoNaSaida;

    private bool teletransportando = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDoJogador) && !teletransportando && pontoDeSaida != null)
        {
            StartCoroutine(ProcessoDeTeleporte(other.gameObject));
        }
    }

    IEnumerator ProcessoDeTeleporte(GameObject jogador)
    {
        teletransportando = true;

        if (portaFisicaEntrada != null)
        {
            if (portaFisicaEntrada.open)
            {
                portaFisicaEntrada.OpenDoor();
                Debug.Log("Fechando porta de entrada: " + portaFisicaEntrada.gameObject.name);
            }
        }

        CharacterController cc = jogador.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        Debug.Log(jogador.name + " entrou no trigger. Aguardando " + atrasoAntesDoTeleporte + " segundos...");
        yield return new WaitForSeconds(atrasoAntesDoTeleporte);

        Debug.Log("Teletransportando " + jogador.name + " para " + pontoDeSaida.name);

        jogador.transform.position = pontoDeSaida.position;

        if (olharParaEsteAlvoNaSaida != null)
        {
            Debug.Log("Alvo para olhar está DEFINIDO: " + olharParaEsteAlvoNaSaida.name + ". Calculando direção...");
            Vector3 direcaoParaOlhar = olharParaEsteAlvoNaSaida.position - jogador.transform.position;
            Debug.Log("Direção crua para o alvo: " + direcaoParaOlhar);
            direcaoParaOlhar.y = 0;
            Debug.Log("Direção para o alvo (Y zerado): " + direcaoParaOlhar);

            if (direcaoParaOlhar != Vector3.zero)
            {
                jogador.transform.rotation = Quaternion.LookRotation(direcaoParaOlhar.normalized);
                Debug.Log(jogador.name + " AGORA ESTÁ OLHANDO PARA " + olharParaEsteAlvoNaSaida.name);
            }
            else
            {
                Debug.LogWarning("DIREÇÃO PARA OLHAR É ZERO. Usando rotação do ponto de saída.");
                jogador.transform.rotation = pontoDeSaida.rotation;
            }
        }
        else
        {
            Debug.Log("Nenhum alvo para olhar definido. Usando rotação do ponto de saída.");
            jogador.transform.rotation = pontoDeSaida.rotation;
        }

        if (cc != null)
        {
            cc.enabled = true;
        }

        if (portaFisicaSaida != null)
        {
            if (!portaFisicaSaida.open)
            {
                portaFisicaSaida.OpenDoor();
                Debug.Log("Abrindo porta de saída: " + portaFisicaSaida.gameObject.name);
            }
        }

        yield return new WaitForSeconds(0.1f);
        teletransportando = false;
    }

    void OnDrawGizmosSelected()
    {
        if (pontoDeSaida != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pontoDeSaida.position, 0.5f);
            Gizmos.DrawLine(transform.position, pontoDeSaida.position);
            Gizmos.DrawLine(pontoDeSaida.position, pontoDeSaida.position + pontoDeSaida.forward * 1f);
        }

        if (olharParaEsteAlvoNaSaida != null && pontoDeSaida != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pontoDeSaida.position, olharParaEsteAlvoNaSaida.position);
            Gizmos.DrawWireSphere(olharParaEsteAlvoNaSaida.position, 0.3f);
        }
    }
}
