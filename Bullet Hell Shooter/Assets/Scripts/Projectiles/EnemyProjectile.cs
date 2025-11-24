using UnityEngine;

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
    }

    void Start()
    {
        if (!isInitialized) Destroy(gameObject, 0.1f);
        if (StageManager.Instance != null)
        {
            StageManager.Instance.RegisterEnemyBullet();
        }
    }

    void OnDestroy()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.UnregisterEnemyBullet();
        }
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        isInitialized = true;
    }

    void FixedUpdate()
    {
        if (!isInitialized) return;
        Vector2 newPosition = rb.position + (moveDirection * speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

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