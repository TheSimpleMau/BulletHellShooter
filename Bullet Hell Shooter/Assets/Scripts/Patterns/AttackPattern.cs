using UnityEngine;

/// <summary>
/// Clase base abstracta para cualquier patrón de ataque.
/// </summary>
public abstract class AttackPattern : ScriptableObject
{
    [Header("Configuración General")]
    public float fireRate = 0.1f;
    
    /// <summary>
    /// Ejecuta la lógica específica del disparo (Círculo, Espiral, etc.).
    /// </summary>
    public abstract void PerformAttack(BossWeapon weapon);
}