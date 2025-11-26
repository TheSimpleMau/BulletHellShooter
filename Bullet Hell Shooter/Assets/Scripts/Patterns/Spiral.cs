using UnityEngine;

/// <summary>
/// Patrón que dispara balas rotando el ángulo disparo tras disparo.
/// </summary>
[CreateAssetMenu(fileName = "New Spiral Pattern", menuName = "Boss/Patterns/Spiral")]
public class Spiral : AttackPattern
{
    [Header("Configuración Espiral")]
    public float angleStep = 10f;
    
    /// <summary>
    /// Dispara una bala y rota el punto de emisión para la siguiente.
    /// </summary>
    public override void PerformAttack(BossWeapon weapon)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, weapon.currentAngle);
        Vector2 direction = rotation * Vector3.right;
        GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
        EnemyProjectile projectile = bullet.GetComponent<EnemyProjectile>();
        projectile.Initialize(direction);
        weapon.currentAngle += angleStep;
        if (weapon.currentAngle >= 360f) weapon.currentAngle -= 360f;
    }
}