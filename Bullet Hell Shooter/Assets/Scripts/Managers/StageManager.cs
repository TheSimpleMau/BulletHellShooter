using UnityEngine;
using System;

/// <summary>
/// Gestor centralizado para el Tiempo y los Límites del escenario.
/// Evita que las balas tengan que calcular el tamaño de la cámara individualmente.
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Límites del Escenario (Calculados Automáticamente)")]
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    // --- Sistema de Tiempo ---
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;
    public int Minute { get; private set; }
    public int Hour { get; private set; }

    [Header("Configuración Tiempo")]
    private float minuteToRealTime = 0.5f;
    private float timer;
    private Camera mainCamera;

    void Awake()
    {
        // Singleton: Asegura que solo haya un StageManager
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
        CalculateStageBounds();
    }

    void Update()
    {
        HandleTime();
    }

    /// <summary>
    /// Calcula los bordes de la pantalla en coordenadas del mundo.
    /// Se debe llamar si la cámara se mueve o cambia de tamaño.
    /// </summary>
    public void CalculateStageBounds()
    {
        float height = mainCamera.orthographicSize; // Altura del centro al borde
        float width = height * mainCamera.aspect;   // Ancho calculado por el aspecto

        MinY = mainCamera.transform.position.y - height;
        MaxY = mainCamera.transform.position.y + height;
        MinX = mainCamera.transform.position.x - width;
        MaxX = mainCamera.transform.position.x + width;
    }

    private void HandleTime()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Minute++;
            OnMinuteChanged?.Invoke();
            
            if (Minute >= 60)
            {
                Hour++;
                OnHourChanged?.Invoke();
                Minute = 0;
            }
            timer = minuteToRealTime;
        }
    }
}