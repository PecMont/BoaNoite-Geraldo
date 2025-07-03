using UnityEngine;
using System; // Necessário para 'Action'

public class PlayerSanity : MonoBehaviour
{
    // Singleton para fácil acesso de outros scripts
    public static PlayerSanity Instance { get; private set; }

    [Header("Configurações de Sanidade")]
    public float maxSanity = 100f;
    private float _currentSanity;

    [Header("UI de Fim de Jogo")]
    public GameObject gameOverPanel; 

    [Header("Dreno de Sanidade com o Tempo")]
    [Tooltip("A quantidade de sanidade que o jogador perde por segundo.")]
    public float sanityDrainPerSecond = 5.0f; // Exemplo: perde 0.5 de sanidade a cada segundo

    [Tooltip("Permite ligar ou desligar o dreno de sanidade (ex: para áreas seguras).")]
    public bool isDraining = true;

    // Propriedade pública para acessar a sanidade atual com segurança
    public float CurrentSanity
    {
        get { return _currentSanity; }
        private set { _currentSanity = Mathf.Clamp(value, 0f, maxSanity); } // Garante que a sanidade fique entre 0 e maxSanity
    }

    // Evento para notificar outros sistemas (UI, Efeitos) quando a sanidade muda
    // Ele enviará o valor atual e o máximo, úteis para a barra de UI.
    public static event Action<float, float> OnSanityChanged;

    void Awake()
    {
        // Configuração do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Start()
    {
        // Começa o jogo com a sanidade no máximo
        CurrentSanity = maxSanity;
        // --- LINHA A SER ADICIONADA ---
        // Notifica todos os 'ouvintes' (como a UI e os efeitos) sobre o valor inicial.
        OnSanityChanged?.Invoke(CurrentSanity, maxSanity);
    }

    void Update()
    {
        // Se a mecânica de dreno estiver ativa e o jogador ainda tiver sanidade para perder...
        if (isDraining && CurrentSanity > 0)
        {
            // Chamamos nosso método ChangeSanity, passando um valor negativo.
            // Multiplicamos pela taxa e por Time.deltaTime para que a perda seja constante por segundo,
            // e não dependente da taxa de frames (FPS) do computador.
            ChangeSanity(-sanityDrainPerSecond * Time.deltaTime);
        }
    }

    /// <summary>
    /// Modifica a sanidade atual. Use um valor positivo para ganhar e um negativo para perder.
    /// </summary>
    /// <param name="amount">A quantidade a ser alterada.</param>
    public void ChangeSanity(float amount)
    {
        CurrentSanity += amount;
        //Debug.Log($"Sanidade alterada em {amount}. Sanidade atual: {CurrentSanity}/{maxSanity}");

        // Dispara o evento para que a UI e os efeitos possam reagir
        OnSanityChanged?.Invoke(CurrentSanity, maxSanity);

        if (CurrentSanity <= Mathf.Epsilon)
        {
            // Garante que o método não seja chamado múltiplas vezes se a sanidade continuar diminuindo
            if (this.enabled) 
            {
                Morrer();
            }
        }
    }

     private void Morrer()
    {
        Debug.Log("A sanidade chegou a zero! Fim de jogo.");
        this.enabled = false;

        // Mostra o cursor e o destrava
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pausa o jogo atrás da tela preta
        Time.timeScale = 0f;

        // ATIVA a tela de Game Over em vez de carregar uma nova cena
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("A referência para 'gameOverPanel' não foi configurada no Inspector do PlayerSanity!");
        }
    }
}
