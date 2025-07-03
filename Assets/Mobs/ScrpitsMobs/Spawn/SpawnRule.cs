using UnityEngine;

[System.Serializable]
public class SpawnRule
{
    public string ruleName;
    public GameObject mobPrefab;
    public Transform spawnPoint;
    public int requiredProgress;
    public int deathProgress;
    
    // NOVO: Tempo em segundos para o mob ressurgir após morrer.
    // 90 segundos = 1 minuto e meio.
    [Tooltip("Tempo em segundos para o mob ressurgir. Coloque 0 para não respawnar.")]
    public float respawnDelay = 90f;

    [HideInInspector]
    public bool isMobActive = false;
    
    [HideInInspector]
    public GameObject activeMobInstance = null;
}