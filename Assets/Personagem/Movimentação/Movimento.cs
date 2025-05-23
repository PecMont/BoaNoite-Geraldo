using UnityEngine;

public class Movimento : MonoBehaviour
{
    private Vector3 entradaJogador;
    private Vector3 direcaoMovimento;
    private CharacterController characterController;
    private float velocidadeJogador = 1f;
    private Transform mycamera;

    private float gravidade = -9.81f;
    private float velocidadeVertical = 0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mycamera = Camera.main.transform;
    }

    void Update()
    {
        // Rotaciona o personagem baseado na câmera
        transform.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);

        // Pega input do jogador
        entradaJogador = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        entradaJogador = transform.TransformDirection(entradaJogador);

        // Aplica gravidade
        if (characterController.isGrounded)
        {
            velocidadeVertical = 0f; // Zera a velocidade vertical ao tocar o chão

            // Opcional: pulo
            // if (Input.GetButtonDown("Jump"))
            // {
            //     velocidadeVertical = Mathf.Sqrt(2f * alturaPulo * -gravidade);
            // }
        }
        else
        {
            velocidadeVertical += gravidade * Time.deltaTime;
        }

        // Combina movimento horizontal e vertical
        direcaoMovimento = entradaJogador * velocidadeJogador;
        direcaoMovimento.y = velocidadeVertical;

        // Move o personagem
        characterController.Move(direcaoMovimento * Time.deltaTime);
    }
}
