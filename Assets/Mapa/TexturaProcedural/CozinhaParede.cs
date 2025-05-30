using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParedeCozinha : MonoBehaviour
{
    public int textureWidth = 512;
    public int textureHeight = 512;

    // Cor base branca ou levemente off-white
    public Color baseColor = new Color(0.96f, 0.96f, 0.96f);

    [Header("Variação Suave da Cor (Sutil)")]
    public float overallVariationScale = 15f;
    public float overallVariationIntensity = 0.03f; // Manter sutil para paredes brancas

    [Header("Textura da Superfície (Mais Pronunciada)")]
    // Escala menor para uma textura mais "pontilhada" ou "granulada".
    // Experimente valores como 100f, 150f, ou até 200f para texturas mais finas e densas.
    public float surfaceDetailScale = 120f;
    // Intensidade maior para a textura ser mais visível
    public float surfaceDetailIntensity = 0.08f; // Aumentado para mais textura

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject.");
            return;
        }

        renderer.material.mainTexture = GenerateWallTexture();

        // Cozinhas podem ter um leve brilho acetinado para facilitar a limpeza
        // Ainda predominantemente fosco, mas um pouco menos que um quarto.
        renderer.material.SetFloat("_Glossiness", 0.1f); // Ajuste conforme necessário
        renderer.material.SetFloat("_Smoothness", 0.15f); // Para Standard Shader
    }

    Texture2D GenerateWallTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Bilinear; // Bilinear para suavizar um pouco

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xCoord = (float)x / textureWidth;
                float yCoord = (float)y / textureHeight;

                // Variação suave da cor base
                float overallNoise = (Mathf.PerlinNoise(xCoord * overallVariationScale, yCoord * overallVariationScale) - 0.5f) * overallVariationIntensity;

                // Detalhe fino / textura da tinta (mais pronunciado)
                float detailNoise = (Mathf.PerlinNoise(xCoord * surfaceDetailScale, yCoord * surfaceDetailScale) - 0.5f) * surfaceDetailIntensity;

                // Combina as variações
                float totalVariation = overallNoise + detailNoise;

                // Aplica a variação à cor base
                Color pixelColor = new Color(
                    baseColor.r + totalVariation,
                    baseColor.g + totalVariation,
                    baseColor.b + totalVariation,
                    baseColor.a
                );

                // Garante que os valores da cor fiquem dentro do intervalo válido [0, 1]
                pixelColor.r = Mathf.Clamp01(pixelColor.r);
                pixelColor.g = Mathf.Clamp01(pixelColor.g);
                pixelColor.b = Mathf.Clamp01(pixelColor.b);

                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return texture;
    }
}