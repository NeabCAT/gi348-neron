using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            col.GetComponent<Player>().canClimb = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player p = col.GetComponent<Player>();
            p.canClimb = false;

            if (p.isClimbing)
            {
                p.isClimbing = false;
                StartCoroutine(RestoreCollision(col));
            }
        }
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