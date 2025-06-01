using UnityEngine;

public class TerrorFogController : MonoBehaviour
{
    [Header("Configurações de Névoa")]
    public bool enableFog = true;
    public FogMode fogMode = FogMode.ExponentialSquared;
    public Color fogColor = new Color(0.1f, 0.1f, 0.15f, 1f);
    
    [Header("Névoa Dinâmica")]
    public float baseFogDensity = 0.015f;
    public float maxFogDensity = 0.035f;
    public float fogPulseSpeed = 0.5f;
    
    [Header("Efeito de Terror")]
    public bool enableTerrorMode = false;
    public Color terrorFogColor = new Color(0.2f, 0.05f, 0.05f, 1f); // Vermelho escuro
    public float terrorFogDensity = 0.05f;
    
    private float originalDensity;
    private Color originalColor;
    
    void Start()
    {
        SetupFog();
        originalDensity = baseFogDensity;
        originalColor = fogColor;
    }
    
    void Update()
    {
        if (enableFog)
        {
            UpdateDynamicFog();
        }
    }
    
    void SetupFog()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = baseFogDensity;
    }
    
    void UpdateDynamicFog()
    {
        if (!enableTerrorMode)
        {
            // Névoa pulsante suave
            float pulse = Mathf.Sin(Time.time * fogPulseSpeed) * 0.5f + 0.5f;
            float currentDensity = Mathf.Lerp(baseFogDensity, maxFogDensity, pulse * 0.3f);
            RenderSettings.fogDensity = currentDensity;
        }
        else
        {
            // Modo terror ativado
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, terrorFogColor, Time.deltaTime);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, terrorFogDensity, Time.deltaTime);
        }
    }
    
    public void ActivateTerrorMode()
    {
        enableTerrorMode = true;
    }
    
    public void DeactivateTerrorMode()
    {
        enableTerrorMode = false;
        RenderSettings.fogColor = originalColor;
        RenderSettings.fogDensity = originalDensity;
    }
    
    public void SetFogIntensity(float intensity)
    {
        baseFogDensity = Mathf.Clamp(intensity, 0.001f, 0.1f);
        RenderSettings.fogDensity = baseFogDensity;
    }
}