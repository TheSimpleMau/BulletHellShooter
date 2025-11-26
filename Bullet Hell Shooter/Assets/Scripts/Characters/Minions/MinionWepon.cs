using UnityEngine;
using System.Collections;

/// <summary>
/// Arma de minions con tipos de disparo (Straight, Diagonal, Random) y corutina de disparo.
/// </summary>
[RequireComponent(typeof(Transform))]
public class MinionWeapon : MonoBehaviour
{
    public enum ShotType { Straight, Diagonal, Random }

    [Header("Bullet prefab")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 6f;
    public float fireRate = 1.0f;
    public Vector2 bulletSpawnOffset = Vector2.zero;

    [Header("Tipo de disparo")]
    public ShotType shotType = ShotType.Straight;
    public bool diagonalBothSides = true;
    public float diagonalAngle = 30f;
    public float randomConeAngle = 45f;

    Coroutine shootRoutine;

    void OnEnable()
    {
        if (bulletPrefab != null)
            shootRoutine = StartCoroutine(ShootRoutine());
    }

    void OnDisable()
    {
        if (shootRoutine != null)
            StopCoroutine(shootRoutine);
    }

    /// <summary>
    /// Rutina que hace FireOnce periódicamente con un pequeño delay inicial aleatorio.
    /// </summary>
    IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

        while (true)
        {
            FireOnce();
            yield return new WaitForSeconds(fireRate);
        }
    }

    void FireOnce()
    {
        if (bulletPrefab == null) return;
        if (StageManager.Instance != null && !StageManager.Instance.IsPositionOnStage(transform.position)) return; 
        Vector3 spawnPos = transform.position + (Vector3)bulletSpawnOffset;
        switch (shotType)
        {
            case ShotType.Straight:
                SpawnBullet(spawnPos, Vector2.down);
                break;

            case ShotType.Diagonal:
                Vector2 leftDir = (Quaternion.Euler(0, 0, diagonalAngle) * Vector2.down).normalized;
                SpawnBullet(spawnPos, leftDir);

                if (diagonalBothSides)
                {
                    Vector2 rightDir = (Quaternion.Euler(0, 0, -diagonalAngle) * Vector2.down).normalized;
                    SpawnBullet(spawnPos, rightDir);
                }
                break;

            case ShotType.Random:
                float half = randomConeAngle * 0.5f;
                float angle = Random.Range(-half, half);
                Vector2 randDir = (Quaternion.Euler(0, 0, angle) * Vector2.down).normalized;
                SpawnBullet(spawnPos, randDir);
                break;
        }
    }

    /// <summary>
    /// Genera un proyectil instanciado y lo inicializa con la velocidad configurada.
    /// </summary>
    void SpawnBullet(Vector3 pos, Vector2 direction)
    {
        GameObject b = Instantiate(bulletPrefab, pos, Quaternion.identity);
        var ep = b.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.Initialize(direction, bulletSpeed);
        }
        else
        {
            Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = direction.normalized * bulletSpeed;
        }
    }

}
