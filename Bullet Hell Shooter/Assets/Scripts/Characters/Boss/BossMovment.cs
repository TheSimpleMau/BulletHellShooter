using UnityEngine;
using System; // Necesario para usar 'Action'

/// <summary>
/// Controla exclusivamente el desplazamiento de entrada del Jefe.
/// Emite un evento cuando alcanza su posición final.
/// </summary>
public class BossMovement : MonoBehaviour
{
    // --- Evento para avisar a otros scripts ---
    /// <summary>
    /// Se ejecuta cuando el jefe termina su entrada y está listo para combatir.
    /// </summary>
    public event Action OnPositionReached;

    [Header("Configuración de Movimiento")]
    public float entrySpeed = 3f;
    public float stopXPosition = 0f;
    public float stopYPosition = 0f;

    [Header("ZigZag Dinámico")]
    public float maxWaveAmplitude = 3f;
    public float waveFrequency = 5f;

    // Estado interno
    private Vector3 ghostPosition;
    private Vector3 targetPosition;
    private float totalJourneyDistance;
    private bool hasArrived = false;

    /// <summary>
    /// Inicializa posiciones y distancia total.
    /// </summary>
    void Start()
    {
        ghostPosition = transform.position;
        targetPosition = new Vector3(stopXPosition, stopYPosition, transform.position.z);
        totalJourneyDistance = Vector3.Distance(ghostPosition, targetPosition);
    }

    /// <summary>
    /// Solo ejecuta la lógica de movimiento si no ha llegado.
    /// </summary>
    void Update()
    {
        if (!hasArrived)
        {
            MoveSmooth();
        }
    }

    /// <summary>
    /// Lógica de movimiento amortiguado (Embudo).
    /// </summary>
    void MoveSmooth()
    {
        ghostPosition = Vector3.MoveTowards(ghostPosition, targetPosition, entrySpeed * Time.deltaTime);
        float remainingDistance = Vector3.Distance(ghostPosition, targetPosition);

        // Cálculo del factor de amortiguación (1 = lejos, 0 = llegó)
        float dampingFactor = (totalJourneyDistance > 0) ? (remainingDistance / totalJourneyDistance) : 0f;
        
        float currentAmplitude = maxWaveAmplitude * dampingFactor;
        float xOffset = Mathf.Sin(Time.time * waveFrequency) * currentAmplitude;

        transform.position = ghostPosition + (Vector3.right * xOffset);

        // Verificación de llegada
        if (remainingDistance < 0.01f)
        {
            hasArrived = true;
            
            // Aseguramos posición final perfecta
            transform.position = targetPosition;

            // ¡AVISO IMPORTANTE! Aquí avisamos al arma (o a quien escuche) que ya llegamos.
            // El "?." significa: "Si hay alguien escuchando, avísale".
            OnPositionReached?.Invoke();
        }
    }
}