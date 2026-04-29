// FlyingEnemy.cs
using UnityEngine;
using System.Collections;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Chase Settings")]
    public float flySpeed = 3f;

    [Header("Spawn Settings")]
    public float spawnDuration = 1.5f;

    [Header("Destroy Settings")]
    public string destroyStateName = "isDestroy";

    private Transform player;
    private bool isBlocked = false;
    private bool isSpawning = true;
    private bool isDestroying = false;
    private Animator animator;

    public System.Action onDestroyComplete;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isSpawning", true);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDuration);
        isSpawning = false;
        animator.SetBool("isSpawning", false);
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

        animator.SetBool("isSpawning", false);
        animator.SetTrigger("isDestroy");

        StartCoroutine(WaitForDestroyAnimation());
    }

    IEnumerator WaitForDestroyAnimation()
    {
        // ✅ รอให้เข้า State isDestroy ก่อน (timeout 1 วิกันค้าง)
        float elapsed = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(destroyStateName))
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 1f) break;
            yield return null;
        }

        // ✅ รอ animation จบ ใช้ clip length + buffer เล็กน้อย
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length + 0.5f);

        onDestroyComplete?.Invoke();
        Destroy(gameObject);
    }
}