using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 3f;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    public float invincibleDuration = 1.5f; // วินาทีที่กันดาเมจหลังโดนตี

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private Vector3 originalScale;

    public bool isGrappling = false;
    public bool canClimb = false;
    public bool isClimbing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || isGrappling) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (canClimb && Input.GetKeyDown(KeyCode.W))
        {
            isClimbing = true;
            Ladder ladder = FindObjectOfType<Ladder>();
            if (ladder != null)
                ladder.DisableGroundCollision(GetComponent<Collider2D>());
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (moveInput != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveInput) * Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    void FixedUpdate()
    {
        if (isDead || isGrappling) return;

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            float climbInput = 0f;
            if (Input.GetKey(KeyCode.W)) climbInput = 1f;
            if (Input.GetKey(KeyCode.S)) climbInput = -1f;
            rb.linearVelocity = new Vector2(0f, climbInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    // ======= Health System =======

    public void TakeDamage(int amount)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        Debug.Log($"HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibleCoroutine());
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        Debug.Log("Player died!");

        // Respawn ผ่าน PlayerRespawn
        PlayerRespawn respawn = GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.Respawn();

        // Reset state หลัง respawn
        currentHealth = maxHealth;
        rb.gravityScale = 1f;
        isDead = false;
    }

    System.Collections.IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    // ======= Collision / Trigger =======

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;

        // ชนศัตรู
        if (col.gameObject.CompareTag("Enemy"))
            TakeDamage(1);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // แตะ SpawnPoint
        if (other.TryGetComponent<SpawnPoint>(out SpawnPoint sp))
        {
            SpawnManager.Instance.TryActivate(sp);
        }

        // ตก DeathZone
        if (other.CompareTag("DeathZone"))
        {
            currentHealth = 1;
            TakeDamage(1);
        }
    }

    // เรียกจากภายนอกได้ เช่น กับดัก กระสุน
    public int GetCurrentHealth() => currentHealth;
    public bool IsDead() => isDead;
}
