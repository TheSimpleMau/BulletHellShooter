using UnityEngine;

/// <summary>
/// Proyectil de enemigo que se mueve en una dirección (inicializable) y notifica al StageManager.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Configuración Base")]
    public float speed = 6f;
    private Vector2 moveDirection;
    private bool isInitialized = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        if (!isInitialized)
        {
            moveDirection = Vector2.down;
            isInitialized = true;
            Debug.LogWarning($"EnemyProjectile: Initialize() no fue llamado antes de Start() en '{gameObject.name}'. Usando dirección por defecto hacia abajo.");
        }

        if (StageManager.Instance != null)
        {
            StageManager.Instance.RegisterEnemyBullet();
        }
    }

    void OnDestroy()
    {
        if (StageManager.Instance != null) StageManager.Instance.UnregisterEnemyBullet();
    }

    /// <summary>
    /// Inicializa la dirección y opcionalmente la velocidad.
    /// </summary>
    public void Initialize(Vector2 direction, float speedOverride = -1f)
    {
        if (direction == Vector2.zero) direction = Vector2.down;
        moveDirection = direction.normalized;

        if (speedOverride > 0f)
            speed = speedOverride;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        isInitialized = true;
    }

    /// <summary>
    /// Mueve la bala en FixedUpdate usando Rigidbody2D.MovePosition.
    /// </summary>
    void FixedUpdate()
    {
        if (!isInitialized) return;
        Vector2 newPosition = rb.position + (moveDirection * speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    /// <summary>
    /// Cuando colisiona con Player aplica daño y se destruye.
    /// </summary>
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitInfo.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }
}