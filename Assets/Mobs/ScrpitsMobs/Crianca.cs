using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Crianca : MonoBehaviour
{
    private enum State { Idle, Circling, Frozen }

    [Header("Comportamento")]
    public float circlingRadius = 4f;
    public float runningSpeed = 5f;
    public float stoppingDistance = 1f;

    [Header("Referências")]
    public Transform playerTransform;
    public Light flashlight;
    public Collider roomTrigger;
    [Tooltip("Ponto específico no corpo da criança para checagem da lanterna.")]
    public Transform flashlightTargetPoint;

    private NavMeshAgent navMeshAgent;
    private State currentState = State.Idle;
    private bool isPlayerInRoom = false;
    private Animator animator; // << NOVO

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // << NOVO

        if (playerTransform == null || flashlight == null || roomTrigger == null || flashlightTargetPoint == null || animator == null) // << NOVO (adicionado animator)
        {
            Debug.LogError("Uma ou mais referências não foram definidas no Inspector ou o Animator não foi encontrado. O script será desativado.", this);
            enabled = false;
        }
    }

    void Start()
    {
        navMeshAgent.speed = runningSpeed;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.isStopped = true;
    }

    void Update()
    {
        UpdateAnimation(); // << NOVO

        if (!isPlayerInRoom)
        {
            SetState(State.Idle);
            return;
        }

        if (IsUnderFlashlight())
        {
            SetState(State.Frozen);
        }
        else
        {
            if (currentState == State.Frozen)
            {
                SetState(State.Circling);
            }
        }

        if (currentState == State.Circling)
        {
            UpdateCirclingMovement();
        }
    }
    
    // << NOVO MÉTODO COMPLETO
    private void UpdateAnimation()
    {
        // Pega a velocidade atual do NavMeshAgent (metros por segundo)
        float speed = navMeshAgent.velocity.magnitude;
        
        // Envia essa velocidade para o parâmetro "Speed" que criamos no Animator Controller
        animator.SetFloat("Speed", speed);
    }

    private void SetState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        navMeshAgent.isStopped = (newState == State.Idle || newState == State.Frozen);
    }

    private void UpdateCirclingMovement()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            Vector3 randomDirection = Random.insideUnitSphere * circlingRadius;
            randomDirection += playerTransform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, circlingRadius, 1))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }

    private bool IsUnderFlashlight()
    {
        if (!flashlight.enabled) return false;
        
        Vector3 targetPosition = flashlightTargetPoint.position;
        Vector3 directionToTarget = (targetPosition - flashlight.transform.position).normalized;
        float angle = Vector3.Angle(flashlight.transform.forward, directionToTarget);
        float distance = Vector3.Distance(flashlight.transform.position, targetPosition);

        if (angle < flashlight.spotAngle / 2f && distance < flashlight.range)
        {
            RaycastHit hit;
            if (Physics.Raycast(flashlight.transform.position, directionToTarget, out hit, flashlight.range))
            {
                if (hit.transform.IsChildOf(this.transform) || hit.transform == this.transform)
                {
                    Debug.Log("Lanterna está na criança!");
                    return true;
                }
            }
        }
        return false;
    }

    public void OnPlayerEnterRoom()
    {
        isPlayerInRoom = true;
        SetState(State.Circling);
        Debug.Log("Jogador entrou no quarto. A criança começou a correr.");
    }

    public void OnPlayerExitRoom()
    {
        isPlayerInRoom = false;
        SetState(State.Idle);
        Debug.Log("Jogador saiu do quarto. A criança parou.");
    }
}