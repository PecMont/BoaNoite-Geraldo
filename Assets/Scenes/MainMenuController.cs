using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas
using System.Collections;          // Necessário para a Coroutine

public class MainMenuController : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("O nome EXATO do arquivo da sua cena de jogo.")]
    public string gameSceneName;

    [Tooltip("O componente AudioSource que tocará o som do clique.")]
    public AudioSource clickSound;

    /// <summary>
    /// Esta é a função pública que o botão irá chamar.
    /// </summary>
    public void StartGame()
    {
        Debug.Log("StartGame() foi chamado!");
        // Inicia a rotina que toca o som e depois carrega a cena.
        StartCoroutine(PlaySoundAndLoadScene());
        SceneManager.LoadScene(1);
    }

    private IEnumerator PlaySoundAndLoadScene()
    {
        Debug.Log("Coroutine PlaySoundAndLoadScene iniciada.");

        // Garante que o jogo não está pausado.
        Time.timeScale = 1f;

        if (clickSound != null && clickSound.clip != null)
        {
            Debug.Log("AudioSource e AudioClip encontrados. Tocando som.");
            // Toca o som de clique.
            clickSound.Play();

            // Espera o tempo exato da duração do clipe de áudio, ignorando o Time.timeScale.
            yield return new WaitForSecondsRealtime(clickSound.clip.length);
            Debug.Log("Espera pelo som finalizada.");
        }
        else
        {
            Debug.LogWarning("AudioSource ou AudioClip não está configurado no Inspector. O som não será tocado.");
            // Mesmo sem som, esperamos um pequeno instante para garantir que a UI responda.
            yield return new WaitForSecondsRealtime(0.1f);
        }

        Debug.Log("Tentando carregar a cena com nome: " + gameSceneName);
        // Carrega a cena do jogo pelo seu nome.
        SceneManager.LoadScene(gameSceneName);
        Debug.Log("Comando para carregar a cena foi enviado. Se a cena não carregar, verifique o Build Settings.");
    }
}