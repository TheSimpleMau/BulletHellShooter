using UnityEngine;

/// <summary>
/// Proyectil usado por minions (usa Rigidbody2D.linearVelocity y se destruye con lifetime).
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MinionProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    public int damage = 1;
    public float lifeTime = 6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// Establece la velocidad inicial y programa destrucción por tiempo.
    /// </summary>
    public void Initialize(Vector2 initialVelocity)
    {
        if (rb != null) rb.linearVelocity = initialVelocity;
        
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Al chocar con Player aplica daño y se destruye; con otros colliders no trigger también se destruye.
    /// </summary>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var ph = col.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);

            Destroy(gameObject);
        }
        else
        {
            if (!col.isTrigger)
                Destroy(gameObject);
        }
    }
}
