using UnityEngine;

public class ShooterAI : MonoBehaviour
{
    [Header("Shoot Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float shootRange = 5f;

    [Header("Facing")]
    public float facingDirection = 1f;

    private float fireTimer = 0f;
    private float dir;
    private EnemyContact enemyContact;
    private Animator animator;

    void Start()
    {
        dir = facingDirection >= 0 ? 1f : -1f;
        enemyContact = GetComponent<EnemyContact>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (enemyContact != null && enemyContact.isDead)
        {
            if (animator != null)
            {
                animator.SetBool("isShooting", false);
                animator.SetBool("isDead", true);
            }
            return;
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            if (animator != null)
                animator.SetBool("isShooting", true);

            fireTimer = 0f;
        }
    }


    // เรียกจาก Animation Event ตรงเฟรมที่ยิง
    public void SpawnBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;
        if (enemyContact != null && enemyContact.isDead) return;

        if (animator != null)
            animator.SetBool("isShooting", false); // ปิดหลังยิง

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