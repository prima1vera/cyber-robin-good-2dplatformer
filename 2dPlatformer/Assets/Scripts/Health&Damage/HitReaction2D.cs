using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class HitReaction2D : MonoBehaviour
{
    [Header("Flash Settings")]
    [Tooltip("Color to flash when hit")]
    public Color flashColor = Color.white;
    [Tooltip("Duration of flash effect in seconds")]
    public float flashDuration = 0.1f;

    [Header("Knockback Settings")]
    [Tooltip("Force applied when hit (0 = no knockback)")]
    public float knockbackForce = 5f;

    [Header("Audio Settings")]
    [Tooltip("Sound played when hit")]
    public AudioClip hitSound;
    
    private AudioSource audioSource;

    // Cached
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;

        if (audioSource == null && hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Главный метод: проигрывает все реакции на урон (звук, флэш, нокаут).
    /// </summary>
    public void OnHit(Transform hitSource, Vector2? hitPoint, float customKnockback = -1f)
    {
        PlayFlash();
        PlaySound();
        ApplyKnockback(hitSource, customKnockback);
    }

    private void PlayFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void PlaySound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void ApplyKnockback(Transform hitSource, float customKnockback)
    {
        float force = customKnockback >= 0 ? customKnockback : knockbackForce;
        if (force <= 0 || rb == null || hitSource == null) return;

        Vector2 dir = (transform.position - hitSource.position).normalized;
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }
}
