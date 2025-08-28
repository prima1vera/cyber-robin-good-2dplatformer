using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class HitReaction2D : MonoBehaviour
{
    [Header("Flash")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashRoutine;

    [Header("Knockback")]
    public float defaultKnockbackForce = 5f;
    private Rigidbody2D rb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = sr.color;
    }

    public void OnHit(Transform source, Vector2? hitPoint, float knockbackForce)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());

        float force = knockbackForce > 0 ? knockbackForce : defaultKnockbackForce;
        if (force > 0 && source != null)
        {
            Vector2 dir = ((Vector2)transform.position - (Vector2)source.position).normalized;
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }

    IEnumerator Flash()
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
        flashRoutine = null;
    }
}
