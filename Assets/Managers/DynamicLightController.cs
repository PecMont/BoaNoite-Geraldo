using UnityEngine;

public class DynamicLightController : MonoBehaviour
{
    [Header("Luzes do Jogo")]
    public Light[] sceneLights;
    
    [Header("Configurações de Tempo")]
    public bool enableDayNightCycle = false;
    public float dayDuration = 120f; // segundos
    
    [Header("Cores")]
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.2f, 0.2f, 0.4f);
    
    private float timeOfDay = 0.5f; // 0 = meia-noite, 0.5 = meio-dia, 1 = meia-noite
    
    void Update()
    {
        if (enableDayNightCycle)
        {
            UpdateDayNightCycle();
        }
    }
    
    void UpdateDayNightCycle()
    {
        timeOfDay += Time.deltaTime / dayDuration;
        if (timeOfDay >= 1) timeOfDay = 0;
        
        // Calcular intensidade da luz baseada na hora
        float lightIntensity = Mathf.Clamp01(Mathf.Cos((timeOfDay - 0.5f) * 2 * Mathf.PI) * 0.5f + 0.5f);
        Color currentColor = Color.Lerp(nightColor, dayColor, lightIntensity);
        
        // Aplicar às luzes
        foreach (Light light in sceneLights)
        {
            if (light != null)
            {
                light.intensity = lightIntensity;
                light.color = currentColor;
            }
        }
    }
    
    public void SetTimeOfDay(float time)
    {
        timeOfDay = Mathf.Clamp01(time);
    }
}