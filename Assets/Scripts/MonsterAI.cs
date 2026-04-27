// MonsterAI.cs
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolDistance = 3f;
    public float moveSpeed = 2f;

    private Vector2 startPosition;
    private int direction = 1;
    private Vector3 originalScale;
    private EnemyContact enemyContact;

    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
        enemyContact = GetComponent<EnemyContact>();
    }

    void Update()
    {
        if (enemyContact != null && enemyContact.isDead) return;

        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        float dist = transform.position.x - startPosition.x;
        if (dist >= patrolDistance)
            direction = -1;
        else if (dist <= -patrolDistance)
            direction = 1;

        transform.localScale = new Vector3(
            direction * Mathf.Abs(originalScale.x),
            originalScale.y,
            originalScale.z
        );
    }

    void OnDrawGizmos()
    {
        Vector2 origin = Application.isPlaying ? startPosition : (Vector2)transform.position;

        Vector2 leftPoint = origin + Vector2.left * patrolDistance;
        Vector2 rightPoint = origin + Vector2.right * patrolDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawSphere(leftPoint, 0.1f);
        Gizmos.DrawSphere(rightPoint, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(origin, 0.1f);
    }
}