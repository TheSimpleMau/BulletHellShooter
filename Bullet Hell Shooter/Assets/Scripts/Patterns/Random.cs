using UnityEngine;

/// <summary>
/// Patrón que dispara ráfagas de balas en direcciones aleatorias.
/// </summary>
[CreateAssetMenu(fileName = "New Random Pattern", menuName = "Boss/Patterns/Random")]
public class RandomAttack : AttackPattern
{
    [Header("Configuración Aleatoria")]
    public int bulletsPerShot = 5;
    public float speedMultiplier = 2f;
    
    /// <summary>
    /// Lanza varias balas con ángulos al azar y velocidad aumentada.
    /// </summary>
    public override void PerformAttack(BossWeapon weapon)
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            float randomAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
            Vector2 direction = rotation * Vector3.right;

            GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
            EnemyProjectile projectile = bullet.GetComponent<EnemyProjectile>();
            
            projectile.speed *= speedMultiplier; 
            projectile.Initialize(direction);
        }
    }
}