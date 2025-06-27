using UnityEngine;
// É importante ter o namespace do seu script Openable aqui.
using DoorScript;

public class Movimento : MonoBehaviour
{
    private Vector3 entradaJogador;
    private Vector3 direcaoMovimento;
    private CharacterController characterController;
    private float velocidadeJogador = 2f; // Velocidade de movimento do jogador
    private Transform mycamera; // Referência à transform da câmera principal

    private float gravidade = -9.81f; // Valor da gravidade
    private float velocidadeVertical = 0f; // Velocidade atual de queda ou subida

    // Referência ao componente de textmeshpro para feedback ao jogador
    public TMPro.TextMeshProUGUI feedbackText;

    // Distância máxima para interagir com objetos como a porta
    public float distanciaInteracao = 2f;
    public bool pausarJogo = false; // Variável para controlar se o jogo está pausado

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mycamera = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Se o jogo está pausado, não executa nenhuma lógica de movimento ou interação.
        if (pausarJogo) return;

        // --- ROTAÇÃO DO PERSONAGEM ---
        transform.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);

        // --- INPUT DE MOVIMENTO ---
        entradaJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradaJogador = transform.TransformDirection(entradaJogador);

        // --- GRAVIDADE ---
        if (characterController.isGrounded)
        {
            velocidadeVertical = -1f;
        }
        else
        {
            velocidadeVertical += gravidade * Time.deltaTime;
        }

        // --- APLICAÇÃO DO MOVIMENTO ---
        direcaoMovimento = entradaJogador * velocidadeJogador;
        direcaoMovimento.y = velocidadeVertical;

        if (characterController.enabled)
        {
            characterController.Move(direcaoMovimento * Time.deltaTime);
        }

        // --- LÓGICA DE INTERAÇÃO ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            TentarAbrirPorta();
            TentarColetar();
        }

        // --- LÓGICA DE PAUSA ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Método para pausar/despausar o jogo
    void TogglePause()
    {
        pausarJogo = !pausarJogo; // Inverte o estado de pausa

        if (pausarJogo)
        {
            // Pausa o jogo
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            // Retoma o jogo
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    // Método para tentar abrir/interagir com um objeto "Openable"
    void TentarAbrirPorta()
    {
        RaycastHit hit;
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, distanciaInteracao))
        {
            Debug.Log("Raycast atingiu: " + hit.collider.name);
            
            // --- MUDANÇA AQUI ---
            // Tenta pegar o componente 'Openable' em vez de 'Door'.
            Openable porta = hit.collider.GetComponent<Openable>();

            if (porta != null)
            {
                // Chama o método público 'OpenDoor()' do script Openable.
                porta.OpenDoor();
            }
        }
    }

    // O método TentarColetar não precisa de mudanças.
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
}