using UnityEngine;

/// <summary>
/// Configuración de movimiento para enemigos (Lineal, Onda, Circular).
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyPattern", menuName = "Patterns/Enemy Movement Preset")]
public class MovementPattern : ScriptableObject
{
    public enum MoveType
    {
        Linear,
        SineWave,
        Circular
    }

    [Header("Configuración General")]
    public MoveType movementType = MoveType.Linear;
    public float verticalSpeed = 2f;
    public float horizontalSpeed = 0f;

    [Header("Solo para Ondas y Circular")]
    public float frequency = 2f;
    public float magnitude = 2f;

    /// <summary>
    /// Devuelve el vector de oscilación (sinusoidal o circular) basado en el tiempo.
    /// </summary>
    public Vector2 CalculateOscillation(float time)
    {
        switch (movementType)
        {
            case MoveType.SineWave:
                return new Vector2(Mathf.Sin(time * frequency) * magnitude, 0);

            case MoveType.Circular:
                return new Vector2(Mathf.Cos(time * frequency) * magnitude, Mathf.Sin(time * frequency) * magnitude);

            case MoveType.Linear:
            default:
                return Vector2.zero; 
        }
    }
}