using UnityEngine;

public class TriggerPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public Transform platform;          // ลาก platform มาใส่
    public Transform targetPoint;       // ลาก target point มาใส่
    public float moveSpeed = 3f;

    [Header("Sound")]
    public AudioClip moveClip;          // เสียงตอนเลื่อน
    [Range(0f, 1f)] public float volume = 1f;

    private bool triggered = false;
    private bool isMoving = false;
    private AudioSource audioSource;

    void Start()
    {
        // สร้าง AudioSource สำหรับเสียง loop ตอนเลื่อน
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = moveClip;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        isMoving = true;

        if (moveClip != null)
            audioSource.Play();
    }

    void Update()
    {
        if (!isMoving || platform == null || targetPoint == null) return;

        platform.position = Vector3.MoveTowards(
            platform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        // ถึงจุดหมายแล้ว
        if (Vector3.Distance(platform.position, targetPoint.position) < 0.01f)
        {
            platform.position = targetPoint.position;
            isMoving = false;

            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    void OnDrawGizmos()
    {
        if (platform == null || targetPoint == null) return;

        // เส้นจาก platform ไป target
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(platform.position, targetPoint.position);

        // วงกลมที่ target
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPoint.position, 0.2f);

        // วงกลมที่ platform
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(platform.position, 0.2f);
    }
}