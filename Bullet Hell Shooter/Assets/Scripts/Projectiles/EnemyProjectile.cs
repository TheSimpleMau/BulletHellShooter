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
        // Seguridad por si olvidamos inicializarla
        if (!isInitialized) Destroy(gameObject, 0.1f);
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        // angle -= 90f; // Descomenta si tu sprite mira hacia arriba
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        isInitialized = true;
    }

    void FixedUpdate()
    {
        if (!isInitialized) return;
        Vector2 newPosition = rb.position + (moveDirection * speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }
}