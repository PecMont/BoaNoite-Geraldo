using UnityEngine;
using System.Collections.Generic; // Para usar List

[RequireComponent(typeof(Renderer))] // Exige que o GameObject tenha um Renderer
public class QuartoChaos : MonoBehaviour
{
    [Header("Dimensões da Textura")]
    public int textureWidth = 512; // Largura da textura
    public int textureHeight = 512; // Altura da textura

    [Header("Paleta de Cores Suaves")]
    public List<Color> colorPalette = new List<Color>() {
        new Color(0.8f, 0.78f, 0.75f), // Cinza claro morno
        new Color(0.75f, 0.76f, 0.79f), // Cinza claro levemente azulado
        new Color(0.82f, 0.8f, 0.76f)  // Outro tom de cinza/bege
    };

    [Header("Layout dos Ladrilhos")]
    public int tileWidth = 40;  // Largura de cada ladrilho
    public int tileHeight = 24; // Altura de cada ladrilho

    [Header("Textura Interna e Bordas")]
    [Tooltip("Escala da textura fina dentro de cada ladrilho. Valores maiores = textura mais fina.")]
    public float fineDetailScale = 150f; // Escala do detalhe fino
    [Tooltip("Intensidade da textura fina. Mantenha baixo para sutileza.")]
    public float subtleTextureIntensity = 0.02f; // Intensidade do detalhe fino
    [Tooltip("Distância em pixels para o degradê da borda. Ajuste se os ladrilhos forem muito pequenos.")]
    public int edgeBlendDistance = 3; // Distância para suavizar borda
    [Tooltip("Quão escuras as bordas dos ladrilhos ficam (0 = sem efeito).")]
    public float edgeDarkeningIntensity = 0.05f; // Intensidade de escurecimento da borda

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>(); // Pega o Renderer
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject."); // Erro se não achar Renderer/Material
            return;
        }
        if (colorPalette == null || colorPalette.Count == 0) {
            Debug.LogError("Paleta de cores está vazia! Adicione ao menos uma cor."); // Erro se não houver cor
            colorPalette.Add(Color.gray); // Adiciona cinza se vazio
        }

        renderer.material.mainTexture = GenerateFloorTexture(); // Gera e aplica a textura

        renderer.material.SetFloat("_Glossiness", 0.1f); // Ajusta brilho
        renderer.material.SetFloat("_Smoothness", 0.15f); // Ajusta suavidade
    }

    Texture2D GenerateFloorTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight); // Cria textura
        texture.filterMode = FilterMode.Bilinear; // Suaviza textura

        if (tileWidth <= 0) tileWidth = 1; // Evita divisão por zero
        if (tileHeight <= 0) tileHeight = 1;

        for (int y = 0; y < textureHeight; y++) // Para cada linha
        {
            for (int x = 0; x < textureWidth; x++) // Para cada coluna
            {
                int row = y / tileHeight; // Calcula linha do ladrilho
                float offsetX = (row % 2 == 0) ? 0 : (float)tileWidth / 2.0f; // Offset para efeito de tijolo
                float effectiveX = x + offsetX; // X ajustado
                int col = Mathf.FloorToInt(effectiveX / tileWidth); // Coluna do ladrilho
                
                int colorIndex = Mathf.Abs(row + col) % colorPalette.Count; // Escolhe cor da paleta
                Color baseTileColor = colorPalette[colorIndex]; // Cor base

                float localX = effectiveX % tileWidth; // X local no ladrilho
                float localY = (float)y % tileHeight; // Y local no ladrilho

                float xNormInternal = localX / tileWidth; // X normalizado
                float yNormInternal = localY / tileHeight; // Y normalizado
                float fineNoise = (Mathf.PerlinNoise(xNormInternal * fineDetailScale + col * 10.0f, yNormInternal * fineDetailScale + row * 10.0f) - 0.5f) * subtleTextureIntensity; // Ruído para detalhe fino

                Color texturedColor = new Color(
                    baseTileColor.r + fineNoise, // Aplica ruído no R
                    baseTileColor.g + fineNoise, // Aplica ruído no G
                    baseTileColor.b + fineNoise, // Aplica ruído no B
                    baseTileColor.a // Mantém alpha
                );

                if (edgeDarkeningIntensity > 0 && edgeBlendDistance > 0)
                {
                    float distToLeftEdge = localX; // Distância até borda esquerda
                    float distToRightEdge = tileWidth - localX; // Até borda direita
                    float distToTopEdge = localY; // Até borda superior
                    float distToBottomEdge = tileHeight - localY; // Até borda inferior

                    float minDistToAnyEdge = Mathf.Min(Mathf.Min(distToLeftEdge, distToRightEdge), Mathf.Min(distToTopEdge, distToBottomEdge)); // Menor distância até borda
                    float edgeFactor = Mathf.Clamp01(minDistToAnyEdge / edgeBlendDistance); // Fator de suavização
                    float brightnessMultiplier = 1.0f - (1.0f - edgeFactor) * edgeDarkeningIntensity; // Multiplicador de brilho
                    texturedColor.r *= brightnessMultiplier; // Escurece R
                    texturedColor.g *= brightnessMultiplier; // Escurece G
                    texturedColor.b *= brightnessMultiplier; // Escurece B
                }
                
                texturedColor.r = Mathf.Clamp01(texturedColor.r); // Garante valor válido
                texturedColor.g = Mathf.Clamp01(texturedColor.g);
                texturedColor.b = Mathf.Clamp01(texturedColor.b);

                texture.SetPixel(x, y, texturedColor); // Define cor do pixel
            }
        }

        texture.Apply(); // Aplica mudanças na textura
        return texture; // Retorna textura pronta
    }
}
