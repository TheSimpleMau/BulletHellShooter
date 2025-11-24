using UnityEngine;

/// <summary>
/// Patrón de Explosión: Dispara múltiples balas en todas direcciones al mismo tiempo.
/// </summary>
[CreateAssetMenu(fileName = "New Circle Pattern", menuName = "Boss/Patterns/Circle")]
public class Circle : AttackPattern
{
    [Header("Configuración Círculo")]
    [Tooltip("Cuántas balas saldrán en la explosión")]
    public int bulletCount = 12;

    public override void PerformAttack(BossWeapon weapon)
    {
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            Vector2 direction = rotation * Vector3.right;
            GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
            EnemyProjectile projectile = bullet.GetComponent<EnemyProjectile>();
            projectile.Initialize(direction);
        }
    }
}