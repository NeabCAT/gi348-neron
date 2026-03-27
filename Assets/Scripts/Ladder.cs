using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Limits")]
    public Transform topPoint;    // จุดบนสุดของบันได
    public Transform bottomPoint; // จุดล่างสุดของบันได

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        Player p = col.GetComponent<Player>();
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();

        p.canClimb = true;

        float playerY = col.transform.position.y;

        // ตรวจว่าผู้เล่นอยู่ระหว่าง top-bottom
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

            // แนวนอน A/D
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontalInput * p.moveSpeed, verticalInput * p.climbSpeed);
        }
        else if (p.isClimbing)
        {
            // ถ้าไม่กด W/S หรืออยู่ปลายแล้ว ยังคงปีนอยู่แต่หยุด vertical
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        Player p = col.GetComponent<Player>();
        Rigidbody2D rb = col.GetComponent<Rigidbody2D>();

        p.canClimb = false;
        p.isClimbing = false;

        StartCoroutine(RestoreCollision(col));
        rb.gravityScale = 1f;
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