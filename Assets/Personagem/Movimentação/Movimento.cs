using UnityEngine;

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

    // Variável para controlar a altura do pulo (se você decidir implementar o pulo)
    // public float alturaPulo = 1.5f;

    void Awake()
    {
        // Pega o componente CharacterController anexado a este GameObject
        characterController = GetComponent<CharacterController>();
        // Pega a transform da câmera principal da cena
        mycamera = Camera.main.transform;

        // Trava o cursor no centro da tela e o torna invisível (comum em jogos FPS/TPS)
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- ROTAÇÃO DO PERSONAGEM ---
        // Faz o personagem olhar na mesma direção horizontal que a câmera
        transform.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);

        // --- INPUT DE MOVIMENTO ---
        // Pega os inputs dos eixos Horizontal (A/D, Setas Esq/Dir) e Vertical (W/S, Setas Cima/Baixo)
        entradaJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // Converte o vetor de entrada do espaço global para o espaço local do personagem,
        // para que "frente" seja sempre para onde o personagem está olhando.
        entradaJogador = transform.TransformDirection(entradaJogador);

        // --- GRAVIDADE E PULO ---
        if (characterController.isGrounded) // Se o personagem está no chão
        {
            velocidadeVertical = -1f; // Aplica uma pequena força para baixo para manter o personagem no chão
        }
        else // Se o personagem está no ar
        {
            // Aplica a gravidade continuamente
            velocidadeVertical += gravidade * Time.deltaTime;
        }

        // --- APLICAÇÃO DO MOVIMENTO ---
        // Combina o movimento horizontal (controlado pelo jogador) com o movimento vertical (gravidade/pulo)
        direcaoMovimento = entradaJogador * velocidadeJogador;
        direcaoMovimento.y = velocidadeVertical;

        // Move o personagem usando o CharacterController.
        // Multiplica por Time.deltaTime para tornar o movimento independente da taxa de quadros (FPS).
        // CORREÇÃO APLICADA AQUI:
        if (characterController.enabled) // Só chama Move() se o controller estiver habilitado
        {
            characterController.Move(direcaoMovimento * Time.deltaTime);
        }

        // --- LÓGICA DE INTERAÇÃO COM A PORTA ---
        // Verifica se a tecla 'E' foi pressionada neste frame
        if (Input.GetKeyDown(KeyCode.E))
        {
            TentarAbrirPorta();
            TentarColetar();
        }
        // Veriifica se a tecla 'escape' foi pressionada neste frame
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausarJogo)
            {
                // Se o jogo já está pausado, retoma o jogo
                pausarJogo = false;
                // Destrava o cursor
                Cursor.lockState = CursorLockMode.Locked;
                // Torna o cursor invisível
                Cursor.visible = false;
                // Retoma o jogo
                Time.timeScale = 1f; // Retoma o jogo
            }
            else
            {
                // Se o jogo não está pausado, pausa o jogo
                pausarJogo = true;
                Cursor.lockState = CursorLockMode.None; // Destrava o cursor
                Cursor.visible = true; // Torna o cursor visível
                Time.timeScale = 0f; // Pausa o jogos
            }
        } 
    }

    // Método para tentar abrir/interagir com uma porta
    void TentarAbrirPorta()
    {
        RaycastHit hit; // Variável para armazenar informações sobre o que o raio atingiu

        // Dispara um raio da posição da câmera, para frente, até a distância de interação.
        // Isso simula o jogador "olhando" para algo para interagir.
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, distanciaInteracao))
        {
            // Se o raio atingiu alguma coisa, imprime no console o nome do objeto (para depuração)
            Debug.Log("Raycast atingiu: " + hit.collider.name);

            // Tenta pegar o componente 'Door' do objeto que o raio atingiu.
            // É importante usar 'DoorScript.Door' se o seu script 'Door' estiver dentro do namespace 'DoorScript'.
            DoorScript.Door porta = hit.collider.GetComponent<DoorScript.Door>();

            if (porta != null) // Se o objeto atingido tem o script 'Door'
            {
                // Chama o método público 'OpenDoor()' do script da porta
                porta.OpenDoor();
            }
        }
    }
    void TentarColetar()
    {
        RaycastHit hit; // Variável para armazenar informações sobre o que o raio atingiu

        // Dispara um raio da posição da câmera, para frente, até a distância de interação.
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, distanciaInteracao))
        {
            // Se o raio atingiu alguma coisa, imprime no console o nome do objeto (para depuração)
            Debug.Log("Raycast atingiu: " + hit.collider.name);

            // Tenta pegar o componente 'CollectibleItem' do objeto que o raio atingiu.
            CollectibleItem coleta = hit.collider.GetComponent<CollectibleItem>();
            NoCollectibleItem usar = hit.collider.GetComponent<NoCollectibleItem>();

            if (coleta != null) // Se o objeto atingido tem o script 'CollectibleItem'
            {   
                LimparFeedback(); // Limpa o feedback anterior
                feedbackText.text = "Item coletado: " + coleta.itemData.itemName; // Atualiza o feedback ao jogador
                // limpa o texto após 1 segundos
                Invoke("LimparFeedback", 1f);
                
                coleta.Collect();
            }
            else if (usar != null) // Se o objeto atingido tem o script 'NoCollectibleItem'
            {
                LimparFeedback(); // Limpa o feedback anterior
                string descricao = usar.Use(); // Chama o método Use() do script NoCollectibleItem
                feedbackText.text = descricao; // Atualiza o feedback ao jogador
                feedbackText.color = Color.black; // Define a cor do texto como branco
                feedbackText.fontSize = 50; // Define o tamanho da fonte do texto
                feedbackText.alignment = TMPro.TextAlignmentOptions.Justified; 
                Invoke("LimparFeedback", 3f); // Limpa o feedback após 1 segundo
            }
        }
    }
    void LimparFeedback()
    {
        feedbackText.text = ""; // Limpa o texto de feedback
        feedbackText.color = Color.white; // Restaura a cor do texto para branco
        feedbackText.fontSize = 30; // Restaura o tamanho da fonte para o
        feedbackText.alignment = TMPro.TextAlignmentOptions.Center; // Restaura o alinhamento do texto para centralizado
    }
}