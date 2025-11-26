using UnityEngine;

/// <summary>
/// Scroller que mueve el sprite hacia abajo y lo reposiciona para bucle infinito.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    private BackgroundController masterController;
    private SpriteRenderer spriteRenderer;
    private float spriteHeight;

    /// <summary>
    /// Mueve el fondo y recoloca cuando sale por abajo.
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteHeight = spriteRenderer.bounds.size.y;
        masterController = GetComponentInParent<BackgroundController>();
    }

    /// <summary>
    /// Mueve el fondo hacia abajo basado en la velocidad del padre 
    /// y verifica constantemente si el sprite ha salido del campo visual de la cámara.
    /// </summary>
    void Update()
    {
        float currentSpeed = masterController.scrollSpeed;
        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);
        float cameraBottomEdge = StageManager.Instance.MinY;
        float myTopEdge = transform.position.y + (spriteHeight / 2);
        if (myTopEdge < cameraBottomEdge)
        {
            RepositionBackground();
        }
    }
    
    /// <summary>
    /// Teletransporta el fondo hacia la parte superior sumando dos veces
    /// su altura para mantener el bucle infinito sin cortes.
    /// </summary>
    void RepositionBackground()
    {
        Vector3 positionChange = Vector3.up * spriteHeight * 2f;
        transform.position += positionChange;
    }
}