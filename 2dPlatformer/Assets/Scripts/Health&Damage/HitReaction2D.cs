using System.Collections;
using UnityEngine;

/// <summary>
/// ������������� ������� �� ���������:
/// - ��������������� ���� ����� SpriteRenderer.color (��� ��������)
/// - ���������� ������ ����� Rigidbody2D.AddForce
/// ������ �� ����� ��� ��������.
/// </summary>
[DisallowMultipleComponent]
public class HitReaction2D : MonoBehaviour
{
    [Header("Flash")]
    [Tooltip("������� ������ ���������� ����")]
    public float flashDuration = 0.12f;
    [Tooltip("���� �����")]
    public Color flashColor = Color.red;

    [Header("Knockback")]
    [Tooltip("���� ������������ �� ���������, ���� �� �������� � �����")]
    public float defaultKnockbackForce = 5f;
    [Tooltip("������� �����, ����� ���� �������� ���������")]
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
    /// ����� �� Health: ��������� ������/������ ���������.
    /// hitSource/hitPoint � �����������.
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
        // ������� � ��������� ����� ������, ���� ��� ����� ���������
        sr.color = originalColor;
        flashCo = null;
    }
}
