using UnityEngine;

public class MobController : MonoBehaviour
{
    private MobManager _manager;
    private SpawnRule _myRule;
    private float timeToLive = 180f; // 3 minutos em segundos

    // O MobManager chama esta função para configurar o mob assim que ele é criado
    public void Setup(MobManager manager, SpawnRule rule)
    {
        _manager = manager;
        _myRule = rule;

        // Inicia o contador de 3 minutos para autodestruição
        // A função DestroyDueToTime será chamada após 180 segundos.
        Invoke("DestroyDueToTime", timeToLive);

        // Se uma condição de morte por progresso foi definida na regra (maior que 0),
        // o mob começa a "escutar" as mudanças de progresso do jogo.
        if (_myRule.deathProgress > 0)
        {
            GameProgression.OnProgressoChanged += CheckProgressDeathCondition;
        }
    }

    // Função que destrói o mob por tempo
    private void DestroyDueToTime()
    {
        Debug.Log($"Mob '{gameObject.name}' está se autodestruindo por tempo (3 minutos).");
        Destroy(gameObject); // Destrói o GameObject deste mob
    }

    // Função que é chamada toda vez que o progresso do jogo muda
    private void CheckProgressDeathCondition(int newProgress)
    {
        // Se o novo progresso for igual ou maior ao progresso de morte definido na regra...
        if (newProgress >= _myRule.deathProgress)
        {
            Debug.Log($"Mob '{gameObject.name}' está se autodestruindo porque o progresso {_myRule.deathProgress} foi atingido.");
            Destroy(gameObject); // ...destrói o mob.
        }
    }

    // A função OnDestroy é chamada automaticamente pela Unity SEMPRE que este objeto é destruído
    // (seja pelo tempo, pelo progresso ou por qualquer outro motivo)
    void OnDestroy()
    {
        // É importante parar de escutar o evento para evitar erros de memória
        if (_myRule != null && _myRule.deathProgress > 0)
        {
            GameProgression.OnProgressoChanged -= CheckProgressDeathCondition;
        }

        // Avisa ao MobManager que este mob morreu, para que a regra seja liberada
        if (_manager != null)
        {
            _manager.OnMobDied(_myRule);
        }
    }
}