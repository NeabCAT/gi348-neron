// TriggerSpawner.cs
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned) return;
        if (!other.CompareTag("Player")) return;

        hasSpawned = true;

        GameObject spawned = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        FlyingEnemy enemy = spawned.GetComponent<FlyingEnemy>();

        if (enemy != null)
            enemy.onDestroyComplete += () => hasSpawned = false;
    }
}