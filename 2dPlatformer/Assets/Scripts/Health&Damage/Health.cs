using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the health state of a game object.
/// 
/// Implementation Notes: 2D Rigidbodies must be set to never sleep for this to interact with trigger stay damage
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 0;

    [Header("Health Settings")]
    [Tooltip("The default health value")]
    public int defaultHealth = 1;
    [Tooltip("The maximum health value")]
    public int maximumHealth = 1;
    [Tooltip("The current in game health value")]
    public int currentHealth = 1;
    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float invincibilityTime = 3f;

    [Header("Lives settings")]
    [Tooltip("Whether or not to use lives")]
    public bool useLives = false;
    [Tooltip("Current number of lives this health has")]
    public int currentLives = 3;
    [Tooltip("The maximum number of lives this health has")]
    public int maximumLives = 5;
    [Tooltip("The amount of time to wait before respawning")]
    public float respawnWaitTime = 3f;

    // The time to respawn at
    private float respawnTime;
    // The specific game time when the health can be damaged again
    private float timeToBecomeDamagableAgain = 0;
    // Whether or not the health is invincible
    public bool isInvincible = false;
    // The position that the health's gameobject will respawn at
    private Vector3 respawnPosition;

    [Header("Effects & Polish")]
    [Tooltip("The effect to create when this health dies")]
    public GameObject deathEffect;
    [Tooltip("The effect to create when this health is damaged (but does not die)")]
    public GameObject hitEffect;

    [Header("Hit Reaction Settings")]
    public float knockbackForce = 5f;   // сила отбрасывания
    public float flashDuration = 0.15f; // сколько длится покраснение

    void Start()
    {
        SetRespawnPoint(transform.position);
    }

    void Update()
    {
        InvincibilityCheck();
        RespawnCheck();
    }

    private void RespawnCheck()
    {
        if (respawnWaitTime != 0 && currentHealth <= 0 && currentLives > 0)
        {
            if (Time.time >= respawnTime)
            {
                Respawn();
            }
        }
    }

    private void InvincibilityCheck()
    {
        if (timeToBecomeDamagableAgain <= Time.time)
        {
            isInvincible = false;
        }
    }

    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    void Respawn()
    {
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
        GameManager.UpdateUIElements();
    }

    /// <summary>
    /// Applies damage and plays hit reaction (knockback + flash).
    /// </summary>
    public void TakeDamage(int damageAmount, Vector2? hitSource = null)
    {
        if (isInvincible || currentHealth <= 0)
        {
            return;
        }
        else
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }

            timeToBecomeDamagableAgain = Time.time + invincibilityTime;
            isInvincible = true;
            currentHealth -= damageAmount;

            // эффекты попадания
            StartCoroutine(HitFlash());
            if (hitSource.HasValue)
            {
                ApplyKnockback((Vector2)hitSource);
            }

            CheckDeath();
        }
        GameManager.UpdateUIElements();
    }

    public void ReceiveHealing(int healingAmount)
    {
        currentHealth += healingAmount;
        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
        CheckDeath();
        GameManager.UpdateUIElements();
    }

    public void AddLives(int bonusLives)
    {
        if (useLives)
        {
            currentLives += bonusLives;
            if (currentLives > maximumLives)
            {
                currentLives = maximumLives;
            }
            GameManager.UpdateUIElements();
        }
    }

    bool CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation, null);
        }

        if (useLives)
        {
            currentLives -= 1;
            if (currentLives > 0)
            {
                if (respawnWaitTime == 0)
                {
                    Respawn();
                }
                else
                {
                    respawnTime = Time.time + respawnWaitTime;
                }
            }
            else
            {
                if (respawnWaitTime != 0)
                {
                    respawnTime = Time.time + respawnWaitTime;
                }
                else
                {
                    Destroy(this.gameObject);
                }
                GameOver();
            }
        }
        else
        {
            GameOver();
            Destroy(this.gameObject);
        }
        GameManager.UpdateUIElements();
    }

    public void GameOver()
    {
        if (GameManager.instance != null && gameObject.tag == "Player")
        {
            GameManager.instance.GameOver();
        }
    }

    // 🔹 Дополнительные методы для реакции на попадание
    private IEnumerator HitFlash()
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            sr.color = original;
        }
    }

    private void ApplyKnockback(Vector2 hitSource)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackDir = ((Vector2)transform.position - hitSource).normalized;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
