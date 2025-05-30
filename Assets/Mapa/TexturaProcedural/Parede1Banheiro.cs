using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Paarede1Banheiro : MonoBehaviour
{
    [Header("Dimensões da Textura")]
    public int textureWidth = 512;
    public int textureHeight = 512;

    [Header("Cores Base do Carvalho Escuro")]
    public Color darkColor = new Color(0.2f, 0.13f, 0.08f);  // Um pouco mais escuro
    public Color midColor = new Color(0.35f, 0.25f, 0.18f);
    public Color lightColor = new Color(0.5f, 0.38f, 0.28f); // Um pouco mais claro para maior contraste

    [Header("Parâmetros da Granulação (Veios)")]
    [Tooltip("Escala geral da granulação. Valores menores = veios mais largos.")]
    public float grainScale = 5.0f; // Levemente ajustado
    [Tooltip("Quão esticados os veios são verticalmente.")]
    public float grainStretch = 8.0f;
    [Tooltip("Frequência das ondulações nos veios.")]
    public float waveFrequency = 1.5f;
    [Tooltip("Intensidade das ondulações nos veios.")]
    public float waveIntensity = 0.06f; // Levemente aumentado
    [Tooltip("Intensidade do ruído fino que detalha os veios.")]
    public float fineDetailNoiseIntensity = 0.25f; // AUMENTADO SIGNIFICATIVAMENTE
    [Tooltip("Potência de contraste para os veios. >1 aumenta contraste, <1 suaviza.")]
    public float woodGrainContrast = 1.8f; // NOVO PARÂMETRO

    [Header("Micro-Rugosidade da Superfície")]
    [Tooltip("Intensidade da micro-rugosidade. Adiciona 'aspereza' visual.")]
    public float microRoughnessIntensity = 0.05f; // NOVO PARÂMETRO
    [Tooltip("Escala da micro-rugosidade. Valores altos = padrão fino.")]
    public float microRoughnessScale = 250f;


    [Header("Parâmetros das Tábuas (Opcional)")]
    public bool enablePlanks = true;
    public int plankWidthPixels = 128;
    public int plankLineWidthPixels = 2;
    public float plankLineDarkening = 0.2f; // Levemente aumentado


    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject.");
            return;
        }

        renderer.material.mainTexture = GenerateWoodTexture();

        renderer.material.SetFloat("_Glossiness", 0.25f); // Menos brilho para parecer mais texturizado
        renderer.material.SetFloat("_Smoothness", 0.35f);
    }

    Texture2D GenerateWoodTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Bilinear;

        float plankSeedOffset = 0;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xNorm = (float)x / textureWidth;
                float yNorm = (float)y / textureHeight;

                if (enablePlanks && plankWidthPixels > 0)
                {
                    plankSeedOffset = Mathf.FloorToInt((float)x / plankWidthPixels) * 10f;
                }

                float yWave = yNorm + (Mathf.PerlinNoise(xNorm * waveFrequency + plankSeedOffset, yNorm * waveFrequency * 0.5f + plankSeedOffset) - 0.5f) * waveIntensity;
                float yStretched = yWave * grainStretch;

                float mainGrainValue = Mathf.PerlinNoise(xNorm * grainScale + plankSeedOffset, yStretched + plankSeedOffset);
                float fineDetailValue = (Mathf.PerlinNoise(xNorm * grainScale * 5f + plankSeedOffset, yStretched * 2f + plankSeedOffset) - 0.5f) * fineDetailNoiseIntensity;
                
                float combinedGrain = Mathf.Clamp01(mainGrainValue + fineDetailValue);

                // Aplicar contraste aos veios
                float contrastedGrain = Mathf.Pow(combinedGrain, woodGrainContrast);

                Color baseWoodColor;
                // Ajustar a interpolação de cores com base no grão contrastado
                // Pode ser necessário ajustar esses limiares (0.4f) dependendo do efeito do contraste
                if (contrastedGrain < 0.45f) // Limiar ajustado levemente
                {
                    baseWoodColor = Color.Lerp(darkColor, midColor, contrastedGrain / 0.45f);
                }
                else
                {
                    baseWoodColor = Color.Lerp(midColor, lightColor, (contrastedGrain - 0.45f) / (1f - 0.45f));
                }

                // Adicionar micro-rugosidade
                float microRoughnessFactor = (Mathf.PerlinNoise(xNorm * microRoughnessScale + plankSeedOffset, yNorm * microRoughnessScale + plankSeedOffset) - 0.5f) * microRoughnessIntensity;
                
                Color finalPixelColor = baseWoodColor;
                finalPixelColor.r = Mathf.Clamp01(finalPixelColor.r + microRoughnessFactor);
                finalPixelColor.g = Mathf.Clamp01(finalPixelColor.g + microRoughnessFactor);
                finalPixelColor.b = Mathf.Clamp01(finalPixelColor.b + microRoughnessFactor);


                if (enablePlanks && plankWidthPixels > 0 && plankLineWidthPixels > 0)
                {
                    if (x % plankWidthPixels < plankLineWidthPixels || x % plankWidthPixels >= plankWidthPixels - (plankLineWidthPixels > 1 ? plankLineWidthPixels / 2 : 0) )
                    {
                        finalPixelColor.r *= (1f - plankLineDarkening);
                        finalPixelColor.g *= (1f - plankLineDarkening);
                        finalPixelColor.b *= (1f - plankLineDarkening);
                    }
                }

                finalPixelColor.r = Mathf.Clamp01(finalPixelColor.r);
                finalPixelColor.g = Mathf.Clamp01(finalPixelColor.g);
                finalPixelColor.b = Mathf.Clamp01(finalPixelColor.b);

                texture.SetPixel(x, y, finalPixelColor);
            }
        }

        texture.Apply();
        return texture;
    }
}