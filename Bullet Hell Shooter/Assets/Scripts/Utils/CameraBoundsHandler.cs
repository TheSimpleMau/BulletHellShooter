using UnityEngine;

public class CameraBoundsHandler : MonoBehaviour
{
    private Camera mainCamera;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        mainCamera = Camera.main;

        // Calculamos el tamaño del sprite del jugador
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

    void LateUpdate()
    {
        // 1. Calculamos la distancia de la cámara al plano Z=0 (donde juega la nave)
        float distanceToZero = Mathf.Abs(mainCamera.transform.position.z);

        // 2. Obtenemos las esquinas de la pantalla proyectadas exactamente en Z=0
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToZero));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceToZero));

        // 3. Obtenemos la posición actual
        Vector3 viewPos = transform.position;

        // 4. Restringimos (Clamp) la posición
        // Matamáticamente: El borde de la pantalla MENOS el ancho de la nave
        viewPos.x = Mathf.Clamp(viewPos.x, bottomLeft.x + objectWidth, topRight.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, bottomLeft.y + objectHeight, topRight.y - objectHeight);

        // 5. Aplicamos la posición corregida
        transform.position = viewPos;
    }

    // ESTO ES NUEVO: Dibuja una caja verde en el editor para que veas el límite real
    void OnDrawGizmos()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        float distance = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 bl = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 tr = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distance));
        
        // Dibujamos el cuadro donde la nave puede moverse (Límite - TamañoNave)
        // Nota: Solo es preciso si el juego está corriendo y calculó el objectWidth
        Gizmos.color = Color.green;
        Vector3 center = (bl + tr) / 2;
        Vector3 size = tr - bl;
        // Restamos visualmente un poco para simular el tamaño de la nave
        size.x -= (objectWidth * 2);
        size.y -= (objectHeight * 2);
        Gizmos.DrawWireCube(center, size);
    }
}