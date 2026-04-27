using UnityEngine;

public class ShieldSkill : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldOffset = 1f;

    [Header("Reflect Settings")]
    public GameObject reflectBulletPrefab;
    public float reflectBulletRange = 15f;

    private GameObject currentShield;
    private Player player;
    private Animator animator;
    private bool isBlocking = false;

    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F) && player.isGrounded)
        {
            isBlocking = true;

            if (currentShield == null)
            {
                currentShield = Instantiate(shieldPrefab);
                ShieldCollider sc = currentShield.GetComponent<ShieldCollider>();
                if (sc != null) sc.Init(this); // ส่ง reference ให้ ShieldCollider
            }

            float dir = Mathf.Sign(transform.localScale.x);
            currentShield.transform.position = transform.position + new Vector3(shieldOffset * dir, 0f, 0f);
            Vector3 originalScale = currentShield.transform.localScale;
            currentShield.transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * dir, originalScale.y, originalScale.z);

            if (player != null)
                player.SetMovementLocked(true);
        }
        else
        {
            isBlocking = false;

            if (currentShield != null)
            {
                Destroy(currentShield);
                currentShield = null;
            }

            if (player != null)
                player.SetMovementLocked(false);
        }
    }

    public bool IsBlocking() => isBlocking;

    public void ReflectBullet(Vector3 hitPosition)
    {
        if (reflectBulletPrefab == null) return;

        int dir = Mathf.Sign(transform.localScale.x) > 0 ? 1 : -1;
        GameObject bullet = Instantiate(reflectBulletPrefab, hitPosition, Quaternion.identity);
        ReflectBullet rb = bullet.GetComponent<ReflectBullet>();
        if (rb != null)
            rb.Init(dir, reflectBulletRange);
    }

    void OnDestroy()
    {
        if (currentShield != null)
            Destroy(currentShield);

        if (player != null)
            player.SetMovementLocked(false);
    }
}