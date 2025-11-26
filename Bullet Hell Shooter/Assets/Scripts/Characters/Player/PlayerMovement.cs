using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimiento del jugador: entrada inicial hacia un punto y control por InputSystem con modo de precisión.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Entrada")]
    public Vector2 entryTargetPosition = new Vector2(-6.2f, 3f); 
    public float entrySpeed = 5f; 
    private bool isControllable = false; 

    [Header("Velocidades de Juego")]
    public float normalSpeed = 8f;
    public float precisionSpeed = 4f;

    private Rigidbody2D rb;
    private Vector2 rawInput;
    private bool isSlowMode = false;

    // Evento que indica que el jugador ya está listo para disparar.
    public event System.Action OnPlayerReady;

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
        Vector2 newPos = Vector2.MoveTowards(rb.position, entryTargetPosition, entrySpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        if (Vector2.Distance(rb.position, entryTargetPosition) < 0.05f)
        {
            isControllable = true;
            rb.position = entryTargetPosition;
            OnPlayerReady?.Invoke();
        }
    }

    void HandleMovement()
    {
        float currentSpeed = isSlowMode ? precisionSpeed : normalSpeed;
        Vector2 newPos = rb.position + (rawInput * currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}