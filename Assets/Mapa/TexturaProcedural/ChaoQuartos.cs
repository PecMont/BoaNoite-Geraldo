using UnityEngine;
using System.Collections.Generic; // Para usar List

[RequireComponent(typeof(Renderer))]
public class QuartoChaos : MonoBehaviour
{
    [Header("Dimensões da Textura")]
    public int textureWidth = 512;
    public int textureHeight = 512;

    [Header("Paleta de Cores Suaves")]
    public List<Color> colorPalette = new List<Color>() {
        new Color(0.8f, 0.78f, 0.75f), // Cinza claro morno
        new Color(0.75f, 0.76f, 0.79f), // Cinza claro levemente azulado
        new Color(0.82f, 0.8f, 0.76f)  // Outro tom de cinza/bege
    };

    [Header("Layout dos Ladrilhos")]
    // AQUI: Valores reduzidos para ladrilhos BEM menores
    public int tileWidth = 40;  // Largura de cada ladrilho/bloco (era 128)
    public int tileHeight = 24; // Altura de cada ladrilho/bloco (era 80)

    [Header("Textura Interna e Bordas")]
    [Tooltip("Escala da textura fina dentro de cada ladrilho. Valores maiores = textura mais fina.")]
    public float fineDetailScale = 150f;
    [Tooltip("Intensidade da textura fina. Mantenha baixo para sutileza.")]
    public float subtleTextureIntensity = 0.02f;
    [Tooltip("Distância em pixels para o degradê da borda. Ajuste se os ladrilhos forem muito pequenos.")]
    public int edgeBlendDistance = 3; // Reduzido um pouco (era 5). Pode precisar de mais ajuste.
    [Tooltip("Quão escuras as bordas dos ladrilhos ficam (0 = sem efeito).")]
    public float edgeDarkeningIntensity = 0.05f;


    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject.");
            return;
        }
        if (colorPalette == null || colorPalette.Count == 0) {
            Debug.LogError("Paleta de cores está vazia! Adicione ao menos uma cor.");
            colorPalette.Add(Color.gray);
        }

        renderer.material.mainTexture = GenerateFloorTexture();

        renderer.material.SetFloat("_Glossiness", 0.1f);
        renderer.material.SetFloat("_Smoothness", 0.15f);
    }

    Texture2D GenerateFloorTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Bilinear;

        if (tileWidth <= 0) tileWidth = 1;
        if (tileHeight <= 0) tileHeight = 1;


        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                int row = y / tileHeight;
                float offsetX = (row % 2 == 0) ? 0 : (float)tileWidth / 2.0f;
                float effectiveX = x + offsetX;
                int col = Mathf.FloorToInt(effectiveX / tileWidth);
                
                int colorIndex = Mathf.Abs(row + col) % colorPalette.Count;
                Color baseTileColor = colorPalette[colorIndex];

                float localX = effectiveX % tileWidth;
                float localY = (float)y % tileHeight;

                float xNormInternal = localX / tileWidth;
                float yNormInternal = localY / tileHeight;
                float fineNoise = (Mathf.PerlinNoise(xNormInternal * fineDetailScale + col * 10.0f, yNormInternal * fineDetailScale + row * 10.0f) - 0.5f) * subtleTextureIntensity;

                Color texturedColor = new Color(
                    baseTileColor.r + fineNoise,
                    baseTileColor.g + fineNoise,
                    baseTileColor.b + fineNoise,
                    baseTileColor.a
                );

                if (edgeDarkeningIntensity > 0 && edgeBlendDistance > 0)
                {
                    float distToLeftEdge = localX;
                    float distToRightEdge = tileWidth - localX; // Ajustado para tileWidth (sem -1) para consistência com localX
                    float distToTopEdge = localY;
                    float distToBottomEdge = tileHeight - localY; // Ajustado para tileHeight (sem -1)

                    float minDistToAnyEdge = Mathf.Min(Mathf.Min(distToLeftEdge, distToRightEdge), Mathf.Min(distToTopEdge, distToBottomEdge));
                    float edgeFactor = Mathf.Clamp01(minDistToAnyEdge / edgeBlendDistance);
                    float brightnessMultiplier = 1.0f - (1.0f - edgeFactor) * edgeDarkeningIntensity;
                    texturedColor.r *= brightnessMultiplier;
                    texturedColor.g *= brightnessMultiplier;
                    texturedColor.b *= brightnessMultiplier;
                }
                
                texturedColor.r = Mathf.Clamp01(texturedColor.r);
                texturedColor.g = Mathf.Clamp01(texturedColor.g);
                texturedColor.b = Mathf.Clamp01(texturedColor.b);

                texture.SetPixel(x, y, texturedColor);
            }
        }

        texture.Apply();
        return texture;
    }
}