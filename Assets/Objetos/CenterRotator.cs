// Script simplificado para pivot correto
using UnityEngine;

public class CenterRotator : MonoBehaviour
{
    [Header("Rotação Central")]
    public float rotationSpeed = 20f;
    public Vector3 rotationAxis = Vector3.up;
    
    void Update()
    {
        // Rotação simples no próprio pivot
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}