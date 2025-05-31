using UnityEngine;

public class RealtimeLighting : MonoBehaviour
{
    [Header("Configurações de Luz")]
    public Light mainLight;
    public bool enableShadows = true;
    public LightShadows shadowType = LightShadows.Soft;
    
    [Header("Ciclo Dia/Noite")]
    public float dayDurationMinutes = 1f;   //  minutos de dia
    public float nightDurationMinutes = 4f; // 4 minutos de noite

    private float totalCycleDuration;
    private bool isDaytime = true;
    private float currentPhaseTime = 0f;
    private float rotationSpeed; // Variável que estava faltando

    void Start()
    {
        SetupRealtimeLighting();
        totalCycleDuration = (dayDurationMinutes + nightDurationMinutes) * 60f;
        // Calcular velocidade de rotação: 360 graus dividido pelo tempo total em segundos
        rotationSpeed = 360f / totalCycleDuration;
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

    void Update()
    {
        if (mainLight != null)
        {
            // Rotacionar o sol continuamente
            mainLight.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
            
            // Atualizar tempo da fase atual
            currentPhaseTime += Time.deltaTime;
            
            // Verificar mudança de fase (dia/noite)
            UpdateDayNightCycle();
        }
    }
    
    void UpdateDayNightCycle()
    {
        float dayDurationSeconds = dayDurationMinutes * 60f;
        float nightDurationSeconds = nightDurationMinutes * 60f;
        
        if (isDaytime && currentPhaseTime >= dayDurationSeconds)
        {
            // Mudar para noite
            isDaytime = false;
            currentPhaseTime = 0f;
        }
        else if (!isDaytime && currentPhaseTime >= nightDurationSeconds)
        {
            // Mudar para dia
            isDaytime = true;
            currentPhaseTime = 0f;
        }
    }
}