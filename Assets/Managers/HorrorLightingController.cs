using UnityEngine;

public class HorrorLightingController : MonoBehaviour
{
    [Header("Atmosfera de Horror")]
    public Light[] flickeringLights; // Luzes que piscam
    public float flickerSpeed = 0.1f;
    public float flickerIntensity = 0.5f;
    
    [Header("Luzes de Emergência")]
    public Light[] emergencyLights;
    public Color emergencyColor = Color.red;
    public float emergencyBlinkSpeed = 2f;
    
    void Update()
    {
        FlickerLights();
        EmergencyBlink();
    }
    
    void FlickerLights()
    {
        foreach (Light light in flickeringLights)
        {
            if (light != null)
            {
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
                light.intensity = Mathf.Lerp(flickerIntensity, 1f, noise);
            }
        }
    }
    
    void EmergencyBlink()
    {
        float blink = Mathf.PingPong(Time.time * emergencyBlinkSpeed, 1);
        foreach (Light light in emergencyLights)
        {
            if (light != null)
            {
                light.intensity = blink;
                light.color = emergencyColor;
            }
        }
    }
}