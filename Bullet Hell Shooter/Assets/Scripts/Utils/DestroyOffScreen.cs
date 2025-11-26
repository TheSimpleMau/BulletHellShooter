using UnityEngine;

/// <summary>
/// Destruye el objeto cuando sale de la pantalla (usa StageManager para los límites).
/// </summary>
public class DestroyOffScreen : MonoBehaviour
{
    public float buffer = 1f; 
    public string[] ignoreTags = new string[] { "Minion" };

    /// <summary>
    /// Verifica cada frame si el objeto está fuera y lo destruye.
    /// </summary>
    void Update()
    {
        if (ignoreTags != null && ignoreTags.Length > 0)
        {
            foreach (string t in ignoreTags)
            {
                if (!string.IsNullOrEmpty(t) && gameObject.CompareTag(t))
                    return;
            }
        }
        float topBound = StageManager.Instance.MaxY + buffer;
        float bottomBound = StageManager.Instance.MinY - buffer;
        float rightBound = StageManager.Instance.MaxX + buffer;
        float leftBound = StageManager.Instance.MinX - buffer;

        Vector3 pos = transform.position;

        if (pos.y > topBound || pos.y < bottomBound || 
            pos.x > rightBound || pos.x < leftBound)
        {
            Destroy(gameObject);
        }
    }
}