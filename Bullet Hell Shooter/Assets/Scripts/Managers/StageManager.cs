using UnityEngine;
using System;

/// <summary>
/// Controla el estado global del nivel: límites, música, tiempo y contadores de balas.
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public int EnemyBulletCount { get; private set; } = 0;
    public int PlayerBulletCount { get; private set; } = 0;

    public event Action<int> OnEnemyBulletCountChanged;
    public event Action<int> OnPlayerBulletCountChanged;

    [Header("Música")]
    public AudioSource musicSource;
    public AudioClip gameplayMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    [Header("Límites del Escenario")]
    public float MinX, MaxX, MinY, MaxY;

    public static Action OnMinuteChanged;
    public static Action OnHourChanged;
    public int Minute { get; private set; }
    public int Hour { get; private set; }

    [Header("Configuración Tiempo")]
    private float minuteToRealTime = 0.5f;
    private float timer;
    private Camera mainCamera;
    private bool isGameOver = false;

    /// <summary>
    /// Configura el Singleton, la cámara y la música inicial.
    /// </summary>
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        mainCamera = Camera.main;

        if (musicSource == null) musicSource = GetComponent<AudioSource>();

        if (musicSource != null) musicSource.ignoreListenerPause = true;

        PlayGameplayMusic();
    }

    /// <summary>
    /// Actualiza el temporizador y recalcula límites si la cámara se mueve.
    /// </summary>
    void Update()
    {
        if (isGameOver) return;
        
        HandleTime();
        CalculateStageBounds(); 
    }

    /// <summary>
    /// Activa la secuencia de victoria (Música, UI, Pausa).
    /// </summary>
    public void PlayGameplayMusic()
    {
        if (musicSource != null && gameplayMusic != null)
        {
            if (musicSource.clip == gameplayMusic) return; // Ya está sonando
            musicSource.Stop();
            musicSource.clip = gameplayMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayBossMusic()
    {
        if (musicSource != null && bossMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = bossMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayVictoryMusic()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (musicSource != null && victoryMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = victoryMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameResult("¡VICTORIA!", Color.green);

        // 3. Detener el juego
        Time.timeScale = 0f; 
    }

    /// <summary>
    /// Activa la secuencia de derrota (Game Over).
    /// </summary>
    public void TriggerDefeat()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (musicSource != null && defeatMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = defeatMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
        else if (musicSource != null)
        {
            musicSource.Stop(); 
        }
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameResult("GAME OVER", Color.red);
        Time.timeScale = 0f; 
    }

    public void CalculateStageBounds()
    {
        if (mainCamera == null) return;

        float height = mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

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

    /// <summary>
    /// Registra una nueva bala enemiga y notifica a la UI.
    /// </summary>
    public void RegisterEnemyBullet()
    {
        EnemyBulletCount++;
        OnEnemyBulletCountChanged?.Invoke(EnemyBulletCount);
    }

    public void UnregisterEnemyBullet()
    {
        EnemyBulletCount--;
        if (EnemyBulletCount < 0) EnemyBulletCount = 0;
        OnEnemyBulletCountChanged?.Invoke(EnemyBulletCount);
    }

    public void RegisterPlayerBullet()
    {
        PlayerBulletCount++;
        OnPlayerBulletCountChanged?.Invoke(PlayerBulletCount);
    }

    public void UnregisterPlayerBullet()
    {
        PlayerBulletCount--;
        if (PlayerBulletCount < 0) PlayerBulletCount = 0;
        OnPlayerBulletCountChanged?.Invoke(PlayerBulletCount);
    }

    public bool IsPositionOnStage(Vector2 position, float buffer = 3f)
    {
        return position.x > MinX + buffer && 
               position.x < MaxX - buffer && 
               position.y > MinY + buffer && 
               position.y < MaxY - buffer;
    }


}