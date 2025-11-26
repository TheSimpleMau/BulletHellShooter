using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lógica de disparo del jugador: modos auto/manual, puntos de disparo y temporizador.
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject bulletPrefab;
    public Transform[] firePoints;

    [Header("Configuración")]
    public float fireRate = 0.1f;
    
    [Tooltip("True: Disparo Infinito (Automático). False: Disparo Manual (Espacio).")]
    public bool isAutoFire = true;

    private bool isHoldingFireButton; // Solo usado en modo manual
    private float shootTimer;
    private bool canShoot = false; 

    void Start()
    {
        canShoot = false;
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.OnPlayerReady += InitializeShootingLogic;
        }
        else
        {
            InitializeShootingLogic();
        }
    }

    /// <summary>
    /// Inicializa la lógica de disparo cuando el jugador y (opcionalmente) el boss están listos.
    /// </summary>
    void InitializeShootingLogic()
    {
        BossMovement boss = Object.FindFirstObjectByType<BossMovement>();

        if (boss != null)
        {
            boss.OnPositionReached += EnableShooting;
        }
        else
        {
            EnableShooting();
        }
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (!isAutoFire)
        {
            if (context.performed) isHoldingFireButton = true;
            if (context.canceled) isHoldingFireButton = false;
        }
    }

    public void OnSwitchModeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAutoFire = !isAutoFire;
            isHoldingFireButton = false;
        }
    }

    void EnableShooting()
    {
        canShoot = true;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        bool shouldShoot = false;

        if (isAutoFire)
        {
            shouldShoot = true;
        }
        else
        {
            shouldShoot = isHoldingFireButton;
        }
        if (shouldShoot && shootTimer <= 0)
        {
            AttemptShoot();
            shootTimer = fireRate;
        }
        if (!canShoot) return; 
    }

    /// <summary>
    /// Intenta disparar desde todos los firePoints si está permitido.
    /// </summary>
    void AttemptShoot()
    {
        if (!canShoot) return; 
        if (bulletPrefab == null || firePoints == null) return;

        foreach (Transform point in firePoints)
        {
            if (point != null) Instantiate(bulletPrefab, point.position, Quaternion.Euler(0, 0, 90));
        }
    }
}