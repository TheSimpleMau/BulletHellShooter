using UnityEngine;

/// <summary>
/// Movimiento de minions: mueve el centro y aplica oscilación del MovementPattern, así como rebotes en los límites.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MinionMovement : MonoBehaviour
{
    public MovementPattern movementPattern;

    [Header("Límites")]
    public float screenPadding = 0.5f;
    private Rigidbody2D rb;
    private Vector2 currentCenterPos; 
    private int dirX = 1; 
    private int dirY = -1;

    private float timeCounter = 0f;
    private bool isInitialized = false;
    private Vector2 minScreenBounds;
    private Vector2 maxScreenBounds;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        CalculateScreenBounds();

        if (!isInitialized && movementPattern != null)
        {
            InitializeAtSpawn(transform.position);
        }
    }

    void CalculateScreenBounds()
    {
        Camera mainCam = Camera.main;
        minScreenBounds = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        maxScreenBounds = mainCam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minScreenBounds.x += screenPadding;
        minScreenBounds.y += screenPadding;
        maxScreenBounds.x -= screenPadding;
        maxScreenBounds.y -= screenPadding;
    }

    /// <summary>
    /// Calcula y aplica movimiento en FixedUpdate: traslado del centro, chequeo de rebotes y aplicar oscilación.
    /// </summary>
    void FixedUpdate()
    {
        if (movementPattern == null || !isInitialized) return;

        timeCounter += Time.fixedDeltaTime;
        float moveX = movementPattern.horizontalSpeed * Time.fixedDeltaTime * dirX;
        float moveY = movementPattern.verticalSpeed * Time.fixedDeltaTime * dirY;
        currentCenterPos += new Vector2(moveX, moveY);

        // Eje X (Izquierda / Derecha)
        if (currentCenterPos.x >= maxScreenBounds.x)
        {
            currentCenterPos.x = maxScreenBounds.x;
            dirX = -1;
        }
        else if (currentCenterPos.x <= minScreenBounds.x)
        {
            currentCenterPos.x = minScreenBounds.x;
            dirX = 1;
        }

        // Eje Y (Arriba / Abajo)
        if (currentCenterPos.y <= minScreenBounds.y)
        {
            currentCenterPos.y = minScreenBounds.y;
            dirY = 1;
        }
        else if (currentCenterPos.y >= maxScreenBounds.y)
        {
            currentCenterPos.y = maxScreenBounds.y;
            dirY = -1;
        }

        Vector2 oscillation = movementPattern.CalculateOscillation(timeCounter);
        rb.MovePosition(currentCenterPos + oscillation);
    }

    /// <summary>
    /// Inicializa la posición y estados al spawnear.
    /// </summary>
    public void InitializeAtSpawn(Vector2 spawnPos)
    {
        CalculateScreenBounds();
        currentCenterPos = spawnPos;
        transform.position = spawnPos;
        timeCounter = 0f;
        dirY = -1;
        dirX = 1; 
        isInitialized = true;
    }
}