using UnityEngine;

public class SensorCorredor : MonoBehaviour
{
    [Header("Configurações do Sensor")]
    [Tooltip("A que distância o sensor detecta o jogador.")]
    public float detectionRadius = 1f;

    // --- MELHORIA 1: Variáveis para otimização ---
    private Transform playerTransform; // Guarda a referência do jogador para não precisar procurar a cada frame.
    private bool isTriggered = false;  // Evita que o sensor seja acionado múltiplas vezes.

    void Start()
    {

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
            if(GameProgression.instance.Progresso < 10 && GameProgression.instance.Progresso > 8)
            {
                GameProgression.instance.AvancarProgresso();
                Destroy(gameObject); // Destrói o sensor após ativar a criatura.

            }
        }
    }

}
