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

    [Tooltip("Invulnerability duration (sec) after taking damage.")]
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
    public GameObject hitEffect;
    public GameObject deathEffect;

    private HitReaction2D hitReaction;

    void Awake()
    {
        hitReaction = GetComponent<HitReaction2D>();
        SetRespawnPoint(transform.position);
        currentHealth = defaultHealth;
    }

    void Update()
    {
        if (isInvincible && Time.time >= timeToBecomeDamageable)
            isInvincible = false;

        if (respawnWaitTime != 0 && currentHealth <= 0 && currentLives > 0 && Time.time >= respawnTime)
            Respawn();
    }

    // ===== IDamageable =====
    public void TakeDamage(int damageAmount, Transform hitSource = null, Vector2? hitPoint = null, float knockbackForce = 0f)
    {
        if (isInvincible || currentHealth <= 0)
            return;

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        isInvincible = true;
        timeToBecomeDamageable = Time.time + invincibilityTime;

        currentHealth -= damageAmount;

        if (hitReaction != null)
        {
            hitReaction.OnHit(hitSource, hitPoint, knockbackForce);
        }

        if (currentHealth <= 0)
            Die();

        GameManager.UpdateUIElements();
    }

    public void ReceiveHealing(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maximumHealth);
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
        GameManager.UpdateUIElements();
    }

    void Die()
    {
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
