using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        SceneTransitionManager.Instance.GoToScene(targetSceneName);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.4f);
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}