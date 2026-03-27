using UnityEngine;

public class CollectiblePlatform : MonoBehaviour
{
    // Platform ที่จะโผล่ออกมา
    public GameObject platform;

    // ตรวจสอบว่าเก็บแล้วหรือยัง
    private bool isCollected = false;

    void Start()
    {
        // ซ่อน platform ตอนเริ่มเกม
        if (platform != null)
            platform.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้า Player ชนกับ Object นี้
        if (!isCollected && other.CompareTag("Player"))
        {
            // ซ่อน Object ที่เก็บ
            gameObject.SetActive(false);

            // โผล่ platform
            if (platform != null)
                platform.SetActive(true);

            isCollected = true;
        }
    }
}