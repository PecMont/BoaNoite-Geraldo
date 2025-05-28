using UnityEngine;
using System.Collections;
// Se o seu script Door.cs está dentro de um namespace chamado "DoorScript",
// esta linha é importante. Caso contrário, você pode removê-la.
using DoorScript;

public class TeleportadorPorta : MonoBehaviour
{
    [Header("Configurações do Teleporte")]
    public Transform pontoDeSaida;
    public float atrasoAntesDoTeleporte = 1.0f;
    public string tagDoJogador = "Player";

    [Header("Portas Físicas Associadas")]
    public Door portaFisicaEntrada;
    public Door portaFisicaSaida;

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
        // Removido o else que continha LogWarning para simplificar,
        // mas você pode adicionar de volta se quiser saber quando não está configurado.

        CharacterController cc = jogador.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        Debug.Log(jogador.name + " entrou no trigger. Aguardando " + atrasoAntesDoTeleporte + " segundos...");
        yield return new WaitForSeconds(atrasoAntesDoTeleporte);

        Debug.Log("Teletransportando " + jogador.name + " para " + pontoDeSaida.name);

        jogador.transform.position = pontoDeSaida.position;

        // --- LÓGICA DE ROTAÇÃO COM MAIS DEBUG ---
        if (olharParaEsteAlvoNaSaida != null)
        {
            // DEBUG ADICIONADO: Confirma que o alvo está atribuído e qual é.
            Debug.Log("Alvo para olhar está DEFINIDO: " + olharParaEsteAlvoNaSaida.name + ". Calculando direção...");

            Vector3 direcaoParaOlhar = olharParaEsteAlvoNaSaida.position - jogador.transform.position;
            // DEBUG ADICIONADO: Mostra a direção calculada antes de zerar o Y.
            Debug.Log("Direção crua para o alvo: " + direcaoParaOlhar);

            direcaoParaOlhar.y = 0; // Mantém o personagem "reto" no eixo Y

            // DEBUG ADICIONADO: Mostra a direção após zerar o Y.
            Debug.Log("Direção para o alvo (Y zerado): " + direcaoParaOlhar);

            if (direcaoParaOlhar != Vector3.zero)
            {
                jogador.transform.rotation = Quaternion.LookRotation(direcaoParaOlhar.normalized);
                Debug.Log(jogador.name + " AGORA ESTÁ OLHANDO PARA " + olharParaEsteAlvoNaSaida.name);
            }
            else
            {
                Debug.LogWarning("DIREÇÃO PARA OLHAR É ZERO. Alvo para olhar está muito próximo ou na mesma posição X,Z do jogador. Usando rotação do ponto de saída.");
                jogador.transform.rotation = pontoDeSaida.rotation;
            }
        }
        else
        {
            Debug.Log("Nenhum alvo para olhar definido (olharParaEsteAlvoNaSaida é NULO). Usando rotação do ponto de saída.");
            jogador.transform.rotation = pontoDeSaida.rotation;
        }
        // --- FIM DA LÓGICA DE ROTAÇÃO ---

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
        // Removido o else que continha LogWarning para simplificar.

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
            Gizmos.DrawLine(pontoDeSaida.position, olharParaEsteAlvoNaSaida.position); // Linha do ponto de saída do jogador até o alvo
            Gizmos.DrawWireSphere(olharParaEsteAlvoNaSaida.position, 0.3f);
        }
    }
}