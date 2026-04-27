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

        float playerY = col.transform.position.y;
        bool canMoveUp = playerY < topPoint.position.y;
        bool canMoveDown = playerY > bottomPoint.position.y;

        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.W) && canMoveUp) verticalInput = 1f;
        if (Input.GetKey(KeyCode.S) && canMoveDown) verticalInput = -1f;

        if (verticalInput != 0f)
        {
            p.isClimbing = true;
            DisableGroundCollision(col);
            rb.gravityScale = 0f;

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput * p.moveSpeed, verticalInput * p.climbSpeed);
        }
        else if (p.isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        Player p = col.GetComponent<Player>();
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
        p.canClimb = false;
        p.isClimbing = false;
        p.StartCoroutine(RestoreCollision(col)); // ย้ายไปรันที่ Player
        rb.gravityScale = originalGravity;
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