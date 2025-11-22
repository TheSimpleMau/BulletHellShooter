using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controla el sistema de armamento del Jefe.
/// Gestiona una secuencia de patrones de ataque que cambian con el tiempo.
/// </summary>
public class BossWeapon : MonoBehaviour
{
    // --- DEFINICIÓN DE UNA FASE ---
    [System.Serializable]
    public class CombatPhase
    {
        public string name;             // Nombre para organizarte (ej: "Fase Espiral")
        public AttackPattern pattern;   // El ScriptableObject del patrón (NOTA: Cambié AttackPatternSO a AttackPattern según tus archivos)
        public float duration;          // Cuánto dura esta fase en segundos
    }

    [Header("Referencias")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Secuencia de Combate")]
    [Tooltip("Lista de fases que el jefe ejecutará en bucle.")]
    public List<CombatPhase> battlePhases;

    // --- Variables de Estado ---
    [HideInInspector] public float currentAngle = 270f; // Empezamos mirando abajo
    
    private AttackPattern currentPattern; // El patrón activo actual
    private int phaseIndex = 0;           // En qué número de fase vamos
    private float phaseTimer = 0f;        // Temporizador para cambiar de fase
    private float shootTimer = 0f;        // Temporizador para disparar balas
    
    private bool canShoot = false;
    private BossMovement movementScript;

    void Awake()
    {
        movementScript = GetComponent<BossMovement>();
    }
    
    void OnEnable()
    {
        if (movementScript != null) movementScript.OnPositionReached += ActivateWeapons;
    }

    void OnDisable()
    {
        if (movementScript != null) movementScript.OnPositionReached -= ActivateWeapons;
    }

    /// <summary>
    /// Se llama automáticamente cuando BossMovement termina de entrar.
    /// </summary>
    void ActivateWeapons()
    {
        canShoot = true;
        shootTimer = 0;
        phaseIndex = 0; // Empezamos por la primera fase
        
        if (battlePhases.Count > 0)
        {
            StartPhase(0); // Cargamos la fase 0
        }
        else
        {
            Debug.LogWarning("¡El Boss no tiene fases de ataque configuradas en el Inspector!");
        }
    }

    void Update()
    {
        // Si no podemos disparar o la lista está vacía, no hacemos nada
        if (!canShoot || battlePhases.Count == 0) return;

        // 1. Gestionar el cambio de Fases (La Playlist)
        HandlePhaseSwitching();

        // 2. Gestionar el Disparo (Si hay un patrón activo válido)
        if (currentPattern != null)
        {
            HandleShooting();
        }
    }

    /// <summary>
    /// Controla el reloj de la fase y cambia a la siguiente cuando se acaba el tiempo.
    /// </summary>
    void HandlePhaseSwitching()
    {
        // Restamos tiempo usando deltaTime (el mismo reloj que usa StageManager)
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0)
        {
            // Pasamos a la siguiente fase
            phaseIndex++;

            // Si llegamos al final de la lista, volvemos al principio (Bucle infinito)
            if (phaseIndex >= battlePhases.Count)
            {
                phaseIndex = 0;
            }

            StartPhase(phaseIndex);
        }
    }

    /// <summary>
    /// Carga los datos de la nueva fase (Patrón y Tiempo).
    /// </summary>
    void StartPhase(int index)
    {
        CombatPhase phase = battlePhases[index];
        
        currentPattern = phase.pattern;
        phaseTimer = phase.duration;

        // Opcional: Si quieres que el patrón "Descanse" y no dispare, 
        // simplemente deja el campo 'pattern' vacío en el Inspector para esa fase.
    }

    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            // Delegamos la matemática al ScriptableObject
            currentPattern.PerformAttack(this);
            
            // Reiniciamos el temporizador según la velocidad del patrón actual
            shootTimer = currentPattern.fireRate;
        }
    }
}