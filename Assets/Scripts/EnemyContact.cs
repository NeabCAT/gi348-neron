using System.Collections;
using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [Header("Stomp Settings")]
    public float stompBounceForce = 8f;
    public Transform headCheck;
    public float headCheckRadius = 0.2f;
    public LayerMask playerLayer;

    [Header("Death Animation")]
    public string deadAnimationName = "Dead";

    private Animator animator;
    public bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isDead) return;
        Collider2D hit = Physics2D.OverlapCircle(headCheck.position, headCheckRadius, playerLayer);
        if (hit != null)
        {
            Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
            if (playerRb.linearVelocity.y <= 0)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
                StartCoroutine(DeathRoutine());
            }
        }
    }

    public IEnumerator DeathRoutine()
    {
        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        GetComponent<Collider2D>().enabled = false;

        if (animator != null)
            animator.SetBool("isDead", true);

        yield return new WaitForSeconds(GetAnimationLength(deadAnimationName));
        Destroy(gameObject);
    }

    float GetAnimationLength(string animName)
    {
        if (animator == null) return 0.5f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 0.5f;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead) return;
        if (!col.gameObject.CompareTag("Player")) return;
        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
            player.TakeDamage(player.GetCurrentHealth());
    }

    void OnDrawGizmosSelected()
    {
        if (headCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}