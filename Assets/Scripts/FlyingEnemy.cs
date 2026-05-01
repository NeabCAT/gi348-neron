using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Chase Settings")]
    public float flySpeed = 3f;

    [Header("Spawn Settings")]
    public float spawnDuration = 1.5f;

    [Header("Destroy Settings")]
    public string destroyStateName = "isDestroy";

    [Header("Sound")]
    public AudioClip spawnClip;                  // เสียงตอน spawn
    public AudioClip flyLoopClip;                // เสียงบินวนซ้ำ
    public AudioClip destroyClip;                // เสียงตาย
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public AudioMixerGroup sfxMixerGroup;
    public float soundMinDistance = 2f;
    public float soundMaxDistance = 8f;

    private Transform player;
    private bool isBlocked = false;
    private bool isSpawning = true;
    private bool isDestroying = false;
    private Animator animator;
    public System.Action onDestroyComplete;

    private AudioSource sfxSource;      // one-shot (spawn, destroy)
    private AudioSource loopSource;     // loop (บิน)
    private Transform audioChildTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isSpawning", true);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // child GameObject สำหรับ 3D Audio
        GameObject audioChild = new GameObject("FlyingEnemySFX");
        audioChild.transform.SetParent(transform);
        audioChild.transform.localPosition = Vector3.zero;
        audioChildTransform = audioChild.transform;

        // sfxSource — one-shot (spawn / destroy)
        sfxSource = audioChild.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 1f;
        sfxSource.rolloffMode = AudioRolloffMode.Linear;
        sfxSource.minDistance = soundMinDistance;
        sfxSource.maxDistance = soundMaxDistance;
        if (sfxMixerGroup != null)
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;

        // loopSource — บิน (loop)
        loopSource = audioChild.AddComponent<AudioSource>();
        loopSource.playOnAwake = false;
        loopSource.loop = true;
        loopSource.spatialBlend = 1f;
        loopSource.rolloffMode = AudioRolloffMode.Linear;
        loopSource.minDistance = soundMinDistance;
        loopSource.maxDistance = soundMaxDistance;
        loopSource.clip = flyLoopClip;
        if (sfxMixerGroup != null)
            loopSource.outputAudioMixerGroup = sfxMixerGroup;

        // 🔊 เสียง spawn
        if (spawnClip != null)
            sfxSource.PlayOneShot(spawnClip, sfxVolume);

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDuration);
        isSpawning = false;
        animator.SetBool("isSpawning", false);

        // 🔊 เริ่มเสียงบินหลัง spawn จบ
        if (flyLoopClip != null)
            loopSource.Play();
    }

    void Update()
    {
        if (isSpawning || isDestroying) return;
        if (player == null || isBlocked) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * flySpeed * Time.deltaTime);

        if (dir.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(dir.x) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroying) return;

        if (other.CompareTag("EnemyBoundary"))
        {
            isBlocked = true;
            TriggerDestroy();
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (isSpawning) return;
            Player p = other.GetComponent<Player>();
            if (p == null) return;
            p.TakeDamage(p.GetCurrentHealth());
            TriggerDestroy();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBoundary"))
            isBlocked = false;
    }

    void TriggerDestroy()
    {
        if (isDestroying) return;
        isDestroying = true;
        isBlocked = true;

        // 🔊 หยุดเสียงบิน + เล่นเสียงตาย
        loopSource.Stop();
        if (destroyClip != null)
            sfxSource.PlayOneShot(destroyClip, sfxVolume);

        animator.SetBool("isSpawning", false);
        animator.SetTrigger("isDestroy");
        StartCoroutine(WaitForDestroyAnimation());
    }

    IEnumerator WaitForDestroyAnimation()
    {
        float elapsed = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(destroyStateName))
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 1f) break;
            yield return null;
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length + 0.5f);

        onDestroyComplete?.Invoke();
        Destroy(gameObject);
    }
}