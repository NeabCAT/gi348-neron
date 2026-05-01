using System.Collections;
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destination;

    [Header("Scene Transition")]
    public bool goToSceneAfter = false;    // ติ๊กถ้าอยากให้ย้าย scene หลัง teleport
    public string targetScene = "MainMenu";
    public float delayBeforeScene = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(TeleportRoutine(other.transform));
    }

    IEnumerator TeleportRoutine(Transform player)
    {
        Player playerScript = player.GetComponent<Player>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (playerScript != null) playerScript.SetMovementLocked(true);
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // จอมืด
        yield return StartCoroutine(SceneTransitionManager.Instance.Fade(0f, 1f));

        // teleport player
        player.position = destination.position;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // snap กล้อง
        CameraFollow cam = FindFirstObjectByType<CameraFollow>();
        if (cam != null) cam.SnapToTarget();

        yield return null;

        // จอสว่าง
        yield return StartCoroutine(SceneTransitionManager.Instance.Fade(1f, 0f));

        if (playerScript != null) playerScript.SetMovementLocked(false);

        // รอแล้วค่อยย้าย scene
        if (goToSceneAfter)
        {
            yield return new WaitForSeconds(delayBeforeScene);
            SceneTransitionManager.Instance.GoToScene(targetScene);
        }
    }
}