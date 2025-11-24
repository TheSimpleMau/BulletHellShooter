using UnityEngine;
using UnityEngine.InputSystem;

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
        // Buscamos al jefe en la escena
        BossMovement boss = Object.FindFirstObjectByType<BossMovement>();
        
        if (boss != null)
        {
            // Si existe el jefe, nos suscribimos a su evento.
            // Cuando él llegue a su sitio, ejecutará 'EnableShooting'
            boss.OnPositionReached += EnableShooting;
        }
        else
        {
            // Si NO encuentras al jefe (ej: estás probando solo la nave en una escena vacía),
            // activamos el disparo inmediatamente para que puedas jugar.
            canShoot = true;
        }
    }

    // Evento FIRE (Espacio)
    public void OnFireInput(InputAction.CallbackContext context)
    {
        // En modo manual, escuchamos si aprietas el botón
        if (!isAutoFire)
        {
            if (context.performed) isHoldingFireButton = true;
            if (context.canceled) isHoldingFireButton = false;
        }
    }

    // Evento SWITCH MODE (Tecla M)
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

        // Lógica de Modos
        bool shouldShoot = false;

        if (isAutoFire)
        {
            // MODO INFINITO: Siempre dispara, ignoramos el input
            shouldShoot = true;
        }
        else
        {
            // MODO MANUAL: Dispara solo si mantenemos el botón
            shouldShoot = isHoldingFireButton;
        }

        // Ejecución del disparo
        if (shouldShoot && shootTimer <= 0)
        {
            AttemptShoot();
            shootTimer = fireRate;
        }
        if (!canShoot) return; 
    }

    void AttemptShoot()
    {
        if (!canShoot) return; 
        if (bulletPrefab == null || firePoints == null) return;

        foreach (Transform point in firePoints)
        {
            // Rotamos 90 grados en el eje Z para que apunte hacia arriba
            if (point != null) Instantiate(bulletPrefab, point.position, Quaternion.Euler(0, 0, 90));
        }
    }
}