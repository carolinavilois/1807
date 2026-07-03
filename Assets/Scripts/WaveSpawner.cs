using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    // Configuraci�n de cada oleada (se edita desde el Inspector)
    [System.Serializable]
    public class WaveConfig
    {
        public int enemyCount = 5;            // Cu�ntos enemigos spawnear
        public float spawnDelay = 1f;         // Segundos entre cada enemigo
        [Range(0, 100)] public int enemy1Weight = 100;   // % de probabilidad de Enemy1
        [Range(0, 100)] public int enemy2Weight = 0;
        [Range(0, 100)] public int enemy3Weight = 0;
        public bool usePathA = true;          // Caminos disponibles en esta oleada
        public bool usePathB = false;
        public bool usePathC = false;
    }

    // Referencias desde el Inspector
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;
    public WaypointPath pathA;
    public WaypointPath pathB;
    public WaypointPath pathC;
    public WaveConfig[] waves = new WaveConfig[10];
    public int maxLives = 10;
    public UnityEngine.UI.Text livesText;
    public UnityEngine.UI.Text waveText;

    int currentWave = 0;         // �ndice de la oleada actual
    int enemiesAlive = 0;        // Enemigos que siguen vivos en esta oleada
    int currentLives;            // Vidas restantes del jugador

    // Control de dificultad progresiva: si un camino se usa por primera vez, solo enemigos b�sicos
    bool pathAWasUsed = false;
    bool pathBWasUsed = false;
    bool pathCWasUsed = false;

    void Start()
    {
        waveText.text = "Oleada 1/10";
        currentLives = maxLives;
        livesText.text = "Vidas: " + currentLives;
    }

    void Update()
    {
        // Espacio inicia la oleada si no hay enemigos vivos y todav�a quedan oleadas
        if (Input.GetKeyDown(KeyCode.Space) && currentWave < waves.Length && enemiesAlive == 0)
        {
            StartCoroutine(SpawnWave(currentWave));
            currentWave++;
        }
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        WaveConfig config = waves[waveIndex];
        waveText.text = "Oleada " + (waveIndex + 1) + "/10";

        // Spawnea enemigos uno por uno con una pausa entre cada uno
        for (int i = 0; i < config.enemyCount; i++)
        {
            WaypointPath path = ChoosePath(config);                   // Elige el camino
            GameObject prefab = ChooseEnemyType(config, path);        // Elige el tipo de enemigo
            if (prefab != null && path != null)
            {
                GameObject enemy = Instantiate(prefab, path.GetWaypoint(0).position, Quaternion.identity);
                enemy.GetComponent<EnemyMovement>().SetPath(path);
                enemiesAlive++;
            }
            yield return new WaitForSeconds(config.spawnDelay);
        }

        // Marca los caminos que ya aparecieron (para dificultad progresiva)
        if (config.usePathA) pathAWasUsed = true;
        if (config.usePathB) pathBWasUsed = true;
        if (config.usePathC) pathCWasUsed = true;

        // Espera a que todos los enemigos mueran o lleguen a la base
        yield return new WaitUntil(() => enemiesAlive == 0);
        waveText.text = "Oleada " + (waveIndex + 1) + " completada";
        if (currentWave >= waves.Length)
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("VictoryScene");
        }
        else
        {
            waveText.text = "Oleada " + (currentWave + 1) + "/10";
        }
    }

    // Elige el tipo de enemigo seg�n los pesos, con dificultad progresiva por camino nuevo
    GameObject ChooseEnemyType(WaveConfig config, WaypointPath path)
    {
        // Si este camino se abre por primera vez, solo enemigos b�sicos (Enemy1)
        if ((path == pathA && !pathAWasUsed) ||
            (path == pathB && !pathBWasUsed) ||
            (path == pathC && !pathCWasUsed))
        {
            return enemy1Prefab;
        }

        // Selecci�n ponderada seg�n los pesos de la oleada
        int total = config.enemy1Weight + config.enemy2Weight + config.enemy3Weight;
        int roll = Random.Range(0, total);
        if (roll < config.enemy1Weight) return enemy1Prefab;
        if (roll < config.enemy1Weight + config.enemy2Weight) return enemy2Prefab;
        return enemy3Prefab;
    }

    // Elige un camino al azar entre los activos en esta oleada
    WaypointPath ChoosePath(WaveConfig config)
    {
        List<WaypointPath> activePaths = new List<WaypointPath>();
        if (config.usePathA) activePaths.Add(pathA);
        if (config.usePathB) activePaths.Add(pathB);
        if (config.usePathC) activePaths.Add(pathC);

        if (activePaths.Count == 0) return null;

        return activePaths[Random.Range(0, activePaths.Count)];
    }

    // Cuando un enemigo llega a la base: resta vida y verifica Game Over
    public void EnemyReachedBase()
    {
        enemiesAlive--;
        currentLives--;
        livesText.text = "Vidas: " + currentLives;
        if (currentLives <= 0)
            GameOver();
    }

    void GameOver()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    public void EnemyDied() => enemiesAlive--;   // Un enemigo muri� antes de llegar a la base

    public bool HasActiveEnemies() => enemiesAlive > 0;  // �Hay enemigos vivos en este momento?
}
