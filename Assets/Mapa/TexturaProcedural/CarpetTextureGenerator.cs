using UnityEngine;

// Exige que o objeto tenha um Renderer
[RequireComponent(typeof(Renderer))]
public class CarpetTextura : MonoBehaviour
{
    // Largura e altura da textura
    public int width = 512;
    public int height = 512;
    // Cor base do carpete
    public Color baseColor = new Color(0.4f, 0.5f, 0.6f); // Azul acinzentado

    void Start()
    {
        // Obtém o Renderer e aplica a textura gerada
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();

        // Define o material como fosco (carpete não brilha)
        renderer.material.SetFloat("_Glossiness", 0.02f);
        renderer.material.SetFloat("_Smoothness", 0.02f);
    }

    // Gera a textura procedural do carpete
    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;

        // Percorre cada pixel da textura
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;

                // Ruído para fibras horizontais
                float horizontalFiber = Mathf.PerlinNoise(xCoord * 80f, yCoord * 4f) * 0.4f;
                // Ruído para fibras verticais
                float verticalFiber = Mathf.PerlinNoise(xCoord * 4f, yCoord * 80f) * 0.4f;
                // Ruído fino para detalhes
                float fineNoise = Mathf.PerlinNoise(xCoord * 150f, yCoord * 150f) * 0.05f;
                // Ruído suave para variação geral
                float largeNoise = Mathf.PerlinNoise(xCoord * 10f, yCoord * 10f) * 0.08f;

                // Soma todas as variações
                float totalVariation = horizontalFiber + verticalFiber + fineNoise + largeNoise;

                // Calcula a cor final do pixel
                Color finalColor = baseColor * (1f - totalVariation * 0.9f);

                // Garante que os valores de cor estejam no intervalo [0,1]
                finalColor.r = Mathf.Clamp01(finalColor.r);
                finalColor.g = Mathf.Clamp01(finalColor.g);
                finalColor.b = Mathf.Clamp01(finalColor.b);

                // Define a cor do pixel na textura
                texture.SetPixel(x, y, finalColor);
            }
        }

        texture.Apply(); // Aplica as alterações na textura
        return texture;
    }
}
