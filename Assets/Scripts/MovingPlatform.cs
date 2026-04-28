using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }

    [Header("Movement Settings")]
    public Direction moveDirection = Direction.Up;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    [Header("Trigger Settings")]
    public bool isTriggerMove = false;

    private Vector3 startPos;
    private Vector3 lastPos;
    private bool isActivated = false;
    private float activatedTime = 0f;
    private Rigidbody2D rb;

    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        startPos = transform.position;
        lastPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isTriggerMove && !isActivated)
        {
            PlatformVelocity = Vector3.zero;
            return;
        }

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

        Vector3 newPos = startPos + offset;
        PlatformVelocity = (newPos - lastPos) / Time.fixedDeltaTime;
        lastPos = newPos;

        rb.MovePosition(newPos);
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;
        activatedTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Player p = col.gameObject.GetComponent<Player>();
        if (p != null) p.SetOnMovingPlatform(true, this);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        Player p = col.gameObject.GetComponent<Player>();
        if (p != null) p.SetOnMovingPlatform(false, null);
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