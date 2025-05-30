using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BathroomTileGenerator : MonoBehaviour
{
    [Header("Dimensões da Textura")]
    public int textureWidth = 512;
    public int textureHeight = 512;

    [Header("Cores")]
    public Color tileColor = Color.white;
    public Color groutColor = Color.black;

    [Header("Propriedades do Azulejo e Rejunte")]
    // AQUI: Valor do tileSize reduzido para azulejos menores
    public int tileSize = 32; // Tamanho da parte cerâmica do azulejo (ex: 64, 32, 24, 16)
    public int groutWidth = 2;  // Largura da linha de rejunte (talvez precise reduzir se o tileSize for muito pequeno)

    [Header("Efeito de Borda Chanfrada/Boleada")]
    [Tooltip("Controla a curvatura da 'caída'. Valores < 1 resultam em bordas mais arredondadas/suaves. Valores > 1 em bordas mais 'sharp' para o centro.")]
    public float bevelCurvePower = 0.5f;
    [Tooltip("Quão mais escura a borda do azulejo fica em relação ao centro. (0 = sem escurecimento, 0.2 = 20% mais escura)")]
    public float bevelEdgeDarkening = 0.15f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject.");
            return;
        }

        renderer.material.mainTexture = GenerateTileTexture();

        renderer.material.SetFloat("_Glossiness", 0.85f);
        renderer.material.SetFloat("_Smoothness", 0.85f);
    }

    Texture2D GenerateTileTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Bilinear;

        int tileAndGroutSize = tileSize + groutWidth;

        // Prevenir divisão por zero se tileAndGroutSize for 0
        if (tileAndGroutSize <= 0) {
            Debug.LogError("tileSize + groutWidth deve ser maior que 0.");
            // Preenche com uma cor de erro ou retorna textura vazia
            for (int y_err = 0; y_err < textureHeight; y_err++) {
                for (int x_err = 0; x_err < textureWidth; x_err++) {
                    texture.SetPixel(x_err, y_err, Color.magenta);
                }
            }
            texture.Apply();
            return texture;
        }


        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                int currentBlockX = x % tileAndGroutSize;
                int currentBlockY = y % tileAndGroutSize;

                bool isGrout = currentBlockX >= tileSize || currentBlockY >= tileSize;

                if (isGrout)
                {
                    texture.SetPixel(x, y, groutColor);
                }
                else
                {
                    int coordInTileX = currentBlockX;
                    int coordInTileY = currentBlockY;

                    float distFromLeft = coordInTileX;
                    float distFromRight = (tileSize - 1) - coordInTileX;
                    float distFromTop = coordInTileY;
                    float distFromBottom = (tileSize - 1) - coordInTileY;

                    float minDistToEdge = Mathf.Min(distFromLeft, distFromRight, distFromTop, distFromBottom);
                    
                    float normalizationFactor = Mathf.Max(1f, (float)tileSize / 2.0f);
                    float normalizedDistFromEdge = Mathf.Clamp01(minDistToEdge / normalizationFactor);

                    float bevelShape = Mathf.Pow(normalizedDistFromEdge, bevelCurvePower);
                    float brightness = 1.0f - (1.0f - bevelShape) * bevelEdgeDarkening;

                    Color currentTileColor = tileColor * brightness;
                    currentTileColor.r = Mathf.Clamp01(currentTileColor.r);
                    currentTileColor.g = Mathf.Clamp01(currentTileColor.g);
                    currentTileColor.b = Mathf.Clamp01(currentTileColor.b);
                    currentTileColor.a = tileColor.a;

                    texture.SetPixel(x, y, currentTileColor);
                }
            }
        }

        texture.Apply();
        return texture;
    }
}