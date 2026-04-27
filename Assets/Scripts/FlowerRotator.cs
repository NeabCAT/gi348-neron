using UnityEngine;

public class FlowerRotator : MonoBehaviour
{
    public Transform enemy;
    public float rotationSpeed = 5f;
    public float returnSpeed = 3f;

    private bool rotating = false;
    private Quaternion originalRotation;
    private float autoOffset = 0f;

    void Start()
    {
        originalRotation = transform.rotation;

        if (enemy != null)
        {
            Vector3 dir = enemy.position - transform.position;
            float redAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;
            autoOffset = currentAngle - redAngle;
        }
    }

    public void StartRotating() => rotating = true;
    public void StopRotating() => rotating = false;

    void Update()
    {
        if (rotating && enemy != null)
        {
            Vector3 dir = enemy.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0f, 0f, angle + autoOffset),
                rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                originalRotation,
                returnSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        if (enemy == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, enemy.position);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 2f);
    }
}