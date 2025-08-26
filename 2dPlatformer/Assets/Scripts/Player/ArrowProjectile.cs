using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 14f;
    public int damage = 1;
    public float maxLifetime = 3f;

    Rigidbody2D rb;
    Collider2D col;
    bool stuck;
    int shooterLayer;

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

        rb.velocity = new Vector2(dir * speed, 0f);

        if (dir < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        Destroy(gameObject, maxLifetime);
    }

    void FixedUpdate()
    {
        if (stuck) return;

        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (stuck) return;
        if (other.gameObject.layer == shooterLayer) return;

        var health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage, transform.position);
        }

        StickTo(other.transform);
    }

    void StickTo(Transform target)
    {
        stuck = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(target, true);
    }
}
