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
        HandleHit(col.gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        HandleHit(col.gameObject);
    }

    void HandleHit(GameObject hit)
    {
        GameObject root = hit.transform.root.gameObject;

        if (root.CompareTag("Enemy"))
        {
            // หา EnemyContact ทั้งบน root และ child
            EnemyContact enemyContact = root.GetComponent<EnemyContact>();
            if (enemyContact == null)
                enemyContact = root.GetComponentInChildren<EnemyContact>();

            if (enemyContact != null && !enemyContact.isDead)
                enemyContact.StartCoroutine(enemyContact.DeathRoutine());
            else if (enemyContact == null)
                Destroy(root);

            Destroy(gameObject);
            return;
        }

        if (hit.CompareTag("Bullet")) { Destroy(hit); return; }
        if (hit.GetComponent<CameraZone>() != null) return;
        if (hit.GetComponent<PlatformTriggerZone>() != null) return;
        if (hit.GetComponent<MusicZone>() != null) return;

        Destroy(gameObject);
    }
}