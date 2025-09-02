using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class HitReaction2D : MonoBehaviour
{
    [Header("Flash Settings")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;

    [Header("Sound Settings")]
    [Tooltip("Звук, который проигрывается при получении урона")]
    public AudioClip hitSound;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private AudioSource audioSource;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        originalColor = spriteRenderer.color;

        // Настраиваем AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    /// <summary>
    /// Вызывается при получении урона
    /// </summary>
    public void OnHit(Transform hitSource, Vector2? hitPoint, float customKnockbackForce = -1f)
    {
        // Flash
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());

        // Knockback
        if (hitSource != null && rb != null)
        {
            float force = customKnockbackForce >= 0 ? customKnockbackForce : knockbackForce;
            if (force > 0)
            {
                Vector2 direction = (transform.position - hitSource.position).normalized;
                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }

        // Sound
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}
