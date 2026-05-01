using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicZone : MonoBehaviour
{
    [Header("ลาก AudioSource BGM ตัวเดิมมาใส่")]
    public AudioSource bgmSource;

    [Header("Music")]
    public AudioClip zoneMusic;
    [Range(0f, 1f)] public float volume = 1f;
    public float fadeDuration = 1f;

    private static MusicZone currentZone;
    private static Coroutine fadeCoroutine;
    private static MonoBehaviour coroutineRunner;
    private static Dictionary<AudioClip, AudioSource> preloadedSources = new Dictionary<AudioClip, AudioSource>();
    private static AudioSource activeSource;
    private static AudioClip defaultClip;
    private static float defaultVolume;

    void Awake()
    {
        if (coroutineRunner == null)
        {
            GameObject obj = new GameObject("MusicZoneRunner");
            DontDestroyOnLoad(obj);
            coroutineRunner = obj.AddComponent<MusicZoneRunner>();
        }
    }

    void Start()
    {
        if (zoneMusic != null)
            PreloadClip(zoneMusic, 0f);

        if (activeSource == null)
        {
            activeSource = bgmSource;
            defaultClip = bgmSource.clip;
            defaultVolume = volume;

            if (defaultClip != null)
            {
                PreloadClip(defaultClip, defaultVolume);
                preloadedSources[defaultClip] = bgmSource;
            }
        }
    }

    void PreloadClip(AudioClip clip, float startVolume)
    {
        if (clip == null) return;
        if (preloadedSources.ContainsKey(clip)) return;

        GameObject obj = new GameObject($"BGM_{clip.name}");
        DontDestroyOnLoad(obj);

        AudioSource src = obj.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.spatialBlend = 0f;
        src.volume = startVolume;
        src.playOnAwake = false;
        src.Play();

        preloadedSources[clip] = src;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (currentZone == this) return;

        currentZone = this;
        StartFade(zoneMusic, volume);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (currentZone != this) return;

        currentZone = null;
        if (defaultClip != null)
            StartFade(defaultClip, defaultVolume);
    }

    void StartFade(AudioClip clip, float targetVolume)
    {
        if (clip == null) return;
        if (fadeCoroutine != null)
            coroutineRunner.StopCoroutine(fadeCoroutine);
        fadeCoroutine = coroutineRunner.StartCoroutine(CrossFade(clip, targetVolume));
    }

    IEnumerator CrossFade(AudioClip newClip, float targetVolume)
    {
        if (!preloadedSources.ContainsKey(newClip)) yield break;

        AudioSource nextSource = preloadedSources[newClip];
        if (nextSource == activeSource) yield break;

        float startVolume = activeSource.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float ratio = t / fadeDuration;

            activeSource.volume = Mathf.Lerp(startVolume, 0f, ratio);
            nextSource.volume = Mathf.Lerp(0f, targetVolume, ratio);

            yield return null;
        }

        activeSource.volume = 0f;
        nextSource.volume = targetVolume;
        activeSource = nextSource;
    }
}

public class MusicZoneRunner : MonoBehaviour { }