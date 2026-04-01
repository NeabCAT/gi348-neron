using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }

    [Header("Movement Settings")]
    public Direction moveDirection = Direction.Up;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    [Header("Trigger Settings")]
    public bool isTriggerMove = false; // ถ้าติ๊ก ต้องให้ผู้เล่น trigger ก่อนถึงจะขยับ

    private Vector3 startPos;
    private bool isActivated = false;
    private float activatedTime = 0f;

    void Start()
    {
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        // ถ้าเปิด isTriggerMove แต่ยังไม่ถูก trigger ให้หยุดนิ่ง
        if (isTriggerMove && !isActivated) return;

        float elapsed = isTriggerMove ? (Time.time - activatedTime) : Time.time;
        float pingPong = Mathf.PingPong(elapsed * moveSpeed, moveDistance);

        Vector3 offset = Vector3.zero;
        switch (moveDirection)
        {
            case Direction.Up: offset = Vector3.up * pingPong; break;
            case Direction.Down: offset = Vector3.down * pingPong; break;
            case Direction.Left: offset = Vector3.left * pingPong; break;
            case Direction.Right: offset = Vector3.right * pingPong; break;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(startPos + offset);
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;
        activatedTime = Time.time;
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = Application.isPlaying ? startPos : transform.position;
        Vector3 endPos = origin;

        switch (moveDirection)
        {
            case Direction.Up: endPos = origin + Vector3.up * moveDistance; break;
            case Direction.Down: endPos = origin + Vector3.down * moveDistance; break;
            case Direction.Left: endPos = origin + Vector3.left * moveDistance; break;
            case Direction.Right: endPos = origin + Vector3.right * moveDistance; break;
        }

        Gizmos.color = isTriggerMove ? Color.cyan : Color.yellow;
        Gizmos.DrawLine(origin, endPos);
        Gizmos.DrawSphere(origin, 0.1f);
        Gizmos.DrawSphere(endPos, 0.1f);
    }
}