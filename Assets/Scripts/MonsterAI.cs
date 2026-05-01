using UnityEngine;
using UnityEngine.Audio;

public class MonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolDistance = 3f;
    public float moveSpeed = 2f;

    [Header("Sound - Footstep")]
    public AudioClip[] footstepClips;
    [Range(0f, 1f)] public float footstepVolume = 1f;
    public float footstepInterval = 0.4f;
    public AudioMixerGroup sfxMixerGroup;
    public float soundMinDistance = 2f;
    public float soundMaxDistance = 8f;

    private Vector2 startPosition;
    private int direction = 1;
    private Vector3 originalScale;
    private EnemyContact enemyContact;

    private AudioSource sfxSource;
    private Transform audioChildTransform;
    private float footstepTimer = 0f;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
        enemyContact = GetComponent<EnemyContact>();

        // child GameObject แยกสำหรับ 3D Audio (ไม่กระทบ parent)
        GameObject audioChild = new GameObject("SlimeSFX");
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
        if (enemyContact != null && enemyContact.isDead) return;

        // เดิน
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        float dist = transform.position.x - startPosition.x;
        if (dist >= patrolDistance)
            direction = -1;
        else if (dist <= -patrolDistance)
            direction = 1;

        transform.localScale = new Vector3(
            direction * Mathf.Abs(originalScale.x),
            originalScale.y,
            originalScale.z
        );

        // เสียงเดิน
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            PlayFootstep();
            footstepTimer = footstepInterval;
        }
    }

    void LateUpdate()
    {
        // sync Z ของ child ให้ตรงกับ Camera — parent ไม่โดนแตะ
        if (audioChildTransform == null || Camera.main == null) return;

        audioChildTransform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            Camera.main.transform.position.z
        );
    }

    void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        if (clip != null)
            sfxSource.PlayOneShot(clip, footstepVolume);
    }

    void OnDrawGizmos()
    {
        Vector2 origin = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Vector2 leftPoint = origin + Vector2.left * patrolDistance;
        Vector2 rightPoint = origin + Vector2.right * patrolDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawSphere(leftPoint, 0.1f);
        Gizmos.DrawSphere(rightPoint, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(origin, 0.1f);
    }
}