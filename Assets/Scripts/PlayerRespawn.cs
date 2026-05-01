using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpawnPoint currentSpawnPoint;
    private Vector3 defaultSpawnPosition; // ✅ จำตำแหน่งเริ่มต้นไว้

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultSpawnPosition = transform.position; // ✅ เก็บตำแหน่งแรกตอน Start
    }

    public void SetSpawnPoint(SpawnPoint newSpawnPoint)
    {
        if (currentSpawnPoint != null && currentSpawnPoint != newSpawnPoint)
            currentSpawnPoint.Deactivate();

        currentSpawnPoint = newSpawnPoint;
    }

    public void Respawn()
    {
        // ✅ ถ้ายังไม่มี spawn point ให้กลับไปจุดเริ่มต้นของ scene
        if (currentSpawnPoint == null)
        {
            transform.position = defaultSpawnPosition;
        }
        else
        {
            transform.position = currentSpawnPoint.transform.position;
            currentSpawnPoint.PlayRespawnSound();
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log("Player respawned!");
    }
}