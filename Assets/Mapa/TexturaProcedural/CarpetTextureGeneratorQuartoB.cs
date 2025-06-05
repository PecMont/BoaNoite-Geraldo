using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CarpetTexturaQuartoB : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public Color baseColor = new Color(0.4f, 0.5f, 0.6f); // Azul acinzentado

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();

        // Material bem fosco (carpete não brilha)
        renderer.material.SetFloat("_Glossiness", 0.02f);
        renderer.material.SetFloat("_Smoothness", 0.02f);
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;

                //Fibras mais definidas
                float horizontalFiber = Mathf.PerlinNoise(xCoord * 80f, yCoord * 4f) * 0.4f;
                float verticalFiber = Mathf.PerlinNoise(xCoord * 4f, yCoord * 80f) * 0.4f;

                //Ruído fino para fiapos
                float fineNoise = Mathf.PerlinNoise(xCoord * 150f, yCoord * 150f) * 0.05f;

                //Variação geral suave
                float largeNoise = Mathf.PerlinNoise(xCoord * 10f, yCoord * 10f) * 0.08f;

                //Soma tudo
                float totalVariation = horizontalFiber + verticalFiber + fineNoise + largeNoise;

                //Controla a intensidade final
                Color finalColor = baseColor * (1f - totalVariation * 0.9f);

                // Garante que não fique fora de faixa
                finalColor.r = Mathf.Clamp01(finalColor.r);
                finalColor.g = Mathf.Clamp01(finalColor.g);
                finalColor.b = Mathf.Clamp01(finalColor.b);

                texture.SetPixel(x, y, finalColor);
            }
        }

        texture.Apply();
        return texture;
    }
}
