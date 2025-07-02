using UnityEngine;

public class MulherPia : MonoBehaviour
{
    [Header("Insanidade")]
    public float insanidadePerda = 10f;
    public float tempoOlhar = 3f;
    public float raioDreno = 10f;

    [Header("Ponto de Foco")]
    [Tooltip("Arraste o objeto filho (o 'ponto') que será o alvo do olhar do jogador.")]
    public Transform lookTargetPoint;

    [Header("Configuração de Colisão")]
    [Tooltip("Defina quais layers o raio de visão deve colidir. Ignore a layer 'Player'.")]
    public LayerMask collisionMask;

    private float tempoOlhando = 0f;
    private Camera mainCamera;
    private Renderer mobRenderer;
    private Plane[] cameraFrustum;

    void Start()
    {
        mainCamera = Camera.main;
        mobRenderer = GetComponentInChildren<Renderer>();
        if (mobRenderer == null)
        {
            Debug.LogError("O mob MulherPia precisa de um componente Renderer para funcionar.", this);
            enabled = false;
        }

        if (lookTargetPoint == null)
        {
            Debug.LogError("O 'lookTargetPoint' não foi definido no Inspector! Arraste o objeto 'ponto' para o campo.", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (EstaSendoOlhada())
        {
            tempoOlhando += Time.deltaTime;
            if (tempoOlhando >= tempoOlhar)
            {
                PlayerSanity.Instance.ChangeSanity(-insanidadePerda);
                tempoOlhando = 0f;
                Debug.LogWarning("--- SANIDADE DIMINUIU! ---");
            }
        }
        else
        {
            tempoOlhando = 0f;
        }
    }

    bool EstaSendoOlhada()
    {
        // PASSO 1: Checa se o MOB INTEIRO está na tela
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        if (!GeometryUtility.TestPlanesAABB(cameraFrustum, mobRenderer.bounds))
        {
            Debug.Log("DEBUG: Mob (MulherPia) está fora do campo de visão da câmera.");
            return false;
        }

        // PASSO 2: Mira no PONTO ESPECÍFICO
        Vector3 targetPoint = lookTargetPoint.position;

        // PASSO 3: Checa a distância até o PONTO
        float distancia = Vector3.Distance(mainCamera.transform.position, targetPoint);
        if (distancia > raioDreno)
        {
            Debug.Log("DEBUG: Jogador está muito longe do ponto de foco. Distância: " + distancia + " / Raio Máximo: " + raioDreno);
            return false;
        }

        // PASSO 4: Raycast em direção ao PONTO
        Debug.DrawRay(mainCamera.transform.position, (targetPoint - mainCamera.transform.position).normalized * raioDreno, Color.magenta);
        Vector3 dir = (targetPoint - mainCamera.transform.position).normalized;
        Ray ray = new Ray(mainCamera.transform.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raioDreno + 1f, collisionMask))
        {
            if (hit.transform == lookTargetPoint || hit.transform.IsChildOf(transform))
            {
                Debug.Log("DEBUG: SUCESSO! Raycast acertou o ponto de foco.");
                return true;
            }
            else
            {
                Debug.Log("DEBUG: Raycast acertou um obstáculo: '" + hit.transform.name + "' na layer: '" + LayerMask.LayerToName(hit.transform.gameObject.layer) + "'");
            }
        }
        else
        {
            Debug.Log("DEBUG: Raycast não acertou NADA. Verifique se a layer do mob está incluída na Collision Mask.");
        }
        
        return false;
    }
}