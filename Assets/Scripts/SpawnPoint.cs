using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector] public bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>(); // ✅ หาใน Child
    }

    public void Activate()
    {
        isActivated = true;

        if (animator != null)
            animator.SetTrigger("IsTrigger");

        if (animator == null && spriteRenderer != null)
            spriteRenderer.color = Color.yellow;
    }
}