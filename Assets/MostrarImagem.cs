using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MostrarImagem : MonoBehaviour
{
    public Image imagemUI;               // Referência à imagem da UI (com fundo)
    public TMP_Text mensagemTexto;
         // Referência ao componente de texto dentro da imagem

    void Start()
    {
        if (imagemUI != null)
            imagemUI.gameObject.SetActive(false);  // Começa invisível

        if (mensagemTexto != null)
            mensagemTexto.text = "";  // Limpa mensagem inicial
    }

    void Update()
    {
        // Teste com a tecla E para simular o resultado
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool resultado = TestarResultado();  // Simula se o jogador venceu ou perdeu
            MostrarMensagem(resultado);         // Chama a função com base no resultado
        }
    }

    /// <summary>
    /// Mostra a imagem com a mensagem dependendo do resultado.
    /// Chame esta função quando o jogador completar ou falhar uma etapa.
    /// </summary>
    /// <param name="resultado">Se true: venceu. Se false: perdeu.</param>
    public void MostrarMensagem(bool resultado)
    {
        if (imagemUI != null)
            imagemUI.gameObject.SetActive(true);

        if (mensagemTexto != null)
        {
            if (resultado)
                mensagemTexto.text = "Você conseguiu! O dia amanheceu";
            else
                mensagemTexto.text = "It's Over... Você perdeu!";
        }
    }

    // Função de exemplo só para teste (simula sucesso ou fracasso aleatório)
    bool TestarResultado()
    {
        return Random.value > 0.5f;  // 50% de chance de dar true ou false
    }
}


//FindObjectOfType<MostrarImagem>().MostrarMensagem(true);  // Venceu
//FindObjectOfType<MostrarImagem>().MostrarMensagem(false); // Perdeu
