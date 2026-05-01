using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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

    // ======= Sound System =======
    [Header("Sound - Footstep")]
    public AudioClip[] footstepClips;          // หลาย clip สำหรับสุ่มเสียงเดิน
    public float footstepInterval = 0.35f;     // ความถี่เสียงเดิน (วินาที)

    [Header("Sound - Jump / Land")]
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Sound - Climb")]
    public AudioClip[] climbStepClips;         // เสียงปีนบันได
    public float climbStepInterval = 0.4f;

    [Header("Sound - Grapple")]
    public AudioClip grappleShootClip;         // เสียงยิงสลิง
    public AudioClip grappleAttachClip;        // เสียงสลิงติด
    public AudioClip grappleReleaseClip;       // เสียงปล่อยสลิง

    [Header("Sound - Block / Shield")]
    public AudioClip blockStartClip;           // เสียงเริ่มใช้โล่
    public AudioClip blockEndClip;             // เสียงเลิกโล่

    [Header("Sound - Hurt / Death")]
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [Header("Sound - Audio Sources")]
    public AudioSource sfxSource;              // one-shot effects
    public AudioSource loopSource;             // looping sounds (ถ้าต้องการ)
    public AudioMixerGroup sfxMixerGroup;      // ลาก SFX Group จาก AudioMixer ใส่ตรงนี้

    [Header("Sound - Volume")]
    [Range(0f, 1f)] public float footstepVolume = 0.5f;
    [Range(0f, 1f)] public float climbVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // ---- internal sound state ----
    private float footstepTimer = 0f;
    private float climbStepTimer = 0f;
    private bool wasGrounded = false;
    private bool wasMovementLocked = false;    // track block state change
    private bool wasGrappling = false;         // track grapple state change

    // ======= Original Fields =======
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

    // ====================================================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Auto-create AudioSources ถ้าไม่ได้ assign ใน Inspector
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.playOnAwake = false;
            loopSource.loop = true;
        }

        // เชื่อม AudioSource เข้า SFX Mixer Group
        if (sfxMixerGroup != null)
        {
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            loopSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    void Update()
    {
        animator.SetBool("isBlocking", isMovementLocked);

        // ---- เสียงโล่ (Block) ----
        HandleBlockSound();

        // ---- เสียงสลิง (Grapple) ----
        HandleGrappleSound();

        if (isDead || isGrappling || isMovementLocked) return;

        bool groundedThisFrame = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (groundedThisFrame)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        isGrounded = coyoteTimeCounter > 0f;

        // ---- เสียงลงพื้น (Land) ----
        if (groundedThisFrame && !wasGrounded
            && rb.linearVelocity.y < -2f
            && !isOnMovingPlatform)
            PlaySound(landClip, sfxVolume);
        wasGrounded = groundedThisFrame;

        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetBool("isWalking", moveInput != 0 && groundedThisFrame);
        animator.SetBool("isJumping", !isGrounded && !isClimbing && rb.linearVelocity.y > 0.1f);
        animator.SetBool("isFalling", !groundedThisFrame && !isClimbing
                                      && rb.linearVelocity.y < -0.5f
                                      && !isOnMovingPlatform);
        animator.SetBool("isClimbing", isClimbing);

        // ---- เสียงเดิน (Footstep) ----
        HandleFootstepSound(groundedThisFrame);

        // ---- เสียงปีนบันได (Climb Step) ----
        HandleClimbSound();

        if (canClimb && Input.GetKeyDown(KeyCode.W))
        {
            isClimbing = true;
            Ladder ladder = FindObjectOfType<Ladder>();
            if (ladder != null)
                ladder.DisableGroundCollision(GetComponent<Collider2D>());
        }

        // ---- เสียงกระโดด (Jump) ----
        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0f;
            PlaySound(jumpClip, sfxVolume);
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

    // ======= Sound Helpers =======

    /// <summary>เล่น AudioClip แบบ one-shot ผ่าน sfxSource</summary>
    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    /// <summary>สุ่มเล่น clip จาก array</summary>
    private void PlayRandomSound(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0 || sfxSource == null) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    private void HandleFootstepSound(bool groundedThisFrame)
    {
        bool isMoving = Mathf.Abs(moveInput) > 0.01f && groundedThisFrame && !isClimbing;
        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayRandomSound(footstepClips, footstepVolume);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // reset ให้เสียงแรกเล่นทันทีเมื่อเริ่มเดินใหม่
        }
    }

    private void HandleClimbSound()
    {
        if (!isClimbing) { climbStepTimer = 0f; return; }

        float climbInput = 0f;
        if (Input.GetKey(KeyCode.W)) climbInput = 1f;
        if (Input.GetKey(KeyCode.S)) climbInput = -1f;

        if (Mathf.Abs(climbInput) > 0.01f)
        {
            climbStepTimer -= Time.deltaTime;
            if (climbStepTimer <= 0f)
            {
                PlayRandomSound(climbStepClips, climbVolume);
                climbStepTimer = climbStepInterval;
            }
        }
        else
        {
            climbStepTimer = 0f;
        }
    }

    private void HandleBlockSound()
    {
        // เสียงเริ่มบล็อก
        if (isMovementLocked && !wasMovementLocked)
            PlaySound(blockStartClip, sfxVolume);
        // เสียงเลิกบล็อก
        else if (!isMovementLocked && wasMovementLocked)
            PlaySound(blockEndClip, sfxVolume);

        wasMovementLocked = isMovementLocked;
    }

    private void HandleGrappleSound()
    {
        // ไม่ใช้แล้ว — เสียงสลิงถูกเรียกจาก GrapplePoint โดยตรง
    }

    /// <summary>เรียกจาก GrapplePoint.StartGrapple() — เสียงยิงสลิงออกไป</summary>
    public void OnGrappleStart()
    {
        PlaySound(grappleShootClip, sfxVolume);
    }

    /// <summary>เรียกจาก GrapplePoint หลังสลิงติดเป้าหมาย (optional)</summary>
    public void OnGrappleAttach()
    {
        PlaySound(grappleAttachClip, sfxVolume);
    }

    /// <summary>เรียกจาก GrapplePoint.StopGrapple() — เสียงปล่อยสลิง</summary>
    public void OnGrappleStop()
    {
        PlaySound(grappleReleaseClip, sfxVolume);
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
        {
            PlaySound(hurtClip, sfxVolume);
            StartCoroutine(InvincibleCoroutine());
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        PlaySound(deathClip, sfxVolume);
        Debug.Log("Player died!");

        // Destroy FlyingEnemy ทุกตัวในแมพ
        foreach (FlyingEnemy enemy in FindObjectsByType<FlyingEnemy>(FindObjectsSortMode.None))
            Destroy(enemy.gameObject);

        PlayerRespawn respawn = GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.Respawn();

        // snap กล้องไปที่ player หลัง respawn ทันที
        CameraFollow cam = FindFirstObjectByType<CameraFollow>();
        if (cam != null) cam.SnapToTarget();

        currentHealth = maxHealth;
        isDead = false;
    }

    IEnumerator InvincibleCoroutine()
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