using UnityEngine;
using System.Collections;

public class FireTrigger : MonoBehaviour
{
    public GameObject flower;
    public int blinkCount = 3;
    public float blinkInterval = 0.2f;

    [Header("Sound")]
    public AudioClip triggerClip;   // เสียงตอนเหยียบ trigger
    public AudioClip appearClip;    // เสียงตอน flower โผล่สมบูรณ์
    [Range(0f, 1f)] public float volume = 1f;

    private bool triggered = false;

    void Start()
    {
        flower.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        if (triggerClip != null)
            AudioSource.PlayClipAtPoint(triggerClip, transform.position, volume);

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            flower.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
            flower.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
        }

        flower.SetActive(true);

        if (appearClip != null)
            AudioSource.PlayClipAtPoint(appearClip, flower.transform.position, volume);

        Destroy(gameObject);
    }
}