using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToLoadScene : MonoBehaviour
{
    [Header("Configuração da Cena")]
    [Tooltip("Digite o nome EXATO da cena que deve ser carregada.")]
    public string sceneNameToLoad;
    public string gameSceneName;

    // Variável para controlar se o jogador está perto o suficiente
    private bool isPlayerNearby = false;

    // A função Update é chamada a cada frame
    private void Update()
    {
        // Se o jogador estiver na área de interação E apertar a tecla 'E'...
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Chama a função que usa o nome da cena, em vez do número.
            LoadScene();
        }
    }

    // Esta função é chamada automaticamente quando outro objeto com Collider
    // entra no nosso Trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Jogador entrou na área de interação do cubo.");
            // (Opcional) Aqui você pode mostrar uma mensagem na tela como "Pressione [E]"
        }
    }

    // Esta função é chamada quando o outro objeto sai do nosso Trigger.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Jogador saiu da área de interação.");
            // (Opcional) Aqui você pode esconder a mensagem da tela
        }
    }

    private void LoadScene()
    {
        // Verifica se o nome da cena foi preenchido no Inspector
        if (!string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.Log("Tecla 'E' pressionada! Carregando a cena: " + sceneNameToLoad);
            //SceneManager.LoadScene(2);
            SceneManager.LoadScene(sceneNameToLoad);
        }
        else
        {
            Debug.LogError("O nome da cena não foi definido no Inspector do objeto " + gameObject.name);
        }
    }
}