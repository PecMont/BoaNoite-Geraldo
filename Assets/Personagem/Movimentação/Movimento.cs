using UnityEngine;
using DoorScript;

public class Movimento : MonoBehaviour
{
    private Vector3 entradaJogador;
    private Vector3 direcaoMovimento;
    private CharacterController characterController;
    public float velocidadeJogador = 2f;
    private Transform mycamera;

    private float gravidade = -9.81f;
    private float velocidadeVertical = 0f;

    private Animator animator;

    public TMPro.TextMeshProUGUI feedbackText;
    public float distanciaInteracao = 2f;
    public bool pausarJogo = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mycamera = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);

        entradaJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (entradaJogador.magnitude > 1)
        {
            entradaJogador.Normalize();
        }

        animator.SetFloat("Speed", entradaJogador.magnitude);

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

        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.SetTrigger("Dance");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TentarAbrirPorta();
            TentarColetar();
            TentarDormir();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if(Input.GetKeyDown(KeyCode.L)){
            GameProgression.instance.Progresso++;
        }
    }

    #region Funções existentes
    void TogglePause()
    {
        pausarJogo = !pausarJogo;

        if (pausarJogo)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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

    void TentarDormir()
    {
        RaycastHit hit;
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, distanciaInteracao))
        {
            Debug.Log("Raycast atingiu: " + hit.collider.name);
            Dormir dormir = hit.collider.GetComponent<Dormir>();
            if (dormir != null)
            {
                Debug.Log("Tentando dormir...");
                if(GameProgression.instance.Progresso == 7 ){
                    GameProgression.instance.AvancarProgresso();
                }
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
