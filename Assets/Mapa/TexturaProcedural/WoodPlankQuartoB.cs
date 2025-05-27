using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WoodPlankQuartoB : MonoBehaviour
{
    public int width = 1024;
    public int height = 1024;
    public int plankCount = 50; // Número de ripas

    public Color baseColor = new Color(0.7f, 0.5f, 0.3f); // Marrom claro

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Texture2D texture = GenerateTexture();
        renderer.material.mainTexture = texture;

        // 🔸 Gera o Normal Map
        Texture2D normalMap = GenerateNormalMap(texture, 5f);
        renderer.material.SetTexture("_BumpMap", normalMap);
        renderer.material.EnableKeyword("_NORMALMAP");

        // 🔸 Configura material mais fosco
        renderer.material.SetFloat("_Glossiness", 0.1f);
        renderer.material.SetFloat("_Smoothness", 0.1f);
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;

        float plankWidth = (float)width / plankCount;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;

                // 🔸 Veios da madeira
                float woodGrain = Mathf.PerlinNoise(xCoord * 20f, yCoord * 200f) * 0.2f;
                woodGrain += Mathf.PerlinNoise(xCoord * 100f, yCoord * 10f) * 0.1f;

                // 🔸 Divisão entre ripas (mais fina)
                float plankEdge = Mathf.Abs((x % plankWidth) / plankWidth - 0.5f) * 2f;
                float edgeDarkening = Mathf.Clamp01(1f - plankEdge * 40f) * 0.1f;

                // 🔸 Sujeira nas bordas
                float dirtTop = Mathf.Clamp01(1f - yCoord * 5f) * 0.3f;
                float dirtBottom = Mathf.Clamp01(yCoord * 5f) * 0.3f;
                float dirt = dirtTop + dirtBottom;

                // 🔸 Combina tudo
                float variation = woodGrain + edgeDarkening + dirt;

                Color finalColor = baseColor * (1f - variation);

                // 🔸 Clamping
                finalColor.r = Mathf.Clamp01(finalColor.r);
                finalColor.g = Mathf.Clamp01(finalColor.g);
                finalColor.b = Mathf.Clamp01(finalColor.b);

                texture.SetPixel(x, y, finalColor);
            }
        }

        texture.Apply();
        return texture;
    }

    // 🔥 Função para gerar Normal Map procedural
    Texture2D GenerateNormalMap(Texture2D source, float strength = 4f)
    {
        Texture2D normalMap = new Texture2D(source.width, source.height);
        for (int x = 1; x < source.width - 1; x++)
        {
            for (int y = 1; y < source.height - 1; y++)
            {
                float left = source.GetPixel(x - 1, y).grayscale;
                float right = source.GetPixel(x + 1, y).grayscale;
                float down = source.GetPixel(x, y - 1).grayscale;
                float up = source.GetPixel(x, y + 1).grayscale;

                float dx = (right - left) * strength;
                float dy = (up - down) * strength;

                Vector3 normal = new Vector3(-dx, -dy, 1).normalized;
                Color nColor = new Color(normal.x * 0.5f + 0.5f,
                                         normal.y * 0.5f + 0.5f,
                                         normal.z * 0.5f + 0.5f);
                normalMap.SetPixel(x, y, nColor);
            }
        }

        normalMap.Apply();
        return normalMap;
    }
}
