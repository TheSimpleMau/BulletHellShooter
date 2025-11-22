using UnityEngine;

/// <summary>
/// Lógica específica para disparar en espiral.
/// </summary>
[CreateAssetMenu(fileName = "New Spiral Pattern", menuName = "Boss/Patterns/Spiral")]
public class Spiral : AttackPattern
{
    [Header("Configuración Espiral")]
    public float angleStep = 10f;

    /// <summary>
    /// Ejecuta un disparo y rota el ángulo del arma para el siguiente.
    /// </summary>
    public override void PerformAttack(BossWeapon weapon)
    {
        // 1. Calcular dirección basada en el ángulo actual del arma
        float radians = weapon.currentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        // 2. Instanciar la bala
        GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
        
        // 3. Inicializarla
        bullet.GetComponent<EnemyProjectile>().Initialize(direction);

        // 4. Modificar el estado del arma para el siguiente frame
        weapon.currentAngle += angleStep;
        if (weapon.currentAngle >= 360f) weapon.currentAngle -= 360f;
    }
}