using UnityEngine;

[CreateAssetMenu(fileName = "New Flower S-Curve", menuName = "Boss/Patterns/FlowerClosed")]
public class FlowerClosed : AttackPattern
{
    [Header("Configuración de la Flor Cerrada")]
    public int numberOfStreams = 5;
    public float sCurveAmplitude = 30f; 
    public float sCurveFrequency = 1f; 
    public float rotationSpeed = 20f;

    [Header("Modo de Inversión")]
    public bool shootInvertedPair = true;

    [Header("Corrección Visual")]
    public float startPhaseShift = 1.5f;

    public override void PerformAttack(BossWeapon weapon)
    {
        float streamAngleStep = 360f / numberOfStreams;
        float timeCalculation = (Time.time * sCurveFrequency) + startPhaseShift;
        float globalOscillation = Mathf.Sin(timeCalculation) * sCurveAmplitude;

        for (int i = 0; i < numberOfStreams; i++)
        {
            float baseStreamAngle = (streamAngleStep * i) + weapon.currentAngle;

            // Chorro A
            float finalAngleA = baseStreamAngle + globalOscillation;
            FireBullet(weapon, finalAngleA);

            // Chorro B
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
        Vector2 direction = Quaternion.Euler(0, 0, angleInDegrees) * Vector3.right;
        GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
        EnemyProjectile projectile = bullet.GetComponent<EnemyProjectile>();
        projectile.Initialize(direction);
    }
}