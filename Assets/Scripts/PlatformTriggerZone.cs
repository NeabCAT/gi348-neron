using UnityEngine;

public class PlatformTriggerZone : MonoBehaviour
{
    private MovingPlatform platform;

    void Start()
    {
        // หา MovingPlatform จาก Parent อัตโนมัติ ไม่ต้องลากใส่เอง
        platform = GetComponentInParent<MovingPlatform>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (platform != null)
            platform.Activate();
    }
}