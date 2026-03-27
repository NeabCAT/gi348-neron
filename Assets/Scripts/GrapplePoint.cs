using UnityEngine;
using System.Collections;

public class GrapplePoint : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleRange = 5f;
    public float pullSpeed = 10f;
    public float stopDistance = 0.5f;

    [Header("Mode")]
    public bool holdToGrapple = false;

    [Header("Swing Settings (Hold Mode Only)")]
    public float swingForce = 5f;

    [Header("Line")]
    public LineRenderer lineRenderer;

    private Player player;
    private Rigidbody2D playerRb;
    private bool isPlayerGrappling = false;
    private bool cancelGrapple = false;
    private float ropeLength;

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (holdToGrapple)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!isPlayerGrappling && dist <= grappleRange)
                {
                    cancelGrapple = false;
                    ropeLength = dist; // ✅ จำความยาวเชือกตอนเริ่ม
                    StartCoroutine(PullPlayer());
                }

                // ✅ เพิ่มแค่นี้ — แกว่งซ้ายขวาด้วย A/D ขณะโหนอยู่
                if (isPlayerGrappling)
                {
                    Vector2 toAnchor = (Vector2)transform.position - (Vector2)player.transform.position;
                    Vector2 perpendicular = new Vector2(toAnchor.y, -toAnchor.x).normalized;
                    float horizontal = Input.GetAxisRaw("Horizontal");
                    if (horizontal != 0f)
                        playerRb.AddForce(perpendicular * horizontal * swingForce, ForceMode2D.Force);

                    // ✅ จำกัดความยาวเชือก ให้แกว่งเป็นลูกตุ้ม
                    Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
                    if (toPlayer.magnitude > ropeLength)
                    {
                        Vector2 constrained = (Vector2)transform.position + toPlayer.normalized * ropeLength;
                        player.transform.position = constrained;
                        Vector2 radial = toPlayer.normalized;
                        float radialVel = Vector2.Dot(playerRb.linearVelocity, radial);
                        if (radialVel > 0f)
                            playerRb.linearVelocity -= radial * radialVel;
                    }
                }
            }
            else
            {
                if (isPlayerGrappling)
                    cancelGrapple = true;
            }
        }
        else
        {
            // ——— โค้ดเดิม ไม่แตะเลย ———
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isPlayerGrappling && dist <= grappleRange)
                {
                    cancelGrapple = false;
                    StartCoroutine(PullPlayer());
                }
                else if (isPlayerGrappling)
                {
                    cancelGrapple = true;
                }
            }
        }

        if (isPlayerGrappling && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.transform.position);
        }
    }

    // ——— Coroutine เดิม ไม่แตะเลย ———
    IEnumerator PullPlayer()
    {
        isPlayerGrappling = true;
        player.isGrappling = true;
        playerRb.gravityScale = holdToGrapple ? 1f : 0f;
        if (lineRenderer != null) lineRenderer.enabled = true;

        while (true)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= stopDistance) break;
            if (cancelGrapple) break;

            // ✅ ถ้า holdToGrapple = true ไม่ดูด ให้แค่โหนแกว่งอย่างเดียว
            if (!holdToGrapple)
            {
                Vector2 direction = ((Vector2)transform.position - (Vector2)player.transform.position).normalized;
                playerRb.linearVelocity = direction * pullSpeed;
            }

            yield return null;
        }

        if (lineRenderer != null) lineRenderer.enabled = false;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.gravityScale = 1f;
        player.isGrappling = false;
        isPlayerGrappling = false;
        cancelGrapple = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}