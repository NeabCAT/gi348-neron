using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [Header("General")]
    public float grappleRange = 5f;

    [Header("Smooth Pull (Tap Mode)")]
    public float targetSpeed = 12f;     // ความเร็วตอนดูดเข้า
    public float steerForce = 5f;       // ความนุ่ม (ยิ่งมากยิ่งตอบสนองไว)
    public float slowRadius = 2f;       // ระยะที่เริ่มชะลอ

    [Header("Rope Physics")]
    public float ropeStiffness = 60f;
    public float swingForce = 10f;

    [Header("Auto Release (Tap Mode)")]
    public float stopDistance = 0.5f;
    public float maxGrappleTime = 2f;
    public float minVelocityToKeep = 0.5f;

    [Header("Mode")]
    public bool holdToGrapple = true; // ✅ ติ๊ก = กดค้าง / ไม่ติ๊ก = กดครั้งเดียว

    public LineRenderer lineRenderer;

    private Rigidbody2D playerRb;
    private Transform player;

    private bool isGrappling = false;
    private float ropeLength;
    private float grappleTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();

        playerRb.linearDamping = 1f; // 👉 เพิ่มความลื่น

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        // 🟢 HOLD MODE (กดค้าง)
        if (holdToGrapple)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!isGrappling && dist <= grappleRange)
                {
                    StartGrapple();
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                StopGrapple();
            }
        }
        // 🔵 TAP MODE (กดครั้งเดียว)
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 🔁 กดซ้ำ = ยกเลิก
                if (isGrappling)
                {
                    StopGrapple();
                    return;
                }

                if (dist <= grappleRange)
                {
                    StartGrapple();
                }
            }
        }

        // 🎨 วาดเชือก
        if (isGrappling && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.position);
        }
    }

    void FixedUpdate()
    {
        if (!isGrappling) return;

        grappleTimer += Time.fixedDeltaTime;

        Vector2 anchor = transform.position;
        Vector2 pos = playerRb.position;

        float dist = Vector2.Distance(anchor, pos);

        // 🎯 TAP MODE: ถึงแล้วปล่อย
        if (!holdToGrapple && dist <= stopDistance)
        {
            StopGrapple();
            return;
        }

        // ⏱️ กันค้าง
        if (!holdToGrapple && grappleTimer > maxGrappleTime)
        {
            StopGrapple();
            return;
        }

        // 🐢 ช้าเกิน = ปล่อย
        if (!holdToGrapple && playerRb.linearVelocity.magnitude < minVelocityToKeep)
        {
            StopGrapple();
            return;
        }

        Vector2 toPlayer = pos - anchor;
        if (toPlayer.magnitude == 0) return;

        Vector2 dir = toPlayer.normalized;

        // 🧷 Constraint (กันหลุดวง)
        float outwardVel = Vector2.Dot(playerRb.linearVelocity, dir);
        if (dist > ropeLength && outwardVel > 0)
        {
            playerRb.linearVelocity -= dir * outwardVel;
        }

        // 🧲 Tension (เชือกตึง)
        float stretch = dist - ropeLength;
        if (stretch > 0)
        {
            playerRb.AddForce(-dir * stretch * ropeStiffness);
        }

        // 🌀 Swing (เฉพาะ hold)
        if (holdToGrapple)
        {
            Vector2 tangent = new Vector2(-dir.y, dir.x);
            float input = Input.GetAxisRaw("Horizontal");
            playerRb.AddForce(tangent * input * swingForce);
        }

        // 🎯 Smooth Pull (แทน impulse/pullForce)
        if (!holdToGrapple)
        {
            float speedMultiplier = Mathf.Clamp01(dist / slowRadius);
            Vector2 desiredVel = -dir * targetSpeed * speedMultiplier;

            Vector2 steering = desiredVel - playerRb.linearVelocity;
            playerRb.AddForce(steering * steerForce);
        }
    }

    void StartGrapple()
    {
        isGrappling = true;
        grappleTimer = 0f;

        float dist = Vector2.Distance(transform.position, player.position);

        // 🔥 ทำให้ tap “ดูดเข้า”
        if (!holdToGrapple)
            ropeLength = dist * 0.7f;
        else
            ropeLength = dist;

        if (lineRenderer != null)
            lineRenderer.enabled = true;

        if (!holdToGrapple)
        {
            Vector2 dir = ((Vector2)transform.position - playerRb.position).normalized;
            playerRb.AddForce(dir * 5f, ForceMode2D.Impulse); // เบา ๆ พอ
        }
    }

    void StopGrapple()
    {
        if (!isGrappling) return;

        isGrappling = false;

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        // 🎯 วงระยะ grapple
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetCenter(), grappleRange);

        // 🎯 วง stop distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetCenter(), stopDistance);

        // 🔲 กริด (ช่วยกะระยะ)
        DrawGrid(GetCenter(), grappleRange);
    }

    void DrawGrid(Vector2 center, float size)
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.15f);

        float step = 1f; // 👈 ระยะช่องกริด (ปรับได้)

        for (float x = -size; x <= size; x += step)
        {
            Vector2 start = new Vector2(center.x + x, center.y - size);
            Vector2 end = new Vector2(center.x + x, center.y + size);
            Gizmos.DrawLine(start, end);
        }

        for (float y = -size; y <= size; y += step)
        {
            Vector2 start = new Vector2(center.x - size, center.y + y);
            Vector2 end = new Vector2(center.x + size, center.y + y);
            Gizmos.DrawLine(start, end);
        }
    }

    Vector2 GetCenter()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            return col.bounds.center;

        return transform.position;
    }
}