using UnityEngine;

public class MobController : MonoBehaviour
{
    private MobManager _manager;
    private SpawnRule _myRule;
    private float timeToLive = 180f;

    public void Setup(MobManager manager, SpawnRule rule)
    {
        _manager = manager;
        _myRule = rule;
        Invoke("DestroyDueToTime", timeToLive);

        if (_myRule.deathProgress > 0)
        {
            GameProgression.OnProgressoChanged += CheckProgressDeathCondition;
        }
    }

    private void DestroyDueToTime()
    {
        Debug.Log($"Mob '{gameObject.name}' está se autodestruindo por tempo (3 minutos).");
        Destroy(gameObject);
    }

    private void CheckProgressDeathCondition(int newProgress)
    {
        if (newProgress >= _myRule.deathProgress)
        {
            Debug.Log($"Mob '{gameObject.name}' está se autodestruindo porque o progresso {_myRule.deathProgress} foi atingido.");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_myRule != null && _myRule.deathProgress > 0)
        {
            GameProgression.OnProgressoChanged -= CheckProgressDeathCondition;
        }

        if (_manager != null)
        {
            _manager.OnMobDied(_myRule);
        }
    }
}
