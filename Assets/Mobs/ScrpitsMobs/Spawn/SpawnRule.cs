using UnityEngine;

[System.Serializable]
public class SpawnRule
{
    public string ruleName;
    public GameObject mobPrefab;
    public Transform spawnPoint;
    public int requiredProgress;
    public int deathProgress;
    public float respawnDelay = 90f;

    [HideInInspector]
    public bool isMobActive = false;

    [HideInInspector]
    public GameObject activeMobInstance = null;
}
