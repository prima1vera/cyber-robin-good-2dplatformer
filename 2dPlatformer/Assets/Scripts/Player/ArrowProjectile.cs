using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Tuning")]
    public float speed = 14f;
    public int damage = 1;
    public float maxLifetime = 3f;

    Rigidbody2D rb;
    Collider2D col;
    bool stuck;
    int shooterLayer; // чтобы игнорить своего стрелка

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Launch(float dir, float spd, int dmg, int shooterLayer)
    {
        this.speed = spd;
        this.damage = dmg;
        this.shooterLayer = shooterLayer;

        rb.gravityScale = 0f;
        rb.velocity = new Vector2(dir * speed, 0f);

        // Отзеркалим визуал при полёте влево (если надо)
        if (dir < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        Destroy(gameObject, maxLifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (stuck) return;
        if (other.gameObject.layer == shooterLayer) return; // не бьём себя

        // Наносим урон, если есть Health
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            // примерный вызов — подстрой под свой Health
            health.TakeDamage(damage);
        }

        StickTo(other.transform);
    }

    void StickTo(Transform target)
    {
        stuck = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(target, true); // «воткнулись» и едем с врагом
    }
}
