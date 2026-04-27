using UnityEngine;

public class EnemyKillPlayer : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = col.gameObject.GetComponent<Rigidbody2D>();

        foreach (ContactPoint2D contact in col.contacts)
        {
            // ถ้าชนจากด้านบน = เหยียบหัว ไม่ตาย
            if (contact.normal.y > 0.5f) return;
        }

        // ชนด้านข้างหรือด้านล่าง = ตายทันที
        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
            player.TakeDamage(player.GetCurrentHealth());
    }
}