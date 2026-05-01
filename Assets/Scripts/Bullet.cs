using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    private int direction = 1;
    private float range;
    private Vector3 startPos;
    private GameObject owner; // เก็บว่าใครยิง

    public void Init(int dir, float range, GameObject owner = null)
    {
        direction = dir;
        this.range = range;
        this.owner = owner;
        startPos = transform.position;

        // ให้ Bullet เมิน Layer ของ Enemy ทั้งหมดเลย
        Physics2D.IgnoreLayerCollision(
            gameObject.layer,                // Layer ของ Bullet
            LayerMask.NameToLayer("Enemy"),  // ชื่อ Layer ของ Enemy ใน Unity
            true
        );
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        if (Vector2.Distance(startPos, transform.position) >= range)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == owner) return; // เมินคนยิง
        if (col.CompareTag("Enemy")) return;
        if (col.CompareTag("Bullet")) return;
        if (col.CompareTag("MusicZone")) return; // เพิ่มตรงนี้
        if (col.GetComponent<CameraZone>() != null) return;
        if (col.GetComponent<PlatformTriggerZone>() != null) return;
        if (col.GetComponent<MusicZone>() != null) return;

        if (col.CompareTag("Player"))
        {
            Player player = col.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(player.GetCurrentHealth());
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject == owner) return; // เมินคนยิง
        if (col.gameObject.CompareTag("Enemy")) return;
        if (col.gameObject.CompareTag("Bullet")) return;

        if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(player.GetCurrentHealth());
        }
        Destroy(gameObject);
    }
}