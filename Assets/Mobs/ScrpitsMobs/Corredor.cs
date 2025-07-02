using UnityEngine;

public class Corredor : MonoBehaviour
{
    [Header("Aura de Insanidade")]
    [Tooltip("A que distância o jogador começa a perder sanidade.")]
    public float drainRadius = 10f;

    [Tooltip("A perda MÁXIMA de sanidade por segundo (quando o jogador está em cima do mob).")]
    public float maxSanityDrainPerSecond = 5f;

    private Transform playerTransform;

    void Start()
    {
        // Encontra o jogador pela tag "Player" para calcular a distância.
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("O objeto do Jogador com a tag 'Player' não foi encontrado. A aura de insanidade não funcionará.", this);
            enabled = false; // Desativa o script se não encontrar o jogador.
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calcula a distância entre o mob e o jogador.
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Se o jogador estiver dentro do raio da aura...
        if (distance <= drainRadius)
        {
            // Calcula a intensidade do dreno (de 0.0 a 1.0).
            // 1.0 = muito perto, 0.0 = na borda do raio.
            float drainIntensity = 1.0f - (distance / drainRadius);

            // Calcula a quantidade de sanidade a ser perdida neste frame.
            float sanityToDrain = maxSanityDrainPerSecond * drainIntensity * Time.deltaTime;

            // Aplica a perda de sanidade.
            PlayerSanity.Instance.ChangeSanity(-sanityToDrain);

            // Debug para ver a intensidade do dreno em tempo real.
            Debug.Log("Jogador está na aura do Corredor. Intensidade do dreno: " + drainIntensity.ToString("P0")); // "P0" formata como porcentagem
        }
    }

    // Desenha a aura de insanidade no editor para facilitar o ajuste.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f); // Vermelho semitransparente
        Gizmos.DrawSphere(transform.position, drainRadius);
    }
}