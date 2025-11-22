using UnityEngine;

[CreateAssetMenu(fileName = "New Flower Pattern", menuName = "Boss/Patterns/Flower")]
public class Flower : AttackPattern
{
    [Header("Configuración de la Flor")]
    [Tooltip("Número de pétalos (Cuántos chorros de balas salen a la vez).")]
    public int numberOfPetals = 5; // En tu imagen de referencia son 5 o 6.

    [Tooltip("Qué tan curvados son los pétalos (Apertura de la flor).")]
    public float petalAmplitude = 20f; 

    [Tooltip("Velocidad a la que oscilan los pétalos (Frecuencia de la curva).")]
    public float curveSpeed = 2f;

    [Tooltip("Velocidad de rotación de toda la flor.")]
    public float rotationSpeed = 10f;

    public override void PerformAttack(BossWeapon weapon)
    {
        // 1. Calcular la oscilación común para todos los pétalos en este momento
        // Usamos Time.time para que la curva se mueva suavemente
        float oscillation = Mathf.Sin(Time.time * curveSpeed) * petalAmplitude;

        // 2. Calcular el paso angular entre pétalos (ej: 360 / 5 = 72 grados)
        float angleStep = 360f / numberOfPetals;

        // 3. BUCLE: Disparar por TODOS los pétalos a la vez
        for (int i = 0; i < numberOfPetals; i++)
        {
            // A. Calcular el ángulo base de este pétalo específico
            // (Angulo del Arma) + (Posición del pétalo i)
            float basePetalAngle = weapon.currentAngle + (angleStep * i);

            // B. Sumarle la oscilación (La curva S)
            float finalAngle = basePetalAngle + oscillation;

            // C. Disparar
            FireBullet(weapon, finalAngle);
        }

        // 4. Rotar la flor completa para el siguiente frame
        weapon.currentAngle += rotationSpeed * Time.deltaTime * 10f; // Multiplicador para ajustar sensibilidad
        
        // if (weapon.currentAngle >= 360f) weapon.currentAngle -= 360f;
    }

    void FireBullet(BossWeapon weapon, float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        GameObject bullet = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyProjectile>().Initialize(direction);
    }
}