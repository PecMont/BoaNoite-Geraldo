using UnityEngine;
using System; // Necessário para usar 'Action'

public class GameProgression : MonoBehaviour
{
    public static GameProgression instance;

    // Use uma propriedade para que possamos acionar o evento sempre que o valor mudar.
    private int _progresso = 0;
    public int Progresso
    {
        get { return _progresso; }
        set
        {
            if (_progresso != value)
            {
                _progresso = value;
                // Dispara o evento avisando que o progresso mudou!
                OnProgressoChanged?.Invoke(_progresso);
            }
        }
    }

    public int InimigosAtivos = 0;

    // Evento estático que outros scripts podem "escutar"
    public static event Action<int> OnProgressoChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantém o progresso entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Exemplo de como você pode aumentar o progresso no seu jogo
    // Chame esta função quando o jogador completar uma tarefa importante.
    public void AvancarProgresso()
    {
        Progresso++;
        Debug.Log("Progresso do Jogo avançou para: " + Progresso);
    }
}