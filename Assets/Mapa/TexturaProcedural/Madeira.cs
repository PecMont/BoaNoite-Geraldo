using UnityEngine;

[RequireComponent(typeof(Renderer))] 
public class Madeira : MonoBehaviour
{
    public int width = 1024; // Largura da textura
    public int height = 1024; // Altura da textura
    public int plankCount = 50; // Número de ripas

    public Color baseColor = new Color(0.7f, 0.5f, 0.3f); // Cor base da madeira

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>(); // Pega o Renderer do objeto
        Texture2D texture = GenerateTexture(); // Gera a textura procedural
        renderer.material.mainTexture = texture; // Aplica a textura no material

        // Gera o Normal Map
        Texture2D normalMap = GenerateNormalMap(texture, 5f); // Cria o normal map
        renderer.material.SetTexture("_BumpMap", normalMap); // Aplica o normal map
        renderer.material.EnableKeyword("_NORMALMAP"); // Ativa o normal map

        // Configura material mais fosco
        renderer.material.SetFloat("_Glossiness", 0.1f); // Deixa pouco brilhante
        renderer.material.SetFloat("_Smoothness", 0.1f); // Deixa pouco liso
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height); // Cria nova textura
        texture.filterMode = FilterMode.Bilinear; // Suaviza a textura

        float plankWidth = (float)width / plankCount; // Calcula largura de cada ripa

        for (int x = 0; x < width; x++) // Percorre largura
        {
            for (int y = 0; y < height; y++) // Percorre altura
            {
                float xCoord = (float)x / width; // Normaliza x
                float yCoord = (float)y / height; // Normaliza y

                // Veios da madeira
                float woodGrain = Mathf.PerlinNoise(xCoord * 20f, yCoord * 200f) * 0.2f; // Veios finos
                woodGrain += Mathf.PerlinNoise(xCoord * 100f, yCoord * 10f) * 0.1f; // Veios grossos

                // Divisão entre ripas (mais fina)
                float plankEdge = Mathf.Abs((x % plankWidth) / plankWidth - 0.5f) * 2f; // Distância da borda da ripa
                float edgeDarkening = Mathf.Clamp01(1f - plankEdge * 40f) * 0.1f; // Escurece borda

                // Sujeira nas bordas
                float dirtTop = Mathf.Clamp01(1f - yCoord * 5f) * 0.3f; // Sujeira em cima
                float dirtBottom = Mathf.Clamp01(yCoord * 5f) * 0.3f; // Sujeira embaixo
                float dirt = dirtTop + dirtBottom; // Soma sujeiras

                // Combina tudo
                float variation = woodGrain + edgeDarkening + dirt; // Soma variações

                Color finalColor = baseColor * (1f - variation); // Aplica variação na cor

                // Clamping
                finalColor.r = Mathf.Clamp01(finalColor.r); // Garante valor válido
                finalColor.g = Mathf.Clamp01(finalColor.g);
                finalColor.b = Mathf.Clamp01(finalColor.b);

                texture.SetPixel(x, y, finalColor); // Define cor do pixel
            }
        }

        texture.Apply(); // Aplica mudanças na textura
        return texture; // Retorna textura pronta
    }

    // Função para gerar Normal Map procedural
    Texture2D GenerateNormalMap(Texture2D source, float strength = 4f)
    {
        Texture2D normalMap = new Texture2D(source.width, source.height); // Cria textura para normal map
        for (int x = 1; x < source.width - 1; x++) // Percorre largura (evita bordas)
        {
            for (int y = 1; y < source.height - 1; y++) // Percorre altura (evita bordas)
            {
                float left = source.GetPixel(x - 1, y).grayscale; // Pixel à esquerda
                float right = source.GetPixel(x + 1, y).grayscale; // Pixel à direita
                float down = source.GetPixel(x, y - 1).grayscale; // Pixel abaixo
                float up = source.GetPixel(x, y + 1).grayscale; // Pixel acima

                float dx = (right - left) * strength; // Diferença horizontal
                float dy = (up - down) * strength; // Diferença vertical

                Vector3 normal = new Vector3(-dx, -dy, 1).normalized; // Calcula normal
                Color nColor = new Color(normal.x * 0.5f + 0.5f, // Converte para cor
                                         normal.y * 0.5f + 0.5f,
                                         normal.z * 0.5f + 0.5f);
                normalMap.SetPixel(x, y, nColor); // Define cor do pixel no normal map
            }
        }

        normalMap.Apply(); // Aplica mudanças
        return normalMap; // Retorna normal map
    }
}
