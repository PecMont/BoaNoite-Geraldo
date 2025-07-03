using UnityEngine;

// Esse script vai trocar o sprite do personagem Geraldo
// para um sprite de emoção específica, como "feliz", "triste", etc.
// são tres texturas diferentes que o gerando tem(cabeça, tronco e pernas),
// e cada uma delas tem um sprite diferente para cada emoção.
public class GeraldoEmo : MonoBehaviour 
{   
    [Header("Configurações do Geraldo")]
    [Tooltip("Sprite do Geraldo quando está normal.")]
    public Material cabeçaNormal;
    public Material troncoNormal;
    public Material pernasNormal;
    [Tooltip("Sprite do Geraldo na fase Emo.")]
    public Material cabeçaEmo;
    public Material troncoEmo;
    public Material pernasEmo;

    [Header("Onde será aplicado o sprite?")]
    // os tres objetos que vão receber o sprite do Geraldo
    public GameObject cabeçaGeraldo;
    public GameObject troncoGeraldo;
    public GameObject pernasGeraldo;

    [Header("Configurações do Sensor")]
    [Tooltip("A que distância o sensor detecta o jogador.")]
    public float detectionRadius = 1f;
    [Tooltip("Para qual criatura este sensor está vinculado.")]
    public GameObject linkedCreature;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
// verifica se o jogador está dentro do raio de detecção
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= detectionRadius)
            {
                Invoke("TrocarTexturaEmo", 3f); // Troca a textura do Geraldo para Emo
            }
        }
        else
        {
            TrocarTexturaNormal();
        }
    }

    void trocarTexturaGeraldo(Material cabeça, Material tronco, Material pernas)
    {
        // Troca o sprite do Geraldo
        cabeçaGeraldo.GetComponent<Renderer>().material = cabeça;
        troncoGeraldo.GetComponent<Renderer>().material = tronco;
        pernasGeraldo.GetComponent<Renderer>().material = pernas;
    }
    void TrocarTexturaEmo()
    {
        if(GameProgression.instance.Progresso < 5 && GameProgression.instance.Progresso < 7) GameProgression.instance.Progresso++; // Aumenta o progresso do jogador
        trocarTexturaGeraldo(cabeçaEmo, troncoEmo, pernasEmo);
        Invoke("TrocarTexturaNormal", 5f); // Troca a textura do Geraldo de volta para normal após 5 segundos
    }
    void TrocarTexturaNormal()
    {
        if(GameProgression.instance.Progresso < 6 && GameProgression.instance.Progresso < 8) GameProgression.instance.Progresso++; // Aumenta o progresso do jogador
        trocarTexturaGeraldo(cabeçaNormal, troncoNormal, pernasNormal);
    }
}

