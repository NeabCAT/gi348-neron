using UnityEngine;
using System.Collections;

public class FireTrigger : MonoBehaviour
{
    public GameObject flower;
    public int blinkCount = 3;
    public float blinkInterval = 0.2f;

    private bool triggered = false;

    void Start()
    {
        flower.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
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
        Destroy(gameObject);
    }
}