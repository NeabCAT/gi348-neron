using UnityEngine;

public class ReflectBullet : MonoBehaviour
{
    public float speed = 12f;
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
        if (col.CompareTag("Enemy"))
        {
            Destroy(col.gameObject); // ฆ่า Enemy ทันที
            Destroy(gameObject);
            return;
        }

        if (col.CompareTag("Bullet")) return;
        if (col.GetComponent<CameraZone>() != null) return;
        if (col.GetComponent<PlatformTriggerZone>() != null) return;

        Destroy(gameObject);
    }
}