using UnityEngine;
using System.Collections;

public class CollectibleDoor : MonoBehaviour
{
    public GameObject door;

    [Header("Sound")]
    public AudioClip collectClip;
    public AudioClip doorOpenClip;
    [Range(0f, 1f)] public float volume = 1f;
    public float doorSoundDelay = 0.5f;

    private bool isCollected = false;

    void Start()
    {
        if (door != null)
            door.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            isCollected = true;

            if (collectClip != null)
                AudioSource.PlayClipAtPoint(collectClip, transform.position, volume);

            // สร้าง GameObject ชั่วคราวมารัน Coroutine
            GameObject runner = new GameObject("DoorSoundRunner");
            runner.AddComponent<DoorSoundRunner>().Play(doorOpenClip, door, doorSoundDelay, volume);

            gameObject.SetActive(false);
        }
    }
}

// Helper class รัน Coroutine แล้วทำลายตัวเอง
public class DoorSoundRunner : MonoBehaviour
{
    public void Play(AudioClip clip, GameObject door, float delay, float volume)
    {
        StartCoroutine(Run(clip, door, delay, volume));
    }

    IEnumerator Run(AudioClip clip, GameObject door, float delay, float volume)
    {
        yield return new WaitForSeconds(delay);

        if (clip != null)
        {
            // สร้าง AudioSource 2D ได้ยินเท่ากันทั้งแมพ
            GameObject audioObj = new GameObject("DoorSound");
            AudioSource src = audioObj.AddComponent<AudioSource>();
            src.clip = clip;
            src.volume = volume;
            src.spatialBlend = 0f; // 0 = 2D, ได้ยินเท่ากันทุกระยะ
            src.Play();
            Destroy(audioObj, clip.length);
        }

        if (door != null)
            door.SetActive(false);

        Destroy(gameObject);
    }
}