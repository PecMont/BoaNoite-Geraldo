using UnityEngine;

public class HorrorLightingController : MonoBehaviour{
    [Header("Atmosfera de Horror")]
    public Light[] flickeringLights;
    public float flickerSpeed = 0.1f;
    public float flickerIntensity = 0.5f;
    
    [Header("Luzes de Emergência")]
    public Light[] emergencyLights;
    public Color emergencyColor = Color.red;
    public float emergencyBlinkSpeed = 2f;
    
    [Header("Integração com Névoa")]
    public TerrorFogController fogController;
    public bool syncFogWithLights = true;
    
    void Start(){
        if (fogController == null)
            fogController = FindObjectOfType<TerrorFogController>();
    }
    
    void Update(){
        FlickerLights();
        EmergencyBlink();
        
        if (syncFogWithLights && fogController != null)
        {
            SyncFogWithLighting();
        }
    }
    
    void FlickerLights(){
        foreach (Light light in flickeringLights){
            if (light != null){
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
                light.intensity = Mathf.Lerp(flickerIntensity, 1f, noise);
            }
        }
    }
    
    void EmergencyBlink(){
        float blink = Mathf.PingPong(Time.time * emergencyBlinkSpeed, 1);
        foreach (Light light in emergencyLights){
            if (light != null){
                light.intensity = blink;
                light.color = emergencyColor;
            }
        }
    }
    
    void SyncFogWithLighting(){
        // Aumentar névoa quando luzes piscam
        float avgLightIntensity = 0f;
        int lightCount = 0;
        
        foreach (Light light in flickeringLights){
            if (light != null){
                avgLightIntensity += light.intensity;
                lightCount++;
            }
        }
        
        if (lightCount > 0){
            avgLightIntensity /= lightCount;
            float fogIntensity = Mathf.Lerp(0.025f, 0.015f, avgLightIntensity);
            fogController.SetFogIntensity(fogIntensity);
        }
    }
    
    public void TriggerTerrorEvent(){
        if (fogController != null){
            fogController.ActivateTerrorMode();
        }
    }
}