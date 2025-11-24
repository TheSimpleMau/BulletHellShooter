using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    [Header("Referencias UI")]
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI playerCounterText; // Nuevo texto para balas jugador
    public TextMeshProUGUI fpsText;           // Nuevo texto para FPS
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI bossHealthText;
    private float fpsPollingTime = 1f;
    private float fpsTime;
    private int frameCount;

    public static UIManager Instance
    {
        get
        {
            // Si la instancia no está asignada, la buscamos en la escena
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<UIManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        // Aquí "this" SÍ es válido porque Awake no es estático
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (StageManager.Instance != null)
        {
            // Suscribirse a ambos eventos
            StageManager.Instance.OnEnemyBulletCountChanged += UpdateEnemyText;
            StageManager.Instance.OnPlayerBulletCountChanged += UpdatePlayerText;
            
            UpdateEnemyText(0);
            UpdatePlayerText(0);
        }
    }

    void OnDestroy()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnEnemyBulletCountChanged -= UpdateEnemyText;
            StageManager.Instance.OnPlayerBulletCountChanged -= UpdatePlayerText;
        }
    }

    void Update()
    {
        // Lógica de FPS
        fpsTime += Time.deltaTime;
        frameCount++;

        if (fpsTime >= fpsPollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / fpsTime);
            fpsText.text = "FPS: " + frameRate.ToString();
            
            fpsTime -= fpsPollingTime;
            frameCount = 0;
        }
    }

    void UpdateEnemyText(int count)
    {
        if(enemyCounterText) enemyCounterText.text = "Balas de enemigos: " + count;
    }

    void UpdatePlayerText(int count)
    {
        if(playerCounterText) playerCounterText.text = "Balas del jugador: " + count;
    }

    public void UpdatePlayerHealth(int health)
    {
        if (playerHealthText != null && health > 0)
        {
            playerHealthText.text = "Vida jugador: " + health;
        }
        else
        {
            playerHealthText.text = "Te haz quedado sin vida\n¡Game over!";
        }
    }

    public void UpdateBossHealth(int health)
    {
        if (bossHealthText != null && health > 0)
        {
            bossHealthText.text = "Vida jefe: " + health;
        }
        else
        {
            
            bossHealthText.text = "¡Jefe derrotado!";
        }
    }
}