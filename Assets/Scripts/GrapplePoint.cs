using UnityEngine;
using System.Collections;

public class GrapplePoint : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleRange = 5f;
    public float pullSpeed = 10f;
    public float stopDistance = 0.5f;

    [Header("Line")]
    public LineRenderer lineRenderer;

    private Player player;
    private Rigidbody2D playerRb;
    private bool isPlayerGrappling = false;
    private bool cancelGrapple = false;

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPlayerGrappling && dist <= grappleRange)
            {
                cancelGrapple = false;
                StartCoroutine(PullPlayer());
            }
            else if (isPlayerGrappling)
            {
                // กด E ระหว่างดึง = ยกเลิก
                cancelGrapple = true;
            }
        }

        if (isPlayerGrappling && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.transform.position);
        }
    }

    IEnumerator PullPlayer()
    {
        isPlayerGrappling = true;
        player.isGrappling = true;
        playerRb.gravityScale = 0f;
        if (lineRenderer != null) lineRenderer.enabled = true;

        while (true)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= stopDistance) break;
            if (cancelGrapple) break;

            Vector2 direction = ((Vector2)transform.position - (Vector2)player.transform.position).normalized;
            playerRb.linearVelocity = direction * pullSpeed;
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