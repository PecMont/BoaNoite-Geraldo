using System.Collections; // Necessário para usar Corrotinas
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

    // --- LÓGICA DE MORTE E RESPAWN ATUALIZADA ---
    public void OnMobDied(SpawnRule rule)
    {
        if (!rule.isMobActive) return; // Evita chamadas duplas

        Debug.Log($"Mob da regra '{rule.ruleName}' foi removido do jogo. Verificando se deve respawnar...");
        
        // Limpa o estado do mob morto
        rule.isMobActive = false;
        rule.activeMobInstance = null;
        GameProgression.instance.InimigosAtivos--;

        // Verifica se o respawn está desativado (delay <= 0)
        if (rule.respawnDelay <= 0)
        {
            Debug.Log($"Respawn para '{rule.ruleName}' está desativado.");
            return;
        }

        // Verifica se o progresso de morte permanente foi atingido
        if (rule.deathProgress > 0 && GameProgression.instance.Progresso >= rule.deathProgress)
        {
            Debug.Log($"Mob da regra '{rule.ruleName}' não vai respawnar. O progresso de morte ({rule.deathProgress}) foi atingido.");
            return;
        }

        // Se todas as verificações passaram, inicia a Corrotina de respawn
        Debug.Log($"Agendando respawn para '{rule.ruleName}' em {rule.respawnDelay} segundos.");
        StartCoroutine(RespawnCoroutine(rule));
    }

    // --- NOVA CORROTINA DE RESPAWN ---
    private IEnumerator RespawnCoroutine(SpawnRule rule)
    {
        // 1. Espera o tempo definido na regra
        yield return new WaitForSeconds(rule.respawnDelay);

        // 2. Faz uma verificação final ANTES de respawnar.
        // (O jogador pode ter atingido o deathProgress durante a espera)
        if (rule.deathProgress > 0 && GameProgression.instance.Progresso >= rule.deathProgress)
        {
            Debug.Log($"Respawn para '{rule.ruleName}' foi cancelado. O progresso de morte foi atingido durante a espera.");
            yield break; // Cancela a corrotina
        }
        
        // 3. Verifica se o local ainda está livre
        if (!IsSpawnPointFree(rule.spawnPoint))
        {
            Debug.Log($"Respawn para '{rule.ruleName}' foi cancelado. O ponto de spawn foi ocupado.");
            // Opcional: você poderia tentar de novo depois de um tempo aqui
            yield break; // Cancela a corrotina
        }
        
        // 4. Se tudo estiver ok, spawna o mob novamente
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