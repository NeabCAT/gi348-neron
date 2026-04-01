using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Respawn()
    {
        transform.position = SpawnManager.Instance.GetRespawnPosition();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log("Player respawned!");
    }
}