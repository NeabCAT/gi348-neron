using UnityEngine;

public class ShooterAI : MonoBehaviour
{
    [Header("Shoot Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float shootRange = 5f;

    [Header("Facing")]
    public float facingDirection = 1f; // 1 = ขวา, -1 = ซ้าย

    private float fireTimer = 0f;
    private Vector3 originalScale;
    private float dir;

    void Start()
    {
        dir = facingDirection >= 0 ? 1f : -1f;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(dir > 0 ? 1 : -1, shootRange);
    }

    void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(firePoint.position, Vector2.right * (facingDirection >= 0 ? 1f : -1f) * shootRange);
    }
}