using UnityEngine;

/// <summary>
/// Patrón de Explosión: Dispara múltiples balas en todas direcciones al mismo tiempo.
/// </summary>
[CreateAssetMenu(fileName = "New Circle Pattern", menuName = "Boss/Patterns/Circle")]
public class Circle : AttackPattern
{
    [Header("Configuración Círculo")]
    [Tooltip("Cuántas balas saldrán en la explosión")]
    public int bulletCount = 12; // Cambié 'angleStep' por 'bulletCount' porque es más descriptivo

    public override void PerformAttack(BossWeapon weapon)
    {
        // 1. Calculamos cuánto espacio hay entre cada bala para completar los 360 grados
        float angleStep = 360f / bulletCount;

        // 2. Bucle: Creamos una bala por cada "rebanada" del pastel
        for (int i = 0; i < bulletCount; i++)
        {
            // A. Calcular el ángulo específico para ESTA bala
            // Si i=0 -> 0 grados. Si i=1 -> 30 grados... etc.
            float currentAngle = i * angleStep;

            // B. Convertir a Radianes y luego a Vector (Dirección)
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            // C. Instanciar
            GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);

            // D. Inicializar inmediatamente (No hace falta guardarla en una lista)
            bullet.GetComponent<EnemyProjectile>().Initialize(direction);
        }
        
        // Nota: En un patrón circular explosivo, generalmente no necesitamos
        // modificar el 'weapon.currentAngle' porque cubrimos todo el espacio a la vez.
    }
}