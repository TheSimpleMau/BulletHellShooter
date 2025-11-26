using UnityEngine;
using System; 
using System.Collections; 

/// <summary>
/// Movimiento de entrada en ZigZag y comportamientos de combate (random, circle, static).
/// </summary>
public class BossMovement : MonoBehaviour
{
    // Evento para avisar que la entrada terminó
    public event Action OnPositionReached;

    [Header("Configuración de Entrada (ZigZag)")]
    public float entrySpeed = 3f;
    public float stopXPosition = 0f;
    public float stopYPosition = 3.5f;

    [Header("ZigZag Dinámico")]
    public float maxWaveAmplitude = 10f;
    public float waveFrequency = 10f;

    [Header("Fase de Combate")]
    public float combatMoveSpeed = 5f; 
    public float circleRadius = 3.5f; // Radio más grande como pediste

    // Tiempos exactos
    private float attackDuration = 10f;
    private float restDuration = 2f;

    // Estados
    private enum BossState { Entering, Resting, Static, Random, Circle }
    private BossState currentState = BossState.Entering;

    // Variables internas Entrada
    private Vector3 ghostPosition;
    private Vector3 targetPosition;
    private float totalJourneyDistance;
    private bool hasArrived = false;

    // Variables internas Combate
    private Vector2 randomTarget;
    private float circleAngle;

    void Start()
    {
        // Configuración inicial de entrada
        ghostPosition = transform.position;
        targetPosition = new Vector3(stopXPosition, stopYPosition, transform.position.z);
        totalJourneyDistance = Vector3.Distance(ghostPosition, targetPosition);
        currentState = BossState.Entering;
    }

    void Update()
    {
        if (!hasArrived)
        {
            MoveSmoothEntry();
        }
        else
        {
            HandleCombatBehavior();
        }
    }

    /// <summary>
    /// Mueve al boss desde la entrada hacia la posición objetivo con una oscilación decreciente.
    /// </summary>
    void MoveSmoothEntry()
    {
        ghostPosition = Vector3.MoveTowards(ghostPosition, targetPosition, entrySpeed * Time.deltaTime);
        float remainingDistance = Vector3.Distance(ghostPosition, targetPosition);
        float dampingFactor = (totalJourneyDistance > 0) ? (remainingDistance / totalJourneyDistance) : 0f;
        float currentAmplitude = maxWaveAmplitude * dampingFactor;
        float xOffset = Mathf.Sin(Time.time * waveFrequency) * currentAmplitude;

        transform.position = ghostPosition + (Vector3.right * xOffset);

        if (remainingDistance < 0.01f)
        {
            hasArrived = true;
            transform.position = targetPosition;
            
            OnPositionReached?.Invoke(); // Avisamos que llegamos

            StartCoroutine(CombatLoop()); // Iniciamos el ciclo de combate
        }
    }

    void HandleCombatBehavior()
    {
        // MoveTowards asegura transiciones suaves desde cualquier punto
        switch (currentState)
        {
            case BossState.Resting:
            case BossState.Static:
                // Se mueve hacia el centro (0,0)
                transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, combatMoveSpeed * Time.deltaTime);
                break;

            case BossState.Random:
                MoveRandomlyFluently();
                break;

            case BossState.Circle:
                MoveInCircle();
                break;
        }
    }

    void MoveRandomlyFluently()
    {
        transform.position = Vector3.MoveTowards(transform.position, randomTarget, combatMoveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, randomTarget) < 0.1f)
        {
            PickNewRandomPosition();
        }
    }

    /// <summary>
    /// Mueve en círculo suavemente usando MoveTowards a la posición objetivo calculada por un ángulo.
    /// </summary>
    void MoveInCircle()
    {
        circleAngle += combatMoveSpeed * Time.deltaTime;
        
        float x = Mathf.Cos(circleAngle) * circleRadius;
        float y = Mathf.Sin(circleAngle) * circleRadius;
        Vector3 targetCirclePos = new Vector3(x, y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetCirclePos, combatMoveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Loop de combate que alterna entre estados y espera tiempos de ataque/descanso.
    /// </summary>
    IEnumerator CombatLoop()
    {
        while (true)
        {
            int randomPick = UnityEngine.Random.Range(1, 4);
            if (randomPick == 1) currentState = BossState.Static;
            else if (randomPick == 2) 
            {
                currentState = BossState.Random;
                PickNewRandomPosition();
            }
            else if (randomPick == 3) 
            {
                currentState = BossState.Circle;
                circleAngle = Mathf.Atan2(transform.position.y, transform.position.x);
            }

            yield return new WaitForSeconds(attackDuration);

            currentState = BossState.Resting;
            yield return new WaitForSeconds(restDuration);
        }
    }

    void PickNewRandomPosition()
    {
        float x = UnityEngine.Random.Range(-7f, 7f);
        float y = UnityEngine.Random.Range(0f, 4.5f);
        randomTarget = new Vector2(x, y);
    }
}