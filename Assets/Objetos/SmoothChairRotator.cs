using UnityEngine;

public class SmoothChairRotator : MonoBehaviour
{
    [Header("Rotação Suave")]
    public float baseRotationSpeed = 20f;
    public float speedVariation = 5f; // Variação aleatória
    public Vector3 rotationAxis = Vector3.up;
    
    [Header("Padrão de Movimento")]
    public bool useRandomPauses = true;
    public float minPauseDuration = 1f;
    public float maxPauseDuration = 3f;
    public float minRotationDuration = 2f;
    public float maxRotationDuration = 5f;
    
    private bool isRotating = true;
    private float currentSpeed;
    private float stateTimer;
    private float stateDuration;
    
    void Start(){
        SetNewRotationState();
    }
    
    void Update(){
    stateTimer += Time.deltaTime;
    
    if (useRandomPauses && stateTimer >= stateDuration)
    {
        ToggleRotationState();
    }
    
    if (isRotating)
    {
        // Garantir rotação no próprio centro
        Vector3 rotation = rotationAxis * currentSpeed * Time.deltaTime;
        transform.Rotate(rotation, Space.Self);
    }
}
    
    void SetNewRotationState()
    {
        currentSpeed = baseRotationSpeed + Random.Range(-speedVariation, speedVariation);
        
        if (useRandomPauses)
        {
            stateDuration = isRotating ? 
                Random.Range(minRotationDuration, maxRotationDuration) :
                Random.Range(minPauseDuration, maxPauseDuration);
        }
        
        stateTimer = 0f;
    }
    
    void ToggleRotationState()
    {
        isRotating = !isRotating;
        SetNewRotationState();
    }

    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, transform.position + transform.rotation * rotationAxis);
}

}