using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParedeQuartoB : MonoBehaviour
{
    public int textureWidth = 512;
    public int textureHeight = 512;

    public Color baseColor = new Color(0.9f, 0.88f, 0.85f); 

    [Header("Variação Suave da Cor")]
    public float overallVariationScale = 10f; 
    public float overallVariationIntensity = 0.05f; 

    [Header("Detalhe Fino/Textura da Tinta")]
    public float surfaceDetailScale = 80f; 
    public float surfaceDetailIntensity = 0.02f; 

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject.");
            return;
        }

        renderer.material.mainTexture = GenerateWallTexture();

        // Paredes geralmente são foscas
        renderer.material.SetFloat("_Glossiness", 0.05f);
        renderer.material.SetFloat("_Smoothness", 0.05f);
        
    }

    Texture2D GenerateWallTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        // Usar Bilinear para variações suaves, Point para um granulado mais "pixelado"
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xCoord = (float)x / textureWidth;
                float yCoord = (float)y / textureHeight;

                
                float overallNoise = (Mathf.PerlinNoise(xCoord * overallVariationScale, yCoord * overallVariationScale) - 0.5f) * overallVariationIntensity;

                // 2. Detalhe fino / textura da tinta
                float detailNoise = (Mathf.PerlinNoise(xCoord * surfaceDetailScale, yCoord * surfaceDetailScale) - 0.5f) * surfaceDetailIntensity;

                // Combina as variações
                float totalVariation = overallNoise + detailNoise;

                // Aplica a variação à cor base
                // Adicionamos a variação a cada componente da cor
                Color pixelColor = new Color(
                    baseColor.r + totalVariation,
                    baseColor.g + totalVariation,
                    baseColor.b + totalVariation,
                    baseColor.a // Mantém o alfa original, se houver
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