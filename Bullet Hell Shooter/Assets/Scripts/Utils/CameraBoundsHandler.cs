using UnityEngine;

/// <summary>
/// Mantiene la posición del jugador dentro de los límites de la cámara.
/// </summary>
public class CameraBoundsHandler : MonoBehaviour
{
    private Camera mainCamera;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        mainCamera = Camera.main;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            objectWidth = spriteRenderer.bounds.extents.x;
            objectHeight = spriteRenderer.bounds.extents.y;
        }
        else
        {
            objectWidth = 0.5f;
            objectHeight = 0.5f;
        }
    }

    /// <summary>
    /// Restringe la posición en LateUpdate para evitar salirse de la pantalla.
    /// </summary>
    void LateUpdate()
    {
        float distanceToZero = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToZero));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceToZero));
        Vector3 viewPos = transform.position;
        
        viewPos.x = Mathf.Clamp(viewPos.x, bottomLeft.x + objectWidth, topRight.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, bottomLeft.y + objectHeight, topRight.y - objectHeight);
        transform.position = viewPos;
    }
}