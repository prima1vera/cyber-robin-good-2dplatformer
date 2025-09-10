using System.Collections;
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

        // Стартовая скорость (вправо/влево). Гравитация у Rigidbody2D задаст падение.
        rb.velocity = new Vector2(dir * speed, 0f);

        // НИКАКОГО флипа scale — только поворот по скорости
        AlignToVelocity();

        Destroy(gameObject, maxLifetime);
    }

    void FixedUpdate()
    {
        if (stuck) return;
        AlignToVelocity();
    }

    void AlignToVelocity()
    {
        if (rb.velocity.sqrMagnitude > 0.0001f)
        {
            // Пусть спрайт «смотрит» вдоль +X: вращаем так, чтобы +X совпал с направлением скорости
            transform.right = rb.velocity.normalized;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (stuck) return;
        if (other.gameObject.layer == shooterLayer) return; // не бьём себя

        var health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            // Контекст удара отдаём в Health — он разрулит флэш/нокаут
            health.TakeDamage(damage, transform, hitPoint);
        }

        StickTo(other.transform);
    }

    void StickTo(Transform target)
    {
        stuck = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        col.enabled = false;
        transform.SetParent(target, true); // «воткнулись» и едем с целью
    }
}
