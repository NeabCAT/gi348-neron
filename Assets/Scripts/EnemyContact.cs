using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [Header("Stomp Settings")]
    public float stompBounceForce = 8f;
    public Transform headCheck;          // Empty Object ไว้บนหัว Enemy
    public float headCheckRadius = 0.2f;
    public LayerMask playerLayer;        // Layer ของ Player

    void Update()
    {
        // เช็คว่า Player อยู่บนหัวและกำลังตก
        Collider2D hit = Physics2D.OverlapCircle(headCheck.position, headCheckRadius, playerLayer);
        if (hit != null)
        {
            Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
            Player player = hit.GetComponent<Player>();

            if (playerRb.linearVelocity.y <= 0)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
            player.TakeDamage(player.GetCurrentHealth());
    }

    void OnDrawGizmosSelected()
    {
        if (headCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}