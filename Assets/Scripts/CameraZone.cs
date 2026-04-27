using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [Header("Camera Settings for this Zone")]
    public float targetOrthographicSize = 5f;
    public Vector2 offset = new Vector2(0f, 2f);

    void Start()
    {
        // เช็คตอนเปิดซีนว่า Player อยู่ใน Zone นี้แล้วหรือเปล่า
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        // ถ้า Player อยู่ใน Collider นี้แล้ว ให้ Snap เลย

        Collider2D zone = GetComponent<Collider2D>();
        if (zone != null && zone.bounds.Contains(player.transform.position))
        {
            if (Camera.main == null) return; // ✅ เพิ่มตรงนี้
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
                cam.SnapToZone(targetOrthographicSize, offset);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.SetZone(targetOrthographicSize, offset);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // เช็คว่าผู้เล่นอยู่ใน Zone อื่นอีกไหม
        CameraZone[] allZones = FindObjectsByType<CameraZone>(FindObjectsSortMode.None);
        foreach (CameraZone zone in allZones)
        {
            if (zone == this) continue;
            Collider2D col = zone.GetComponent<Collider2D>();
            if (col != null && col.bounds.Contains(other.transform.position))
            {
                // ยังอยู่ใน Zone อื่น ให้ Zone นั้น Override แทน
                CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
                if (cam != null)
                    cam.SetZone(zone.targetOrthographicSize, zone.offset);
                return;
            }
        }

        CameraFollow cam2 = Camera.main.GetComponent<CameraFollow>();
        if (cam2 != null)
            cam2.ResetZone();
    }


}