using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [Header("Lanterna")]
    public Light flashlight;
    public KeyCode toggleKey = KeyCode.F;
    public float batteryLife = 100f;
    public float batteryDrain = 0.01f;
    
    [Header("UI (Opcional)")]
    public Slider batterySlider;
    public Text batteryText;
    
    [Header("Efeitos")]
    public AudioSource clickSound;
    
    void Start()
    {
        // Tenta encontrar a luz automaticamente se não foi atribuída
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
            if (flashlight == null)
            {
                Debug.LogError("Nenhuma luz encontrada! Adicione uma Spot Light como filha do Player.");
                enabled = false; // Desativa este script
                return;
            }
        }
        
        // Garante que a lanterna comece desligada
        flashlight.enabled = false;
    }
    
    void Update()
    {
        if (flashlight == null) return; // Segurança extra
        
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
        
        if (flashlight.enabled && batteryLife > 0)
        {
            DrainBattery();
        }
        
        UpdateUI();
    }
    
    void ToggleFlashlight()
    {
        // Só liga se tem bateria
        if (!flashlight.enabled && batteryLife <= 0)
        {
            Debug.Log("Bateria esgotada! Não é possível ligar a lanterna.");
            return;
        }
        
        flashlight.enabled = !flashlight.enabled;
        
        // Som de click
        if (clickSound != null)
            clickSound.Play();
            
        Debug.Log($"Lanterna: {(flashlight.enabled ? "Ligada" : "Desligada")} - Bateria: {batteryLife:F1}%");
    }
    
    void DrainBattery()
    {
        batteryLife -= batteryDrain * Time.deltaTime;
        batteryLife = Mathf.Clamp(batteryLife, 0, 100);
        
        // Diminui intensidade conforme bateria acaba
        flashlight.intensity = Mathf.Lerp(0.1f, 1.5f, batteryLife / 100f);
        
        // Desliga quando acaba a bateria
        if (batteryLife <= 0)
        {
            flashlight.enabled = false;
            Debug.Log("Bateria esgotada! Lanterna desligada.");
        }
    }
    
    void UpdateUI()
    {
        if (batterySlider != null)
            batterySlider.value = batteryLife / 100f;
            
        if (batteryText != null)
            batteryText.text = $"Bateria: {batteryLife:F0}%";
    }
}