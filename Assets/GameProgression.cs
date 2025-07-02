using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public static GameProgression instance;

    public int Progresso = 0;
    public int InimigosAtivos = 0;

    // A função Awake() é chamada antes de qualquer Start().
    void Awake()
    {
        // 3. Lógica do Singleton
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
