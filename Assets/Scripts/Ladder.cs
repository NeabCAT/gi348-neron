using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Limits")]
    public Transform topPoint;
    public Transform bottomPoint;

    private float originalGravity;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        Player p = col.GetComponent<Player>();
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();

        p.canClimb = true;

        // ส่งข้อมูล limit ให้ Player ตัดสินใจเอง
        float playerY = col.transform.position.y;
        p.climbCanGoUp = playerY < topPoint.position.y;
        p.climbCanGoDown = playerY > bottomPoint.position.y;

        // เริ่ม climbing เมื่อกด W/S
        if (!p.isClimbing)
        {
            bool wantUp = Input.GetKey(KeyCode.W) && p.climbCanGoUp;
            bool wantDown = Input.GetKey(KeyCode.S) && p.climbCanGoDown;

            if (wantUp || wantDown)
            {
                p.isClimbing = true;
                DisableGroundCollision(col);
                rb.gravityScale = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        Player p = col.GetComponent<Player>();
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();

        p.canClimb = false;
        p.isClimbing = false;
        p.climbCanGoUp = false;
        p.climbCanGoDown = false;

        rb.gravityScale = originalGravity;
        p.StartCoroutine(RestoreCollision(col));
    }

    public void DisableGroundCollision(Collider2D playerCol)
    {
        foreach (Collider2D ground in FindObjectsOfType<Collider2D>())
        {
            if (ground.CompareTag("Ground"))
                Physics2D.IgnoreCollision(playerCol, ground, true);
        }
    }

    IEnumerator RestoreCollision(Collider2D playerCol)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        foreach (Collider2D ground in FindObjectsOfType<Collider2D>())
        {
            if (ground.CompareTag("Ground"))
                Physics2D.IgnoreCollision(playerCol, ground, false);
        }
    }
}