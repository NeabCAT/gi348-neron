using UnityEngine;

public class CollectibleDoor : MonoBehaviour
{
    // ประตูที่จะเปิด
    public GameObject door;
    // ตรวจสอบว่าเก็บแล้วหรือยัง
    private bool isCollected = false;

    void Start()
    {
        // ประตูปิดตอนเริ่มเกม
        if (door != null)
            door.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            // ซ่อน Object ที่เก็บ
            gameObject.SetActive(false);

            // เปิดประตู
            if (door != null)
                door.SetActive(false);

            isCollected = true;
        }
    }
}