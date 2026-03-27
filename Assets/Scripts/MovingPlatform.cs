using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Movement Settings")]
    public Direction moveDirection = Direction.Up; // เลือกทิศทางใน Inspector
    public float moveDistance = 3f; // ระยะทางที่จะเลื่อน
    public float moveSpeed = 2f;    // ความเร็ว

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        float pingPong = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
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

    // วาด Gizmo ใน Scene view (เหมือน MonsterAI)
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

        // เส้นระยะ
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, endPos);

        // จุดเริ่มต้นและจุดปลาย
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(origin, 0.1f);
        Gizmos.DrawSphere(endPos, 0.1f);
    }
}