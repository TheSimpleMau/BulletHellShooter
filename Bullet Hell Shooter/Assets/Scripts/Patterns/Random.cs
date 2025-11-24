using UnityEngine;

[CreateAssetMenu(fileName = "New Random Pattern", menuName = "Boss/Patterns/Random")]
public class RandomAttack : AttackPattern
{
    [Header("Configuración Aleatoria")]
    public int bulletsPerShot = 5; // Cuántas balas lanza de golpe
    public float speedMultiplier = 2f; // Para que sean balas rápidas

    public override void PerformAttack(BossWeapon weapon)
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            // 1. Elegir un ángulo aleatorio entre 0 y 360
            float randomAngle = Random.Range(0f, 360f);

            // 2. USANDO QUATERNION.EULER (Requisito)
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
            Vector2 direction = rotation * Vector3.right;

            // 3. Instanciar
            GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
            EnemyProjectile projectile = bullet.GetComponent<EnemyProjectile>();
            
            // 4. Aumentar velocidad solo para estas balas
            projectile.speed *= speedMultiplier; 
            
            projectile.Initialize(direction);
        }
    }
}