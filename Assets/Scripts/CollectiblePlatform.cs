using UnityEngine;
using UnityEngine.Audio;

public class CollectiblePlatform : MonoBehaviour
{
    public GameObject platform;

    [Header("Sound")]
    public AudioClip collectClip;
    [Range(0f, 1f)] public float collectVolume = 1f;
    public AudioMixerGroup sfxMixerGroup;

    private bool isCollected = false;

    void Start()
    {
        if (platform != null)
            platform.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            isCollected = true;

            // 🔊 เสียงก่อน SetActive(false) — ใช้ temp GameObject เพราะตัวนี้กำลังจะหาย
            PlayCollectSound();

            gameObject.SetActive(false);

            if (platform != null)
                platform.SetActive(true);
        }
    }

    void PlayCollectSound()
    {
        if (collectClip == null) return;

        if (sfxMixerGroup != null)
        {
            GameObject temp = new GameObject("CollectSFX");
            temp.transform.position = transform.position;
            AudioSource src = temp.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxMixerGroup;
            src.clip = collectClip;
            src.volume = collectVolume;
            src.spatialBlend = 0f;
            src.Play();
            Destroy(temp, collectClip.length + 0.1f);
        }
        else
        {
            AudioSource.PlayClipAtPoint(collectClip, transform.position, collectVolume);
        }
    }
}