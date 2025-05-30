using UnityEngine;

public class RealtimeLighting : MonoBehaviour
{
    [Header("Configurações de Luz")]
    public Light mainLight;
    public bool enableShadows = true;
    public LightShadows shadowType = LightShadows.Soft;
    
    void Start()
    {
        SetupRealtimeLighting();
    }
    
    void SetupRealtimeLighting()
    {
        if (mainLight == null)
            mainLight = GetComponent<Light>();
            
        if (mainLight != null)
        {
            // Configurar para tempo real
            mainLight.lightmapBakeType = LightmapBakeType.Realtime;
            
            // Configurar sombras
            mainLight.shadows = enableShadows ? shadowType : LightShadows.None;
            mainLight.shadowStrength = 0.8f;
            
            // Configurar resolução de sombras (método correto para versões recentes do Unity)
            mainLight.shadowCustomResolution = 1024; // ou 2048 para alta qualidade
        }
    }
}