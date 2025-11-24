using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 15f; // Más rápidas que las del enemigo

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        // Registramos como bala de JUGADOR
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

    void FixedUpdate()
    {
        // Siempre hacia ARRIBA (Vector2.up)
        Vector2 newPosition = rb.position + (Vector2.up * speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Debug para ver si choca con ALGO

        if (hitInfo.CompareTag("Boss"))
        {
            BossHealth bossHealth = hitInfo.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(1);
                Debug.Log("¡Jefe dañado!"); // Confirmación en consola
            }
            Destroy(gameObject);
        }
    }
}