// Scripts/SanityEffects.cs
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // Importante!

public class SanityEffects : MonoBehaviour
{
    public PostProcessVolume postProcessVolume; // Arraste seu PostProcessVolume aqui

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    void Start()
    {
        // Tenta obter os efeitos do profile. Garanta que eles já foram adicionados ao Profile no editor.
        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);
    }

    void OnEnable() => PlayerSanity.OnSanityChanged += UpdateVisualEffects;
    void OnDisable() => PlayerSanity.OnSanityChanged -= UpdateVisualEffects;

    private void UpdateVisualEffects(float currentSanity, float maxSanity)
    {
        float sanityPercentage = currentSanity / maxSanity; // Valor de 0.0 a 1.0

        // Ajusta o Vignette: quanto menor a sanidade, mais escuro nas bordas
        if (vignette != null)
        {
            // Lerp para suavizar a transição. Queremos que a intensidade seja alta (ex: 0.5) quando a sanidade é 0,
            // e baixa (ex: 0) quando a sanidade é 100.
            vignette.intensity.value = Mathf.Lerp(0.5f, 0f, sanityPercentage);
        }

        // Ajusta a Aberração Cromática: quanto menor a sanidade, maior a distorção das cores
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0.75f, 0f, sanityPercentage);
        }

        // Aqui você pode adicionar lógica para sussurros, tremores de câmera, etc.
    }
}