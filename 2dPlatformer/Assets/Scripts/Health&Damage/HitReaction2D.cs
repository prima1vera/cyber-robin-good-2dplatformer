using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HitReaction2D : MonoBehaviour
{
    [Header("Flash Settings")]
    public SpriteRenderer flashRenderer; // можно указать вручную (например, PlayerSprite)
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;

    [Header("Sound Settings")]
    [Tooltip("Звук, который проигрывается при получении урона")]
    public AudioClip hitSound;

    private Color originalColor;
    private AudioSource audioSource;
    private Rigidbody2D rb;

    void Awake()
    {
        // Если явно не указали flashRenderer — пробуем взять с текущего объекта
        if (flashRenderer == null)
        {
            flashRenderer = GetComponent<SpriteRenderer>();
        }

        if (flashRenderer != null)
        {
            originalColor = flashRenderer.color;
        }

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

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
        if (flashRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine());
        }

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
        flashRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        flashRenderer.color = originalColor;
    }
}
