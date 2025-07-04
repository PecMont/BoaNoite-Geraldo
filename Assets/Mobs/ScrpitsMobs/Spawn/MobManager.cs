using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public List<SpawnRule> spawnRules;

    void OnEnable()
    {
        GameProgression.OnProgressoChanged += CheckSpawnRules;
    }

    void OnDisable()
    {
        GameProgression.OnProgressoChanged -= CheckSpawnRules;
    }

    private void CheckSpawnRules(int currentProgress)
    {
        foreach (var rule in spawnRules)
        {
            if (currentProgress >= rule.requiredProgress && !rule.isMobActive)
            {
                if (IsSpawnPointFree(rule.spawnPoint))
                {
                    SpawnMob(rule);
                }
            }
        }
    }

    private void SpawnMob(SpawnRule rule)
    {
        Debug.Log($"Spawning mob '{rule.mobPrefab.name}' no local '{rule.spawnPoint.name}' por atingir o progresso {rule.requiredProgress}.");
        GameObject mobInstance = Instantiate(rule.mobPrefab, rule.spawnPoint.position, rule.spawnPoint.rotation);
        MobController controller = mobInstance.GetComponent<MobController>();
        if (controller != null)
        {
            controller.Setup(this, rule);
        }
        else
        {
            Debug.LogError($"ERRO: O prefab '{rule.mobPrefab.name}' não tem o script MobController.cs anexado!");
        }
        rule.isMobActive = true;
        rule.activeMobInstance = mobInstance;
        GameProgression.instance.InimigosAtivos++;
    }

    public void OnMobDied(SpawnRule rule)
    {
        if (!rule.isMobActive) return;

        Debug.Log($"Mob da regra '{rule.ruleName}' foi removido do jogo. Verificando se deve respawnar...");
        rule.isMobActive = false;
        rule.activeMobInstance = null;
        GameProgression.instance.InimigosAtivos--;

        if (rule.respawnDelay <= 0)
        {
            Debug.Log($"Respawn para '{rule.ruleName}' está desativado.");
            return;
        }

        if (rule.deathProgress > 0 && GameProgression.instance.Progresso >= rule.deathProgress)
        {
            Debug.Log($"Mob da regra '{rule.ruleName}' não vai respawnar. O progresso de morte ({rule.deathProgress}) foi atingido.");
            return;
        }

        Debug.Log($"Agendando respawn para '{rule.ruleName}' em {rule.respawnDelay} segundos.");
        StartCoroutine(RespawnCoroutine(rule));
    }

    private IEnumerator RespawnCoroutine(SpawnRule rule)
    {
        yield return new WaitForSeconds(rule.respawnDelay);

        if (rule.deathProgress > 0 && GameProgression.instance.Progresso >= rule.deathProgress)
        {
            Debug.Log($"Respawn para '{rule.ruleName}' foi cancelado. O progresso de morte foi atingido durante a espera.");
            yield break;
        }
        
        if (!IsSpawnPointFree(rule.spawnPoint))
        {
            Debug.Log($"Respawn para '{rule.ruleName}' foi cancelado. O ponto de spawn foi ocupado.");
            yield break;
        }
        
        Debug.Log($"Tempo de espera finalizado! Respawnando mob da regra '{rule.ruleName}'.");
        SpawnMob(rule);
    }
    
    private bool IsSpawnPointFree(Transform spawnPoint)
    {
        foreach (var rule in spawnRules)
        {
            if (rule.isMobActive && rule.spawnPoint == spawnPoint)
            {
                return false;
            }
        }
        return true;
    }
}
