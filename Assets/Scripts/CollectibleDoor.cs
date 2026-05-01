using UnityEngine;

public class CollectibleDoor : MonoBehaviour
{
    public GameObject door;

    [Header("Sound")]
    public AudioClip collectClip;
    public AudioClip doorOpenClip;
    [Range(0f, 1f)] public float volume = 1f;

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
            if (collectClip != null)
                AudioSource.PlayClipAtPoint(collectClip, transform.position, volume);

            if (door != null)
            {
                if (doorOpenClip != null)
                    AudioSource.PlayClipAtPoint(doorOpenClip, door.transform.position, volume);
                door.SetActive(false);
            }

            gameObject.SetActive(false);
            isCollected = true;
        }
    }
}