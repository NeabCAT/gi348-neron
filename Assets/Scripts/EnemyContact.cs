using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyContact : MonoBehaviour
{
    [Header("Stomp Settings")]
    public float stompBounceForce = 8f;
    public Transform headCheck;
    public float headCheckRadius = 0.2f;
    public LayerMask playerLayer;

    [Header("Death Animation")]
    public string deadAnimationName = "Dead";

    [Header("Sound - Stomp")]
    public AudioClip stompClip;              // เสียงตอนโดน stomp
    [Range(0f, 1f)] public float stompVolume = 1f;
    public AudioMixerGroup sfxMixerGroup;    // ลาก SFX Group ตัวเดิมใส่

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

        // 🔊 เสียง stomp — ใช้ PlayClipAtPoint เพราะ GameObject กำลังจะ Destroy
        PlayStompSound();

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

    void PlayStompSound()
    {
        if (stompClip == null) return;

        // PlayClipAtPoint สร้าง AudioSource ชั่วคราวที่ตำแหน่ง enemy
        // และ destroy ตัวเองหลังเสียงจบ — ไม่ขึ้นกับ GameObject นี้
        if (sfxMixerGroup != null)
        {
            // ถ้าต้องการให้ผ่าน Mixer ต้องสร้าง AudioSource temp เอง
            GameObject temp = new GameObject("StompSFX");
            temp.transform.position = transform.position;
            AudioSource src = temp.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxMixerGroup;
            src.clip = stompClip;
            src.volume = stompVolume;
            src.spatialBlend = 0f;   // 2D sound
            src.Play();
            Destroy(temp, stompClip.length + 0.1f);
        }
        else
        {
            // fallback: ไม่มี Mixer ก็ยังเล่นได้
            AudioSource.PlayClipAtPoint(stompClip, transform.position, stompVolume);
        }
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