using UnityEngine;

public class GeraldoEmo : MonoBehaviour
{
    [Header("Partes do Corpo do Geraldo")]
    public GameObject cabeçaGeraldo;
    public GameObject troncoGeraldo;
    public GameObject pernasGeraldo;

    [Header("Materiais Normais")]
    public Material cabeçaNormal;
    public Material troncoNormal;
    public Material pernasNormal;

    [Header("Materiais Emo")]
    public Material cabeçaEmo;
    public Material troncoEmo;
    public Material pernasEmo;

    [Header("Configurações do Sensor")]
    [Tooltip("A que distância o sensor detecta o jogador.")]
    public float detectionRadius = 1f;

    // --- MELHORIA 1: Variáveis para otimização ---
    private Transform playerTransform; // Para não procurar o jogador a cada frame
    private bool hasBeenTriggered = false; // Para garantir que a troca de textura aconteça só uma vez

    void Start()
    {
        // Procura o jogador APENAS UMA VEZ
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Jogador com a tag 'Player' não encontrado!", this);
            enabled = false; // Desativa o script se não encontrar o jogador
        }
    }

    void Update()
    {
        // Se o jogador não existe ou se o evento já foi disparado, não faz nada.
        if (playerTransform == null || hasBeenTriggered)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            // Marca que o evento foi disparado para não acontecer de novo
            hasBeenTriggered = true;
            Debug.Log("Jogador detectado! Geraldo vai entrar na fase Emo em 3 segundos.");
            Invoke("TrocarTexturaEmo", 3f);
        }
    }

    // Função genérica para trocar as texturas/materiais
    void AplicarMateriais(Material cabeça, Material tronco, Material pernas)
    {
        // É uma boa prática verificar se as partes do corpo foram atribuídas antes de usá-las
        if (cabeçaGeraldo != null) cabeçaGeraldo.GetComponent<Renderer>().material = cabeça;
        if (troncoGeraldo != null) troncoGeraldo.GetComponent<Renderer>().material = tronco;
        if (pernasGeraldo != null) pernasGeraldo.GetComponent<Renderer>().material = pernas;
    }

    void TrocarTexturaEmo()
    {
        Debug.Log("Geraldo agora está Emo. O progresso será verificado.");
        
        // --- MUDANÇA 1 ---
        // A condição "Progresso < 5 && Progresso < 7" é o mesmo que "Progresso < 5".
        if (GameProgression.instance.Progresso < 5)
        {
            // ANTES: GameProgression.instance.Progresso++;
            GameProgression.instance.AvancarProgresso(); // CORRIGIDO
        }
        
        AplicarMateriais(cabeçaEmo, troncoEmo, pernasEmo);
        
        // Agenda a volta ao normal após 5 segundos
        Invoke("TrocarTexturaNormal", 5f);
    }

    void TrocarTexturaNormal()
    {
        Debug.Log("Geraldo voltou ao normal. O progresso será verificado.");

        // --- MUDANÇA 2 ---
        // A condição "Progresso < 6 && Progresso < 8" é o mesmo que "Progresso < 6".
        if (GameProgression.instance.Progresso < 6)
        {
            // ANTES: GameProgression.instance.Progresso++;
            GameProgression.instance.AvancarProgresso(); // CORRIGIDO
        }
        
        AplicarMateriais(cabeçaNormal, troncoNormal, pernasNormal);
    }
}