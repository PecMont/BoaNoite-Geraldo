using UnityEngine;
using DoorScript;

public class Movimento : MonoBehaviour
{
    private Vector3 entradaJogador;
    private Vector3 direcaoMovimento;
    private CharacterController characterController;
    private float velocidadeJogador = 2f;
    private Transform mycamera;

    private float gravidade = -9.81f;
    private float velocidadeVertical = 0f;

    // --- Variável para o Animator ---
    private Animator animator; // <<< ADICIONAR

    public TMPro.TextMeshProUGUI feedbackText;
    public float distanciaInteracao = 2f;
    public bool pausarJogo = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mycamera = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        // --- Pegar o componente Animator do objeto filho ---
        // Usamos GetComponentInChildren porque o modelo 3D (com o Animator) é um filho.
        animator = GetComponentInChildren<Animator>(); // <<< ADICIONAR
    }

    void Update()
    {
        
        if (pausarJogo) return;

        transform.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);

        entradaJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // --- Normalizar a entrada para evitar movimento mais rápido na diagonal ---
        if (entradaJogador.magnitude > 1)
        {
            entradaJogador.Normalize();
        }
        
        // --- ATUALIZAR O ANIMATOR ---
        // Avisa o Animator qual a "velocidade" do jogador. Magnitude é 0 se parado, e 1 se movendo.
        animator.SetFloat("Speed", entradaJogador.magnitude); // <<< ADICIONAR

        entradaJogador = transform.TransformDirection(entradaJogador);

        if (characterController.isGrounded)
        {
            velocidadeVertical = -1f;
        }
        else
        {
            velocidadeVertical += gravidade * Time.deltaTime;
        }

        direcaoMovimento = entradaJogador * velocidadeJogador;
        direcaoMovimento.y = velocidadeVertical;

        if (characterController.enabled)
        {
            characterController.Move(direcaoMovimento * Time.deltaTime);
        }

        // --- LÓGICA DE ANIMAÇÃO DE DANÇA ---
        // Se a tecla 'P' (ou outra de sua escolha) for pressionada
        if (Input.GetKeyDown(KeyCode.P)) // <<< ADICIONAR (Pode trocar 'P' por outra tecla)
        {
            animator.SetTrigger("Dance"); // <<< ADICIONAR
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TentarAbrirPorta();
            TentarColetar();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    // O resto do seu código continua igual...
    // TogglePause(), TentarAbrirPorta(), TentarColetar(), LimparFeedback()
    // ...
    #region Funções existentes
    void TogglePause()
    {
        pausarJogo = !pausarJogo;

        if (pausarJogo)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    void TentarAbrirPorta()
    {
        RaycastHit hit;
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, distanciaInteracao))
        {
            Debug.Log("Raycast atingiu: " + hit.collider.name);
            Openable porta = hit.collider.GetComponent<Openable>();
            if (porta != null)
            {
                porta.OpenDoor();
            }
        }
    }

    void TentarColetar()
    {
        RaycastHit hit;
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, distanciaInteracao))
        {
            Debug.Log("Raycast atingiu: " + hit.collider.name);
            CollectibleItem coleta = hit.collider.GetComponent<CollectibleItem>();
            NoCollectibleItem usar = hit.collider.GetComponent<NoCollectibleItem>();

            if (coleta != null)
            {
                LimparFeedback();
                feedbackText.text = "Item coletado: " + coleta.itemData.itemName;
                Invoke("LimparFeedback", 1f);
                coleta.Collect();
            }
            else if (usar != null)
            {
                LimparFeedback();
                string descricao = usar.Use();
                feedbackText.text = descricao;
                feedbackText.color = Color.black;
                feedbackText.fontSize = 50;
                feedbackText.alignment = TMPro.TextAlignmentOptions.Justified;
                Invoke("LimparFeedback", 3f);
            }
        }
    }

    void LimparFeedback()
    {
        feedbackText.text = "";
        feedbackText.color = Color.white;
        feedbackText.fontSize = 30;
        feedbackText.alignment = TMPro.TextAlignmentOptions.Center;
    }
    #endregion
}