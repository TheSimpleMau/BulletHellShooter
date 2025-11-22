using UnityEngine;

/// <summary>
/// Destruye automáticamente el objeto si sale de los límites definidos por el StageManager.
/// </summary>
public class DestroyOffScreen : MonoBehaviour
{
    [Tooltip("Margen extra para asegurar que el sprite salga completamente antes de borrarlo.")]
    public float buffer = 1f; 

    void Update()
    {
        // Obtenemos los límites cacheados del Manager (¡Súper rápido!)
        float topBound = StageManager.Instance.MaxY + buffer;
        float bottomBound = StageManager.Instance.MinY - buffer;
        float rightBound = StageManager.Instance.MaxX + buffer;
        float leftBound = StageManager.Instance.MinX - buffer;

        Vector3 pos = transform.position;

        // Verificamos si salió por cualquiera de los 4 lados
        if (pos.y > topBound || pos.y < bottomBound || 
            pos.x > rightBound || pos.x < leftBound)
        {
            Destroy(gameObject);
        }
    }
}