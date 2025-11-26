using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controla toda la interfaz gráfica: barras de vida, contadores y textos.
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    [Header("Referencias UI")]
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI playerCounterText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI bossHealthText;
    public TextMeshProUGUI gameResultText;

    [Header("Barras de Vida")]
    public Slider playerHealthSlider;
    public Slider bossHealthSlider;
    private float fpsPollingTime = 1f;
    private float fpsTime;
    private int frameCount;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<UIManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
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

    /// <summary>
    /// Actualiza el texto de balas de enemigos.
    /// </summary>
    void UpdateEnemyText(int count)
    {
        if(enemyCounterText) enemyCounterText.text = "Balas de enemigos: " + count;
    }

    /// <summary>
    /// Actualiza el texto de balas del jugador.
    /// </summary>
    void UpdatePlayerText(int count)
    {
        if(playerCounterText) playerCounterText.text = "Balas del jugador: " + count;
    }

    /// <summary>
    /// Actualiza la barra de vida del jugador.
    /// </summary>
    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Actualiza la barra de vida del jefe o de la oleada.
    /// </summary>
    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        if (bossHealthSlider != null)
        {
            int safeMax = Mathf.Max(1, maxHealth);
            int safeCurrent = Mathf.Clamp(currentHealth, 0, safeMax);
            bossHealthSlider.maxValue = safeMax;
            bossHealthSlider.value = safeCurrent;
        }
        if (bossHealthText != null)
        {
            bossHealthText.text = $"Enemigos: {currentHealth} / {maxHealth}";
        }
    }

    /// <summary>
    /// Muestra el mensaje final de partida (Victoria/Derrota).
    /// </summary>
    public void ShowGameResult(string message, Color color)
    {
        if (gameResultText != null)
        {
            gameResultText.text = message;
            gameResultText.color = color;
            gameResultText.gameObject.SetActive(true);
        }
    }

}