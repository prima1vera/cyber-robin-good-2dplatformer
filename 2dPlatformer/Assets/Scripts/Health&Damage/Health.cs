using System;
using UnityEngine;

[RequireComponent(typeof(HitReaction2D))]
public class Health : MonoBehaviour, IDamageable
{
    [Header("Team Settings")]
    public int teamId = 0;

    [Header("Health Settings")]
    public int defaultHealth = 1;
    public int maximumHealth = 1;
    public int currentHealth = 1;

    [Tooltip("Invulnerability duration (sec) after taking damage).")]
    public float invincibilityTime = 0.3f;
    private bool isInvincible = false;
    private float timeToBecomeDamageable = 0f;

    [Header("Lives")]
    public bool useLives = false;
    public int currentLives = 3;
    public int maximumLives = 5;
    public float respawnWaitTime = 3f;
    private float respawnTime;
    private Vector3 respawnPosition;

    [Header("Effects")]
    public GameObject deathEffect;

    private HitReaction2D hitReaction;

    // --- Events for UI / other systems ---
    // current, max
    public event Action<int, int> OnHealthChanged;
    // called whenever damage successfully applied (before death check)
    public event Action OnDamaged;
    // called when died
    public event Action OnDied;

    void Awake()
    {
        hitReaction = GetComponent<HitReaction2D>();
        SetRespawnPoint(transform.position);
        currentHealth = defaultHealth;
    }

    void Start()
    {
        // notify listeners initial state
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);
        // legacy UI update kept
        GameManager.UpdateUIElements();
    }

    void Update()
    {
        if (isInvincible && Time.time >= timeToBecomeDamageable)
            isInvincible = false;

        if (respawnWaitTime != 0 && currentHealth <= 0 && currentLives > 0 && Time.time >= respawnTime)
            Respawn();
    }

    // ===== IDamageable =====
    // Backwards-compatible simple call
    public void TakeDamage(int damageAmount)
    {
        TakeDamage(damageAmount, null, null, 0f);
    }

    // Full API (used across the project)
    public void TakeDamage(int damageAmount, Transform hitSource = null, Vector2? hitPoint = null, float knockbackForce = 0f)
    {
        if (isInvincible || currentHealth <= 0)
            return;

        // invulnerability window
        isInvincible = true;
        timeToBecomeDamageable = Time.time + invincibilityTime;

        // apply damage
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        // events for UI / other listeners
        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);

        // hit reaction (flash/knockback/sound) — if present
        if (hitReaction != null)
        {
            hitReaction.OnHit(hitSource, hitPoint, knockbackForce);
        }

        // death check — Die() calls OnDied internally
        if (currentHealth <= 0)
        {
            Die();
        }

        // legacy UI update kept
        GameManager.UpdateUIElements();
    }

    public void ReceiveHealing(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maximumHealth);
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);
        GameManager.UpdateUIElements();
    }

    // ===== Lives & Respawn =====
    public void AddLives(int amount)
    {
        if (!useLives) return;
        currentLives = Mathf.Min(currentLives + amount, maximumLives);
        GameManager.UpdateUIElements();
    }

    public void SetRespawnPoint(Vector3 newPoint) => respawnPosition = newPoint;

    void Respawn()
    {
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);
        GameManager.UpdateUIElements();
    }

    void Die()
    {
        // notify listeners first (so they can play death anim / fx before object disappears)
        OnDied?.Invoke();

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (useLives)
        {
            currentLives--;
            if (currentLives > 0)
            {
                respawnTime = Time.time + respawnWaitTime;
            }
            else
            {
                GameOver();
                Destroy(gameObject);
            }
        }
        else
        {
            GameOver();
            Destroy(gameObject);
        }

        GameManager.UpdateUIElements();
    }

    void GameOver()
    {
        if (GameManager.instance != null && CompareTag("Player"))
            GameManager.instance.GameOver();
    }
}
