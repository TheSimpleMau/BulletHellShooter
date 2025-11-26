using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Controla las fases de combate y disparos del Jefe.
/// </summary>
public class BossWeapon : MonoBehaviour
{
    [System.Serializable]
    public class CombatPhase
    {
        public string name;
        public AttackPattern pattern;
        public float duration;

        [Header("Minions")]
        public WaveManager waveManager;
        public bool spawnWaves = false;
        public bool waitUntilWavesFinish = true;
    }

    [Header("Referencias")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Secuencia de Combate")]
    [Tooltip("Lista de fases que el jefe ejecutará en bucle.")]
    public List<CombatPhase> battlePhases;

    [HideInInspector] public float currentAngle = 270f;
    
    private AttackPattern currentPattern;
    private int phaseIndex = 0;
    private float phaseTimer = 0f;
    private float shootTimer = 0f;
    
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
        phaseIndex = 0;
        
        if (battlePhases.Count > 0)
        {
            StartPhase(0);
        }
        else
        {
            Debug.LogWarning("¡El Boss no tiene fases de ataque configuradas en el Inspector!");
        }
    }

    void Update()
    {
        if (!canShoot || battlePhases.Count == 0) return;
        HandlePhaseSwitching();
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
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0)
        {
            phaseIndex++;
            if (phaseIndex >= battlePhases.Count)
            {
                phaseIndex = 0;
            }

            StartPhase(phaseIndex);
        }
    }

    /// <summary>
    /// Inicia una fase concreta (cambia patrón, tiempo y puede lanzar oleadas).
    /// </summary>
    void StartPhase(int index)
    {
        CombatPhase phase = battlePhases[index];
        currentPattern = phase.pattern;
        phaseTimer = phase.duration;
        if (phase.spawnWaves && phase.waveManager != null)
        {
            phase.waveManager.StartWaves();
            if (phase.waitUntilWavesFinish)
            {
                canShoot = false;
                StartCoroutine(ResumeAfterWaves(phase.waveManager));
            }
        }
        shootTimer = 0f;
    }

    /// <summary>
    /// Espera a que termine un WaveManager para reanudar disparo.
    /// </summary>
    IEnumerator ResumeAfterWaves(WaveManager wm)
    {
        yield return new WaitUntil(() => wm.AreWavesFinished);
        canShoot = true;
    }

    /// <summary>
    /// Maneja el temporizador de disparo y delega el PerformAttack al patrón actual.
    /// </summary>
    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            currentPattern.PerformAttack(this);
            shootTimer = currentPattern.fireRate;
        }
    }
}