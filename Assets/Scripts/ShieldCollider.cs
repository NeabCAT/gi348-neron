using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
    private ShieldSkill shieldSkill;

    public void Init(ShieldSkill skill)
    {
        shieldSkill = skill;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (shieldSkill == null) return;
        if (!shieldSkill.IsBlocking()) return; // เช็คว่ากำลัง Block อยู่จริง

        Bullet bullet = col.GetComponent<Bullet>();
        if (bullet != null)
        {
            shieldSkill.ReflectBullet(col.transform.position);
            Destroy(col.gameObject);
        }
    }
}