using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Entrada")]
    // Aseguramos que el valor por defecto sea el que pides
    public Vector2 entryTargetPosition = new Vector2(-6.2f, 3f); 
    public float entrySpeed = 5f; 
    private bool isControllable = false; 

    [Header("Velocidades de Juego")]
    public float normalSpeed = 8f;
    public float precisionSpeed = 4f;

    // Referencias internas
    private Rigidbody2D rb;
    private Vector2 rawInput;
    private bool isSlowMode = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.defaultActionMap = "Player";
            playerInput.ActivateInput();
        }

        // TRUCO: Si la nave ya está muy cerca del destino al iniciar, no se moverá.
        // Asegúrate en la escena de colocar la nave lejos (ej: 0, -10).
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (isControllable) 
            rawInput = context.ReadValue<Vector2>();
        else
            rawInput = Vector2.zero;
    }

    public void OnSlowInput(InputAction.CallbackContext context)
    {
        if (context.performed) isSlowMode = true;
        if (context.canceled) isSlowMode = false;
    }

    void FixedUpdate()
    {
        if (!isControllable)
        {
            HandleEntryAnimation();
        }
        else
        {
            HandleMovement();
        }
    }

    void HandleEntryAnimation()
    {
        // Movemos la nave hacia la posición objetivo exacta
        Vector2 newPos = Vector2.MoveTowards(rb.position, entryTargetPosition, entrySpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Aumenté ligeramente la tolerancia a 0.05f para asegurar que detecte la llegada
        if (Vector2.Distance(rb.position, entryTargetPosition) < 0.05f)
        {
            isControllable = true;
            // Forzamos la posición exacta final para evitar decimales sueltos
            rb.position = entryTargetPosition;
        }
    }

    void HandleMovement()
    {
        float currentSpeed = isSlowMode ? precisionSpeed : normalSpeed;
        Vector2 newPos = rb.position + (rawInput * currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}