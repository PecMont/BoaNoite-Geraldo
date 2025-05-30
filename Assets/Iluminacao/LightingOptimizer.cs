using UnityEngine;

public class LightingOptimizer : MonoBehaviour
{
    [Header("Otimização")]
    public float lightCullingDistance = 50f;
    public int maxRealtimeLights = 8;
    
    private Camera playerCamera;
    
    void Start()
    {
        playerCamera = Camera.main;
        OptimizeLighting();
    }
    
    void OptimizeLighting()
    {
        // Configurar culling de luzes por distância
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        
        foreach (Light light in allLights)
        {
            if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                // Ajustar range baseado na distância
                if (Vector3.Distance(playerCamera.transform.position, light.transform.position) > lightCullingDistance)
                {
                    light.enabled = false;
                }
                else
                {
                    light.enabled = true;
                }
            }
        }
    }
    
    void Update()
    {
        // Atualizar otimização em tempo real (opcional - pode impactar performance)
        if (Time.frameCount % 60 == 0) // Executa a cada 60 frames (~1 segundo)
        {
            OptimizeLighting();
        }
    }
}