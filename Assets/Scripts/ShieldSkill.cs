using UnityEngine;

public class ShieldSkill : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldPrefab;
    public float shieldOffset = 1f;

    private GameObject currentShield;

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (currentShield == null)
                currentShield = Instantiate(shieldPrefab);

            // ไม่เป็น Child - คำนวณตำแหน่งเองใน World Space
            float dir = Mathf.Sign(transform.localScale.x);
            currentShield.transform.position = transform.position + new Vector3(shieldOffset * dir, 0f, 0f);
        }
        else
        {
            if (currentShield != null)
            {
                Destroy(currentShield);
                currentShield = null;
            }
        }
    }

    void OnDestroy()
    {
        if (currentShield != null)
            Destroy(currentShield);
    }
}