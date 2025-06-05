using UnityEngine; // Importa a biblioteca do Unity

[RequireComponent(typeof(Renderer))] 
public class ChaoCozinha : MonoBehaviour 
{
    public int textureWidth = 512; // Largura da textura
    public int textureHeight = 512; // Altura da textura
    public int tileSize = 32; // Tamanho de cada azulejo em pixels
    public Color colorA = Color.white; // Cor do primeiro tipo de azulejo
    public Color colorB = Color.black; // Cor do segundo tipo de azulejo

    void Start() 
    {
        Renderer renderer = GetComponent<Renderer>(); // Pega o componente Renderer
        if (renderer == null || renderer.material == null) // Verifica se existe Renderer e Material
        {
            Debug.LogError("Renderer ou Material não encontrado neste GameObject."); // Mostra erro se não encontrar
            return; 
        }

        renderer.material.mainTexture = GenerateTileTexture(); // Define a textura gerada como principal do material

        // Azulejos Brilho
        renderer.material.SetFloat("_Glossiness", 0.8f); // Define o brilho do material
        renderer.material.SetFloat("_Smoothness", 0.8f); // Define a suavidade do material
    }

    Texture2D GenerateTileTexture() // Função para gerar a textura dos azulejos
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight); // Cria uma nova textura
        texture.filterMode = FilterMode.Point; // Define filtro para bordas nítidas

        for (int y = 0; y < textureHeight; y++) // Percorre cada linha da textura
        {
            for (int x = 0; x < textureWidth; x++) // Percorre cada coluna da textura
            {
                int tileX = x / tileSize; // Calcula o índice do azulejo na horizontal
                int tileY = y / tileSize; // Calcula o índice do azulejo na vertical

                bool isColorA = (tileX + tileY) % 2 == 0; // Alterna entre as cores

                if (isColorA) // Se for cor A
                {
                    texture.SetPixel(x, y, colorA); // Define o pixel como cor A
                }
                else // Se for cor B
                {
                    texture.SetPixel(x, y, colorB); // Define o pixel como cor B
                }
            }
        }

        texture.Apply(); // Aplica as mudanças na textura
        return texture; // Retorna a textura criada
    }
}
