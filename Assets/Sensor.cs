using UnityEngine;

public class Sensor : MonoBehaviour
{
    [Header("Configurações do Sensor")]
    [Tooltip("A que distância o sensor detecta o jogador.")]
    public float detectionRadius = 1f;
    [Tooltip("Para qual criatura este sensor está vinculado.")]
    public GameObject linkedCreature;

    // --- MELHORIA 1: Variáveis para otimização ---
    private Transform playerTransform; // Guarda a referência do jogador para não precisar procurar a cada frame.
    private bool isTriggered = false;  // Evita que o sensor seja acionado múltiplas vezes.

    void Start()
    {
        if (linkedCreature == null)
        {
            Debug.LogError("O sensor precisa estar vinculado a uma criatura. Por favor, arraste o GameObject da criatura no campo 'Linked Creature' no Inspector.", this);
            enabled = false;
            return;
        }

        // Procura o jogador APENAS UMA VEZ no início e guarda sua posição.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Não foi possível encontrar o objeto do Jogador (Player). Verifique se ele tem a tag 'Player'.", this);
            enabled = false;
        }
    }

    void Update()
    {
        // Se o jogador não foi encontrado ou o sensor já foi acionado, não faz mais nada.
        if (playerTransform == null || isTriggered)
        {
            return;
        }

        // Verifica a distância usando a referência guardada (muito mais rápido).
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            // O jogador está dentro do raio.
            isTriggered = true; // Marca como acionado para não repetir.
            linkedCreature.SetActive(true);
            Invoke("DeactivateLinkedCreature", 1f);
        }
    }

    public void DeactivateLinkedCreature()
    {
        if (linkedCreature != null)
        {
            // Apaga todos os outros sensores vinculados à mesma criatura.
            foreach (var obj in FindObjectsByType<Sensor>(FindObjectsSortMode.None))
            {
                if (obj.linkedCreature != null && obj.linkedCreature == linkedCreature)
                {
                    Destroy(obj.gameObject);
                }
            }

            // --- ALTERAÇÃO PRINCIPAL (O que você pediu) ---
            // Verifica se o progresso é menor que 1 e avança usando o método correto.
            if (GameProgression.instance.Progresso < 1)
            {
                GameProgression.instance.AvancarProgresso();
            }

            linkedCreature.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Nenhuma criatura vinculada para desativar.", this);
        }
    }
}