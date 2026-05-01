using UnityEngine;
using UnityEngine.Audio;

public class ShooterAI : MonoBehaviour
{
    [Header("Shoot Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float shootRange = 5f;

    [Header("Facing")]
    public float facingDirection = 1f;

    [Header("Sound - Shoot")]
    public AudioClip shootClip;
    [Range(0f, 1f)] public float shootVolume = 1f;
    public AudioMixerGroup sfxMixerGroup;
    public float soundMinDistance = 2f;
    public float soundMaxDistance = 10f;

    private float fireTimer = 0f;
    private float dir;
    private EnemyContact enemyContact;
    private Animator animator;
    private AudioSource sfxSource;
    private Transform audioChildTransform;

    void Start()
    {
        dir = facingDirection >= 0 ? 1f : -1f;
        enemyContact = GetComponent<EnemyContact>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // สร้าง child GameObject แยกสำหรับ AudioSource
        // parent (enemy) จะไม่ถูกขยับเลย
        GameObject audioChild = new GameObject("ShooterSFX");
        audioChild.transform.SetParent(transform);
        audioChild.transform.localPosition = Vector3.zero;
        audioChildTransform = audioChild.transform;

        sfxSource = audioChild.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 1f;
        sfxSource.rolloffMode = AudioRolloffMode.Linear;
        sfxSource.minDistance = soundMinDistance;
        sfxSource.maxDistance = soundMaxDistance;
        if (sfxMixerGroup != null)
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
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

    void LateUpdate()
    {
        // ขยับแค่ child AudioSource ให้ Z ตรงกับ Camera
        // parent (enemy) ไม่ถูกแตะเลย ดังนั้น enemy ยังแสดงผลปกติ
        if (audioChildTransform == null || Camera.main == null) return;

        audioChildTransform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            Camera.main.transform.position.z
        );
    }

    // เรียกจาก Animation Event ตรงเฟรมที่ยิง
    public void SpawnBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;
        if (enemyContact != null && enemyContact.isDead) return;

        if (animator != null)
            animator.SetBool("isShooting", false);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(dir > 0 ? 1 : -1, shootRange, gameObject);

        // 🔊 เสียงยิง
        if (shootClip != null)
            sfxSource.PlayOneShot(shootClip, shootVolume);
    }

    void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(firePoint.position, Vector2.right * (facingDirection >= 0 ? 1f : -1f) * shootRange);
    }
}