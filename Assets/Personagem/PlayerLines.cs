using UnityEngine;

[System.Serializable]
public struct PlayerLinesData
{
    public string lineText;
    public float duration;
    public string requisite;
}

public class PlayerLines : MonoBehaviour
{
    public PlayerLinesData[] lines;
    public int currentLineIndex = 0;

    public TMPro.TextMeshProUGUI lineTextUI; // Referência ao componente TextMeshProUGUI

    private bool isLineActive = false; // Variável para controlar se a linha está ativa

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Line(); // Chama o método Line a cada frame
    }

    private void Line(){
        if (currentLineIndex < lines.Length && !isLineActive)
        {
            // Verifica se a linha atual tem um requisito
            if (string.IsNullOrEmpty(lines[currentLineIndex].requisite)){
                // Atualiza o texto da linha atual
                lineTextUI.text = lines[currentLineIndex].lineText;
                
                // apaga a linha atual após o tempo especificado
                Invoke("Limparfala", lines[currentLineIndex].duration);
                isLineActive = true; // Marca que a linha está ativa
            }
            else
            {
                // Aqui você pode adicionar lógica para verificar o requisito
                // Por exemplo, se o requisito for cumprido, então exibe a linha
                // Caso contrário, você pode ignorar ou mostrar uma mensagem de erro
            }
        }
    }

    void Limparfala()
    {
        // Limpa o texto da linha atual
        

        lineTextUI.text = "";
        currentLineIndex++;
        isLineActive = false; // Marca que a linha não está mais ativa
    }
}
