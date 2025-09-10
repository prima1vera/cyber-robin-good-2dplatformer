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
    int shooterLayer; // ����� �������� ������ �������

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

        // ��������� �������� (������/�����). ���������� � Rigidbody2D ������ �������.
        rb.velocity = new Vector2(dir * speed, 0f);

        // �������� ����� scale � ������ ������� �� ��������
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
            // ����� ������ �������� ����� +X: ������� ���, ����� +X ������ � ������������ ��������
            transform.right = rb.velocity.normalized;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (stuck) return;
        if (other.gameObject.layer == shooterLayer) return; // �� ���� ����

        var health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            // �������� ����� ����� � Health � �� �������� ����/������
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
        transform.SetParent(target, true); // ������������ � ���� � �����
    }
}
