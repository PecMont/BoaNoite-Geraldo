using UnityEngine;
using UnityEngine.UI;

public class LanternaSegue : MonoBehaviour
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

    [Header("Ajuste de Posição")]
    public Vector3 offset = new Vector3(0, -0.1f, 0.5f);

    private Transform target;

    void Start()
    {
        target = Camera.main.transform;

        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
            if (flashlight == null)
            {
                Debug.LogError("Nenhuma luz encontrada! Adicione uma Spot Light como filha deste objeto.");
                enabled = false;
                return;
            }
        }

        flashlight.enabled = false;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + target.TransformDirection(offset);
            transform.rotation = target.rotation;
        }

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
        if (!flashlight.enabled && batteryLife <= 0)
        {
            Debug.Log("Bateria esgotada! Não é possível ligar a lanterna.");
            return;
        }

        flashlight.enabled = !flashlight.enabled;

        if (clickSound != null)
        {
            clickSound.Play();
        }

        Debug.Log($"Lanterna: {(flashlight.enabled ? "Ligada" : "Desligada")} - Bateria: {batteryLife:F1}%");
    }

    void DrainBattery()
    {
        batteryLife -= batteryDrain * Time.deltaTime;
        batteryLife = Mathf.Clamp(batteryLife, 0, 100);

        flashlight.intensity = Mathf.Lerp(0.1f, 1.5f, batteryLife / 100f);

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
