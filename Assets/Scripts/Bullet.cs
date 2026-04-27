using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    private int direction = 1;
    private float range;
    private Vector3 startPos;

    public void Init(int dir, float range)
    {
        direction = dir;
        this.range = range;
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        if (Vector2.Distance(startPos, transform.position) >= range)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy")) return;
        if (col.CompareTag("Bullet")) return;
        //if (col.CompareTag("Shield")) return; // ให้ ShieldCollider จัดการแทน
        if (col.GetComponent<CameraZone>() != null) return;
        if (col.GetComponent<PlatformTriggerZone>() != null) return;

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy")) return;
        if (col.gameObject.CompareTag("Bullet")) return;
        Destroy(gameObject);
    }
}