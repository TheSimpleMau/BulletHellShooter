using UnityEngine;

/// <summary>
/// Clase base abstracta para cualquier patrón de ataque.
/// No se puede usar directamente, sirve de plantilla para crear la Espiral, el Círculo, etc.
/// </summary>
public abstract class AttackPattern : ScriptableObject
{
    [Header("Configuración General")]
    public float fireRate = 0.1f;
    
    /// <summary>
    /// Método para que cada patrón utilice su propia lógica matemática.
    /// </summary>
    public abstract void PerformAttack(BossWeapon weapon);
}