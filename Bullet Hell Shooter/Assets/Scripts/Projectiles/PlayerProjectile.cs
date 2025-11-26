using UnityEngine;

/// <summary>
/// Proyectil del jugador: va hacia arriba y daña minions/boss.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 15f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.RegisterPlayerBullet();
        }
    }

    void OnDestroy()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.UnregisterPlayerBullet();
        }
    }

    /// <summary>
    /// Mueve la bala hacia arriba en FixedUpdate.
    /// </summary>
    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + (Vector2.up * speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }
    /// <summary>
    /// Al chocar aplica daño a Boss/Minion y se destruye.
    /// </summary>
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Boss"))
        {
            BossHealth bossHealth = hitInfo.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        if (hitInfo.CompareTag("Minion"))
        {
            MinionHealth minionHealth = hitInfo.GetComponent<MinionHealth>();
            if (minionHealth != null)
            {
                minionHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}