using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Administra la aparición de oleadas de enemigos y activa al jefe final.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public GameObject enemyPrefab;
        public int count = 5;
        public float timeBetweenSpawns = 1f;
    }

    public List<Wave> waves;
    public Transform[] spawnPoints;
    
    [Header("Boss control")]
    public GameObject bossToActivate;
    public bool disableBossAtStart = true;

    [Header("Inicio automático")]
    public bool startOnAwake = true;
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool wavesFinished = false;
    private bool wavesStarted = false;
    private List<Component> activeMinions = new List<Component>();
    private Dictionary<int, float> maxHealthByInstance = new Dictionary<int, float>();
    private Coroutine uiUpdateRoutine;


    public bool AreWavesFinished => wavesFinished;

    void Awake()
    {
        if (disableBossAtStart && bossToActivate != null)
        {
            bossToActivate.SetActive(false);
        }
    }

    void Start()
    {
        if (startOnAwake)
        {
            StartWaves();
        }
    }

    void OnEnable()
    {
        MinionHealth.OnMinionDeath += HandleEnemyDeath;
    }

    void OnDisable()
    {
        MinionHealth.OnMinionDeath -= HandleEnemyDeath;
    }

    /// <summary>
    /// Inicia la secuencia de oleadas si no ha comenzado.
    /// </summary>
    public void StartWaves()
    {
        if (wavesStarted)
        {
            return;
        }
        wavesStarted = true;
        StartCoroutine(SpawnWaveRoutine());
    }

    /// <summary>
    /// Lógica principal: spawnea oleadas, actualiza UI y espera a que mueran los enemigos.
    /// </summary>
    IEnumerator SpawnWaveRoutine()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];

            activeMinions.Clear();
            maxHealthByInstance.Clear();

            for (int i = 0; i < currentWave.count; i++)
            {
                SpawnEnemy(currentWave.enemyPrefab);
                yield return new WaitForSeconds(currentWave.timeBetweenSpawns);
            }
            if (uiUpdateRoutine != null) StopCoroutine(uiUpdateRoutine);
            uiUpdateRoutine = StartCoroutine(WaveHealthUpdater());

            yield return new WaitUntil(() => enemiesAlive == 0);

            if (uiUpdateRoutine != null) { StopCoroutine(uiUpdateRoutine); uiUpdateRoutine = null; }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateBossHealth(0, 1);
            }

            yield return new WaitForSeconds(2f);
            currentWaveIndex++;
        }

        wavesFinished = true;

        if (bossToActivate != null)
        {
            bossToActivate.SetActive(true);
            if (StageManager.Instance != null) 
            {
                StageManager.Instance.PlayBossMusic();
            }
        }
    }

    /// <summary>
    /// Instancia un enemigo en una posición aleatoria y registra su salud.
    /// </summary>
    void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            return;
        }

        Vector3 spawnPos = Vector3.zero;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            spawnPos = spawnPoints[randomIndex].position;
        }
        else
        {
            spawnPos = new Vector3(Random.Range(-8f, 8f), 11f, 0f);
        }

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
        enemiesAlive++;

        MinionMovement movement = go.GetComponent<MinionMovement>();
        if (movement != null)
        {
            movement.InitializeAtSpawn(spawnPos);
        }

        Component healthComp = go.GetComponent(typeof(MinionHealth)) as Component;
        if (healthComp == null)
        {
            healthComp = go.GetComponent("MinionHealth") as Component;
        }

        if (healthComp != null)
        {
            activeMinions.Add(healthComp);

            if (uiUpdateRoutine == null)
            {
                uiUpdateRoutine = StartCoroutine(WaveHealthUpdater());
            }

            int id = go.GetInstanceID();
            if (TryGetHealth(healthComp, out float curH, out float maxH))
            {
                maxHealthByInstance[id] = maxH;
            }
            else
            {
                maxHealthByInstance[id] =  Mathf.Max(1f, (float) (healthComp.GetType().GetField("health") != null ? (float)ConvertToFloat(healthComp.GetType().GetField("health").GetValue(healthComp)) : 1f));
            }
        }

    }

    /// <summary>
    /// Actualiza la barra de vida del jefe sumando la vida de todos los minions activos.
    /// </summary>
    IEnumerator WaveHealthUpdater()
    {
        while (true)
        {
            activeMinions.RemoveAll(m => m == null);

            float sumCurrent = 0f;
            float sumMax = 0f;

            foreach (var comp in activeMinions)
            {
                if (comp == null) continue;

                if (TryGetHealth(comp, out float cur, out float max))
                {
                    sumCurrent += cur;
                    sumMax += max;
                }
                else
                {
                    sumCurrent += 0f;
                    sumMax += 1f;
                }
            }

            int displayCurrent = Mathf.Max(0, Mathf.RoundToInt(sumCurrent));
            int displayMax = Mathf.Max(1, Mathf.RoundToInt(sumMax));

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateBossHealth(displayCurrent, displayMax);
            }
            if (activeMinions.Count == 0)
            {
                if (UIManager.Instance != null)
                    UIManager.Instance.UpdateBossHealth(0, 1);
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void HandleEnemyDeath() { if (enemiesAlive > 0) enemiesAlive--; }
    private static float ConvertToFloat(object o) { if (o == null) return 0f; if (float.TryParse(o.ToString(), out float res)) return res; return 0f; }
    private bool TryGetHealth(Component comp, out float current, out float max)
    {
        current = 0f; max = 0f;
        if (comp == null) return false;
        Type t = comp.GetType();
        
        // Esta sección me ayudó a refactorizarla Gemini puesto que era una lógica sencilla.
        FieldInfo fCur = t.GetField("health") ?? t.GetField("currentHealth") ?? t.GetField("hp");
        PropertyInfo pCur = t.GetProperty("health") ?? t.GetProperty("CurrentHealth") ?? t.GetProperty("HP");
        if (fCur != null) current = ConvertToFloat(fCur.GetValue(comp));
        else if (pCur != null) current = ConvertToFloat(pCur.GetValue(comp));

        FieldInfo fMax = t.GetField("maxHealth") ?? t.GetField("maxHP") ?? t.GetField("max");
        PropertyInfo pMax = t.GetProperty("maxHealth") ?? t.GetProperty("MaxHP");
        if (fMax != null) max = ConvertToFloat(fMax.GetValue(comp));
        else if (pMax != null) max = ConvertToFloat(pMax.GetValue(comp));

        if (max <= 0f && current > 0f) max = current;
        return max > 0f;
    }

}