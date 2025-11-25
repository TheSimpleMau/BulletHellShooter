using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    // --- CONTADORES ---
    public int EnemyBulletCount { get; private set; } = 0;
    public int PlayerBulletCount { get; private set; } = 0;

    public event Action<int> OnEnemyBulletCountChanged;
    public event Action<int> OnPlayerBulletCountChanged;

    [Header("Música")]
    public AudioSource musicSource;
    public AudioClip gameplayMusic;
    public AudioClip victoryMusic;

    [Header("Límites del Escenario")]
    public float MinX, MaxX, MinY, MaxY;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        mainCamera = Camera.main;

        if (musicSource != null && gameplayMusic != null)
        {
            musicSource.clip = gameplayMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
        if (victoryMusic == null) {
            victoryMusic = Resources.Load<AudioClip>("Audio/victoryMusicName");
        }
    }

    void Update()
    {
        HandleTime();
        // AGREGA ESTO AQUÍ: Calculamos los límites en cada frame
        CalculateStageBounds(); 
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Dibuja un cuadrado representando los límites
        Vector3 center = new Vector3((MinX + MaxX) / 2, (MinY + MaxY) / 2, 0);
        Vector3 size = new Vector3(MaxX - MinX, MaxY - MinY, 1);
        Gizmos.DrawWireCube(center, size);
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

    // --- MÉTODOS DE REGISTRO ---
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

    public void PlayVictoryMusic()
{
    if (musicSource == null)
    {
        musicSource = GetComponent<AudioSource>();
    }

    if (musicSource != null && victoryMusic != null)
    {
        musicSource.Stop(); // opcional: detiene la música actual
        musicSource.PlayOneShot(victoryMusic); // reproducir jingle de victoria
    }
}


}