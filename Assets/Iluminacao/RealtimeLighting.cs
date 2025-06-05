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


//LOGICA DE AMANHECER E ANOITCER FORA DA CASA
// COMENTADO POIS PREFIRMOS SEGIR COM TODOS OS DIAS NOITE

//using UnityEngine;
//using UnityEngine.UI;
//
//
//public class RealtimeLighting : MonoBehaviour
//{
//
//    public Image imagemUI;
//     
//    [Header("Configurações de Luz")]
//    public Light mainLight;
//    public bool enableShadows = true;
//    public LightShadows shadowType = LightShadows.Soft;
//    
//    [Header("Ciclo Dia/Noite")]
//    public float dayDurationMinutes = 1f;   // 1 minuto de dia
//    public float nightDurationMinutes = 4f; // 4 minutos de noite
//    
//    [Header("Configurações Visuais")]
//    public Color dayColor = Color.white;
//    public Color sunsetColor = new Color(1f, 0.6f, 0.3f);
//    public Color nightColor = new Color(0.2f, 0.2f, 0.5f);
//    public float dayIntensity = 1.5f;
//    public float nightIntensity = 0.3f;
//    
//    [Header("Configurações de Transição")]
//    public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
//    public bool smoothTransitions = true;
//
//    private float totalCycleDuration;
//    private bool isDaytime = true;
//    private float currentPhaseTime = 0f;
//    private float rotationSpeed;
//
//    void Start()
//    {
//        SetupRealtimeLighting();
//        totalCycleDuration = (dayDurationMinutes + nightDurationMinutes) * 60f;
//        // Calcular velocidade de rotação: 360 graus dividido pelo tempo total em segundos
//        rotationSpeed = 360f / totalCycleDuration;
//    }
//    
//    void SetupRealtimeLighting()
//    {
//        if (mainLight == null)
//            mainLight = GetComponent<Light>();
//            
//        if (mainLight != null)
//        {
//            // Configurar para tempo real
//            mainLight.lightmapBakeType = LightmapBakeType.Realtime;
//            
//            // Configurar sombras
//            mainLight.shadows = enableShadows ? shadowType : LightShadows.None;
//            mainLight.shadowStrength = 0.8f;
//            
//            // Configurar resolução de sombras
//            mainLight.shadowCustomResolution = 1024;
//            
//            // Configurar estado inicial
//            mainLight.color = dayColor;
//            mainLight.intensity = dayIntensity;
//        }
//    }
//
//    void Update()
//    {
//        if (mainLight != null)
//        {
//            // Rotacionar o sol continuamente
//            mainLight.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
//            
//            // Atualizar tempo da fase atual
//            currentPhaseTime += Time.deltaTime;
//            
//            // Verificar mudança de fase (dia/noite) e aplicar efeitos visuais
//            UpdateDayNightCycle();
//        }
//    }
//    
//    void UpdateDayNightCycle()
//    {
//        float dayDurationSeconds = dayDurationMinutes * 60f;
//        float nightDurationSeconds = nightDurationMinutes * 60f;
//        
//        if (isDaytime)
//        {
//            // Durante o dia - transição do amanhecer para meio-dia
//            float dayProgress = currentPhaseTime / dayDurationSeconds;
//            
//            if (smoothTransitions)
//            {
//                // Usar curva para transição suave
//                float curveValue = intensityCurve.Evaluate(dayProgress);
//                mainLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, curveValue);
//                mainLight.color = Color.Lerp(sunsetColor, dayColor, curveValue);
//            }
//            else
//            {
//                // Transição linear
//                mainLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, dayProgress);
//                mainLight.color = Color.Lerp(sunsetColor, dayColor, dayProgress);
//            }
//            
//            if (currentPhaseTime >= dayDurationSeconds)
//            {
//                // Mudar para noite
//                isDaytime = false;
//                currentPhaseTime = 0f;
//                Debug.Log("Transição para noite iniciada");
//            }
//        }
//        else
//        {
//            // Durante a noite - transição do pôr do sol para madrugada
//            float nightProgress = currentPhaseTime / nightDurationSeconds;
//            
//            if (smoothTransitions)
//            {
//                // Usar curva para transição suave
//                float curveValue = intensityCurve.Evaluate(nightProgress);
//                mainLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, curveValue);
//                mainLight.color = Color.Lerp(dayColor, nightColor, curveValue);
//            }
//            else
//            {
//                // Transição linear
//                mainLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, nightProgress);
//                mainLight.color = Color.Lerp(dayColor, nightColor, nightProgress);
//            }
//            
//            if (currentPhaseTime >= nightDurationSeconds)
//            {
//                // Mudar para dia
//                isDaytime = true;
//                currentPhaseTime = 0f;
//                Debug.Log("Transição para dia iniciada");
//            }
//        }
//    }
//    
//    // Método público para forçar mudança de ciclo (útil para testes)
//    public void ForceDayNightToggle()
//    {
//        isDaytime = !isDaytime;
//        currentPhaseTime = 0f;
//        Debug.Log($"Forçando mudança para: {(isDaytime ? "Dia" : "Noite")}");
//    }
//    
//    // Método para obter informações do ciclo atual
//    public string GetCurrentCycleInfo()
//    {
//        float progress = isDaytime ? 
//            currentPhaseTime / (dayDurationMinutes * 60f) : 
//            currentPhaseTime / (nightDurationMinutes * 60f);
//        
//        return $"Fase: {(isDaytime ? "Dia" : "Noite")} | Progresso: {(progress * 100f):F1}%";
//    }
//    
//    // Método para ajustar velocidade do ciclo em runtime
//    public void SetCycleSpeed(float multiplier)
//    {
//        rotationSpeed = (360f / totalCycleDuration) * multiplier;
//    }
//}
//
//