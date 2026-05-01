using UnityEngine;
using UnityEngine.Audio;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector] public bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Sound")]
    public AudioClip activateClip;
    public AudioClip respawnClip;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public AudioMixerGroup sfxMixerGroup;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;

        if (animator != null)
            animator.SetTrigger("IsTrigger");

        if (animator == null && spriteRenderer != null)
            spriteRenderer.color = Color.yellow;

        PlaySound(activateClip);
    }

    public void Deactivate()
    {
        isActivated = false;
        // reset animator / color ถ้าต้องการ
    }

    public void PlayRespawnSound()
    {
        PlaySound(respawnClip);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // ✅ บอก PlayerRespawn ให้เปลี่ยน spawn point มาที่นี่
        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        if (respawn != null)
            respawn.SetSpawnPoint(this);

        Activate();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxMixerGroup != null)
        {
            GameObject temp = new GameObject("SFX_" + clip.name);
            temp.transform.position = transform.position;
            AudioSource src = temp.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxMixerGroup;
            src.clip = clip;
            src.volume = sfxVolume;
            src.spatialBlend = 0f;
            src.Play();
            Destroy(temp, clip.length + 0.1f);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolume);
        }
    }
}