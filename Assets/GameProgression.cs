using UnityEngine;
using System;

public class GameProgression : MonoBehaviour
{
    public static GameProgression instance;

    private int _progresso = 0;
    public int Progresso
    {
        get { return _progresso; }
        set
        {
            if (_progresso != value)
            {
                _progresso = value;
                OnProgressoChanged?.Invoke(_progresso);
            }
        }
    }

    public int InimigosAtivos = 0;

    public static event Action<int> OnProgressoChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AvancarProgresso()
    {
        Progresso++;
        Debug.Log("Progresso do Jogo avançou para: " + Progresso);
    }
}
