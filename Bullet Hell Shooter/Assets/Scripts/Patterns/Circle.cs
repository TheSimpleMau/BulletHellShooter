using UnityEngine;

/// <summary>
/// Patrón que dispara balas en 360 grados simultáneamente.
/// </summary>
[CreateAssetMenu(fileName = "New Circle Pattern", menuName = "Boss/Patterns/Circle")]
public class Circle : AttackPattern
{
    [Header("Configuración Círculo")]
    public int bulletCount = 12;

    /// <summary>
    /// Calcula el ángulo para cada bala y las instancia en círculo.
    /// </summary>
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