using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public Vector2 offset = new Vector2(0f, 2f);
    public float followSmoothTime = 0.1f;
    public float maxSpeed = 20f;

    [Header("Zoom Settings")]
    public float defaultSize = 5f;
    public float startSize = -1f;          // ถ้า > 0 จะเริ่มที่ size นี้เลย ไม่ smooth
    public float zoomSmoothTime = 0.4f;
    public float zoomResetSmoothTime = 1.2f;

    [Header("Offset Transition")]
    public float offsetSmoothTime = 0.6f;
    public float offsetResetSmoothTime = 1.0f;

    private float targetSize;
    private Vector2 targetOffset;
    private Vector2 currentOffset;
    private Vector2 offsetVelocity;
    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity = 0f;
    private float currentZoomSmoothTime;
    private float currentOffsetSmoothTime;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetOffset = offset;
        currentOffset = offset;
        currentZoomSmoothTime = zoomSmoothTime;
        currentOffsetSmoothTime = offsetSmoothTime;

        // ถ้าตั้ง startSize ไว้ใช้ค่านั้นเลย ไม่ smooth
        float initSize = (startSize > 0f) ? startSize : defaultSize;
        targetSize = initSize;
        cam.orthographicSize = initSize;    // ← set ทันที ไม่รอ smooth
    }

    void LateUpdate()
    {
        if (target == null) return;

        currentOffset = Vector2.SmoothDamp(
            currentOffset, targetOffset, ref offsetVelocity, currentOffsetSmoothTime
        );

        Vector3 targetPos = new Vector3(
            target.position.x + currentOffset.x,
            target.position.y + currentOffset.y,
            transform.position.z
        );
        transform.position = Vector3.SmoothDamp(
            transform.position, targetPos, ref velocity, followSmoothTime, maxSpeed
        );

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize, targetSize, ref zoomVelocity, currentZoomSmoothTime
        );
    }

    public void SetZone(float size, Vector2 newOffset)
    {
        targetSize = size;
        targetOffset = newOffset;
        currentZoomSmoothTime = zoomSmoothTime;
        currentOffsetSmoothTime = offsetSmoothTime;
    }

    public void ResetZone()
    {
        targetSize = defaultSize;
        targetOffset = offset;
        currentZoomSmoothTime = zoomResetSmoothTime;
        currentOffsetSmoothTime = offsetResetSmoothTime;
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        currentOffset = offset;
        transform.position = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        velocity = Vector3.zero;
        offsetVelocity = Vector2.zero;
        zoomVelocity = 0f;

        // Snap zoom ทันที ไม่ smooth
        cam.orthographicSize = targetSize;
    }

    public void SnapToZone(float size, Vector2 zoneOffset)
    {
        targetSize = size;
        targetOffset = zoneOffset;
        currentOffset = zoneOffset;
        currentZoomSmoothTime = zoomSmoothTime;
        currentOffsetSmoothTime = offsetSmoothTime;

        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x + zoneOffset.x,
                target.position.y + zoneOffset.y,
                transform.position.z
            );
        }

        velocity = Vector3.zero;
        offsetVelocity = Vector2.zero;
        zoomVelocity = 0f;
        cam.orthographicSize = size;
    }
}
