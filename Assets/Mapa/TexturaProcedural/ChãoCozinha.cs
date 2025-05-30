using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ChãoCozinha : MonoBehaviour
{
    public int textureWidth = 512;
    public int textureHeight = 512;

    // AQUI: Diminua este valor para ter azulejos menores.
    // Por exemplo, de 64 para 32 ou 16.
    public int tileSize = 32; // <<-- VALOR ALTERADO PARA AZULEJOS MENORES

    public Color colorA = Color.white;
    public Color colorB = Color.black;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject.");
            return;
        }

        renderer.material.mainTexture = GenerateTileTexture();

        // Azulejos de cozinha costumam ser brilhantes
        renderer.material.SetFloat("_Glossiness", 0.8f);
        renderer.material.SetFloat("_Smoothness", 0.8f);
    }

    Texture2D GenerateTileTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        texture.filterMode = FilterMode.Point; // Para bordas de azulejo bem definidas

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                int tileX = x / tileSize;
                int tileY = y / tileSize;

                bool isColorA = (tileX + tileY) % 2 == 0;

                if (isColorA)
                {
                    texture.SetPixel(x, y, colorA);
                }
                else
                {
                    texture.SetPixel(x, y, colorB);
                }
            }
        }

        texture.Apply();
        return texture;
    }
}