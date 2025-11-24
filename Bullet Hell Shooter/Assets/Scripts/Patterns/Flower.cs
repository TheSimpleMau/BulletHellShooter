using UnityEngine;

[CreateAssetMenu(fileName = "New Flower Pattern", menuName = "Boss/Patterns/Flower")]
public class Flower : AttackPattern
{
    [Header("Configuración de la Flor")]
    public int numberOfPetals = 5;
    public float petalAmplitude = 20f; 
    public float curveSpeed = 2f;
    public float rotationSpeed = 10f;

    public override void PerformAttack(BossWeapon weapon)
    {
        float oscillation = Mathf.Sin(Time.time * curveSpeed) * petalAmplitude;
        float angleStep = 360f / numberOfPetals;
        for (int i = 0; i < numberOfPetals; i++)
        {
            float basePetalAngle = weapon.currentAngle + (angleStep * i);
            float finalAngle = basePetalAngle + oscillation;
            
            FireBullet(weapon, finalAngle);
        }
        weapon.currentAngle += rotationSpeed * Time.deltaTime * 10f; 
    }

    void FireBullet(BossWeapon weapon, float angleInDegrees)
    {
        Vector2 direction = Quaternion.Euler(0, 0, angleInDegrees) * Vector3.right;
        GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyProjectile>().Initialize(direction);
    }
}