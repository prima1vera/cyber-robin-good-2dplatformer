using System.Collections;
using UnityEngine;

/// <summary>
/// Универсальная реакция на попадание:
/// - кратковременный флэш через SpriteRenderer.color (без шейдеров)
/// - физический отскок через Rigidbody2D.AddForce
/// Ничего не знает про здоровье.
/// </summary>
[DisallowMultipleComponent]
public class HitReaction2D : MonoBehaviour
{
    [Header("Flash")]
    [Tooltip("Сколько длится визуальный флэш")]
    public float flashDuration = 0.12f;
    [Tooltip("Цвет флэша")]
    public Color flashColor = Color.red;

    [Header("Knockback")]
    [Tooltip("Сила отбрасывания по умолчанию, если не передана с удара")]
    public float defaultKnockbackForce = 5f;
    [Tooltip("Добавка вверх, чтобы было приятнее визуально")]
    public float verticalBonus = 0.5f;

    SpriteRenderer sr;
    Rigidbody2D rb;

    Color originalColor;
    bool hasOriginal;
    Coroutine flashCo;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (sr != null)
        {
            originalColor = sr.color;
            hasOriginal = true;
        }
    }

    /// <summary>
    /// Вызов из Health: проиграть визуал/физику попадания.
    /// hitSource/hitPoint — опциональны.
    /// </summary>
    public void OnHit(Transform hitSource = null, Vector2? hitPoint = null, float knockbackForce = 0f)
    {
        PlayFlash();

        if (rb != null)
        {
            Vector2 dir;
            if (hitSource != null)
                dir = (Vector2)(transform.position - hitSource.position);
            else if (hitPoint.HasValue)
                dir = (Vector2)transform.position - hitPoint.Value;
            else
                dir = Vector2.left;

            dir = dir.normalized;
            float force = (knockbackForce > 0f) ? knockbackForce : defaultKnockbackForce;

            Vector2 push = (dir + Vector2.up * verticalBonus).normalized * force;
            rb.AddForce(push, ForceMode2D.Impulse);
        }
    }

    void PlayFlash()
    {
        if (sr == null || !hasOriginal) return;

        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        sr.color = flashColor;
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        // Возврат к исходному цвету надёжно, даже при спаме попаданий
        sr.color = originalColor;
        flashCo = null;
    }
}
