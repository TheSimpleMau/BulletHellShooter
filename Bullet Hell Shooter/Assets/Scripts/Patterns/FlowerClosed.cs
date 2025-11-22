using UnityEngine;

[CreateAssetMenu(fileName = "New Flower S-Curve", menuName = "Boss/Patterns/FlowerClosed")]
public class Flower_S_Curve : AttackPattern
{
    [Header("Configuración de la Flor Cerrada")]
    public int numberOfStreams = 5;
    public float sCurveAmplitude = 30f; 
    public float sCurveFrequency = 1f; 
    public float rotationSpeed = 20f;

    [Header("Modo de Inversión")]
    public bool shootInvertedPair = true;

    [Header("Corrección Visual")]
    [Tooltip("Mueve el inicio de la onda para que no empiece recta. Prueba con valores como 1.5 o 4.")]
    public float startPhaseShift = 1.5f; // Valor por defecto que suele funcionar bien

    public override void PerformAttack(BossWeapon weapon)
    {
        float streamAngleStep = 360f / numberOfStreams;

        // --- CORRECCIÓN AQUÍ ---
        // Sumamos 'startPhaseShift' dentro del Seno.
        // Esto engaña a la matemática haciéndole creer que ya ha pasado tiempo.
        float timeCalculation = (Time.time * sCurveFrequency) + startPhaseShift;
        
        float globalOscillation = Mathf.Sin(timeCalculation) * sCurveAmplitude;

        for (int i = 0; i < numberOfStreams; i++)
        {
            float baseStreamAngle = (streamAngleStep * i) + weapon.currentAngle;

            // Chorro A
            float finalAngleA = baseStreamAngle + globalOscillation;
            FireBullet(weapon, finalAngleA);

            // Chorro B (Invertido)
            if (shootInvertedPair)
            {
                float finalAngleB = baseStreamAngle - globalOscillation;
                FireBullet(weapon, finalAngleB);
            }
        }

        weapon.currentAngle += rotationSpeed * Time.deltaTime;
    }

    void FireBullet(BossWeapon weapon, float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyProjectile>().Initialize(direction);
    }
}