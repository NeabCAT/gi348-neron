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
    public float invincibleDuration = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public float coyoteTime = 0.15f;

    private float coyoteTimeCounter;
    private Rigidbody2D rb;
    public bool isGrounded;
    private float moveInput;
    private Vector3 originalScale;

    public bool isGrappling = false;
    public bool canClimb = false;
    public bool isClimbing = false;
    private Animator animator;
    private bool isMovementLocked = false;

    [HideInInspector] public bool climbCanGoUp = false;
    [HideInInspector] public bool climbCanGoDown = false;

    private bool isOnMovingPlatform = false;
    private MovingPlatform currentPlatform = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("isBlocking", isMovementLocked);
        if (isDead || isGrappling || isMovementLocked) return;

        bool groundedThisFrame = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (groundedThisFrame)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        isGrounded = coyoteTimeCounter > 0f;

        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetBool("isWalking", moveInput != 0 && groundedThisFrame);
        animator.SetBool("isJumping", !isGrounded && !isClimbing && rb.linearVelocity.y > 0.1f);
        animator.SetBool("isFalling", !groundedThisFrame && !isClimbing
                                      && rb.linearVelocity.y < -0.5f
                                      && !isOnMovingPlatform);
        animator.SetBool("isClimbing", isClimbing);

        if (canClimb && Input.GetKeyDown(KeyCode.W))
        {
            isClimbing = true;
            Ladder ladder = FindObjectOfType<Ladder>();
            if (ladder != null)
                ladder.DisableGroundCollision(GetComponent<Collider2D>());
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }

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
        if (isDead || isGrappling || isMovementLocked) return;

        float platformVelX = currentPlatform != null ? currentPlatform.PlatformVelocity.x : 0f;

        if (isClimbing)
        {
            float climbInput = 0f;
            if (Input.GetKey(KeyCode.W)) climbInput = 1f;
            if (Input.GetKey(KeyCode.S)) climbInput = -1f;
            rb.linearVelocity = new Vector2(0f, climbInput * climbSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed + platformVelX, rb.linearVelocity.y);
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

        Debug.Log("Player died!");

        PlayerRespawn respawn = GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.Respawn();

        currentHealth = maxHealth;
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
        if (col.gameObject.CompareTag("Enemy"))
        {
            StompEnemy stomp = col.gameObject.GetComponent<StompEnemy>();
            bool isAbove = transform.position.y > col.transform.position.y + 0.2f;
            bool isFalling = rb.linearVelocity.y < 0;

            if (stomp != null && isAbove && isFalling) return;

            TakeDamage(currentHealth);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<SpawnPoint>(out SpawnPoint sp))
        {
            SpawnManager.Instance.TryActivate(sp);
        }

        if (other.CompareTag("DeathZone"))
        {
            currentHealth = 1;
            TakeDamage(1);
        }
    }

    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;
        if (locked)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void SetOnMovingPlatform(bool value, MovingPlatform platform)
    {
        isOnMovingPlatform = value;
        currentPlatform = platform;
    }

    public int GetCurrentHealth() => currentHealth;
    public bool IsDead() => isDead;

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}