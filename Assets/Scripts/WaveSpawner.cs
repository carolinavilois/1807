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
        public int pathACount = 3;            // Cu�ntos enemigos por Path A
        public int pathBCount = 0;            // Cu�ntos enemigos por Path B
        public int pathCCount = 0;            // Cu�ntos enemigos por Path C
        public float spawnDelay = 1f;         // Segundos entre cada enemigo
        [Range(0, 100)] public int enemy1Weight = 100;
        [Range(0, 100)] public int enemy2Weight = 0;
        [Range(0, 100)] public int enemy3Weight = 0;
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
    public ProcerDialog procerDialog;
    public UnityEngine.UI.Text waveHUDText;
    public SupplyRaidUI supplyRaidUI;
    public GameObject supplyCharacterPrefab;
    public WaypointPath[] allPaths;
    string[] procerNames = { "Nombre 1", "Nombre 2", "Nombre 3", "Nombre 4" };

    int currentWave = 0;         // �ndice de la oleada actual
    int enemiesAlive = 0;        // Enemigos que siguen vivos en esta oleada
    int[] enemiesByType = new int[3];  // 0=E1, 1=E2, 2=E3
    int currentLives;            // Vidas restantes del jugador

    // Bonos de Cabalgatas de Abastecimiento (duran una oleada)
    public int bonusExtraPatriotism = 0;
    public float fireRateMultiplier = 1f;
    int tempBonusLives = 0;
    bool supplyRaidActive = false;

    // Control de dificultad progresiva: si un camino se usa por primera vez, solo enemigos b�sicos
    bool pathAWasUsed = false;
    bool pathBWasUsed = false;
    bool pathCWasUsed = false;

    void Start()
    {
        waveText.text = "Oleada 1/10";
        currentLives = maxLives;
        livesText.text = "Vidas: " + currentLives;

        if (procerDialog != null)
        {
            WaveConfig first = waves[0];
            procerDialog.Show(BuildProcerMessage(first), GetProcerName(0));
        }
    }

    void Update()
    {
        // Espacio inicia la oleada solo si no hay di�logo ni cabalgata abiertos
        if (Input.GetKeyDown(KeyCode.Space) && currentWave < waves.Length && enemiesAlive == 0
            && (procerDialog == null || !procerDialog.IsOpen())
            && !supplyRaidActive
            && (supplyRaidUI == null || !supplyRaidUI.IsOpen()))
        {
            StartCoroutine(SpawnWave(currentWave));
            currentWave++;
        }
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        WaveConfig config = waves[waveIndex];
        waveText.text = "Oleada " + (waveIndex + 1) + "/10";
        System.Array.Clear(enemiesByType, 0, enemiesByType.Length);

        // Spawnea enemigos por Path A
        for (int i = 0; i < config.pathACount; i++)
        {
            SpawnEnemy(pathA, config);
            yield return new WaitForSeconds(config.spawnDelay);
        }

        // Spawnea enemigos por Path B
        for (int i = 0; i < config.pathBCount; i++)
        {
            SpawnEnemy(pathB, config);
            yield return new WaitForSeconds(config.spawnDelay);
        }

        // Spawnea enemigos por Path C
        for (int i = 0; i < config.pathCCount; i++)
        {
            SpawnEnemy(pathC, config);
            yield return new WaitForSeconds(config.spawnDelay);
        }

        // Marca los caminos que ya aparecieron (para dificultad progresiva)
        if (config.pathACount > 0) pathAWasUsed = true;
        if (config.pathBCount > 0) pathBWasUsed = true;
        if (config.pathCCount > 0) pathCWasUsed = true;

        // Espera a que todos los enemigos mueran o lleguen a la base
        yield return new WaitUntil(() => enemiesAlive == 0);

        // Resetear bonos de cabalgata
        bonusExtraPatriotism = 0;
        fireRateMultiplier = 1f;
        if (tempBonusLives > 0)
        {
            maxLives -= 2;
            currentLives = Mathf.Min(currentLives, maxLives);
            livesText.text = "Vidas: " + currentLives;
            tempBonusLives = 0;
        }

        if (currentWave >= waves.Length)
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("VictoryScene");
        }
        else
        {
            // Muestra el di�logo del pr�cer con info de la pr�xima oleada
            if (procerDialog != null)
            {
                WaveConfig next = waves[currentWave];
                string procerName = GetProcerName(currentWave);
                procerDialog.Show(BuildProcerMessage(next), procerName);
                yield return new WaitUntil(() => !procerDialog.IsOpen());
            }
            // Cabalgata de Abastecimiento en oleadas 3, 6, 9 (�ndices 2, 5, 8)
            if (currentWave == 2 || currentWave == 5 || currentWave == 8)
            {
                yield return StartCoroutine(RunSupplyRaid());
            }

            waveText.text = "Oleada " + (currentWave + 1) + "/10";
        }
    }

    IEnumerator RunSupplyRaid()
    {
        supplyRaidActive = true;

        // Elegir path aleatorio
        WaypointPath randomPath = allPaths[Random.Range(0, allPaths.Length)];

        // Instanciar personaje en waypoint 0
        GameObject character = Instantiate(supplyCharacterPrefab, randomPath.GetWaypoint(0).position, Quaternion.identity);

        // Esperar a que llegue al final
        bool arrived = false;
        character.GetComponent<SupplyCharacter>().SetPathAndGo(randomPath, () => arrived = true);
        yield return new WaitUntil(() => arrived);

        // Mostrar panel de bonos
        int chosen = -1;
        supplyRaidUI.Show((index) => chosen = index);
        yield return new WaitUntil(() => chosen >= 0);

        // Aplicar bono
        switch (chosen)
        {
            case 0: // Refuerzos
                bonusExtraPatriotism = 5;
                break;
            case 1: // Reparar Murallas
                tempBonusLives = 2;
                maxLives += 2;
                currentLives += 2;
                livesText.text = "Vidas: " + currentLives;
                break;
            case 2: // Avituallamiento
                fireRateMultiplier = 1.5f;
                break;
        }

        supplyRaidActive = false;
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

    // Genera el mensaje del pr�cer seg�n los caminos activos en la pr�xima oleada
    string BuildProcerMessage(WaveConfig config)
    {
        int total = config.pathACount + config.pathBCount + config.pathCCount;
        List<string> pathNames = new List<string>();
        if (config.pathACount > 0) pathNames.Add("el Camino C�ntrico");
        if (config.pathBCount > 0) pathNames.Add("el Camino Inferior");
        if (config.pathCCount > 0) pathNames.Add("el Camino Superior");

        string pathsStr;
        if (pathNames.Count == 1) pathsStr = pathNames[0];
        else if (pathNames.Count == 2) pathsStr = pathNames[0] + " y " + pathNames[1];
        else pathsStr = pathNames[0] + ", " + pathNames[1] + " y " + pathNames[2];

        string enemies = total == 1 ? "1 ingl�s" : total + " ingleses";
        string verbo = total == 1 ? "avanza" : "avanzan";

        return "�Alerta, Liniers! " + enemies + " " + verbo + " por " + pathsStr + ". �Preparad vuestras defensas!";
    }

    // Crea un enemigo en el camino indicado con el tipo de enemigo correspondiente
    void SpawnEnemy(WaypointPath path, WaveConfig config)
    {
        if (path == null) return;
        GameObject prefab = ChooseEnemyType(config, path);
        if (prefab == null) return;
        GameObject enemy = Instantiate(prefab, path.GetWaypoint(0).position, Quaternion.identity);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.enemyTypeIndex = prefab == enemy2Prefab ? 1 : prefab == enemy3Prefab ? 2 : 0;
        enemiesByType[enemyScript.enemyTypeIndex]++;
        enemy.GetComponent<EnemyMovement>().SetPath(path);
        enemiesAlive++;
        UpdateWaveHUD();
    }

    // Cuando un enemigo llega a la base: resta vida y verifica Game Over
    public void EnemyReachedBase(int typeIndex)
    {
        enemiesAlive--;
        enemiesByType[typeIndex]--;
        currentLives--;
        livesText.text = "Vidas: " + currentLives;
        UpdateWaveHUD();
        if (currentLives <= 0)
            GameOver();
    }

    void GameOver()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    public void EnemyDied(int typeIndex)
    {
        enemiesAlive--;
        enemiesByType[typeIndex]--;
        if (bonusExtraPatriotism > 0)
            PatriotismManager.Instance.AddKillReward(bonusExtraPatriotism);
        UpdateWaveHUD();
    }

    public bool HasActiveEnemies() => enemiesAlive > 0;  // �Hay enemigos vivos en este momento?

    void UpdateWaveHUD()
    {
        if (waveHUDText == null) return;
        string[] labels = { "E1", "E2", "E3" };
        List<string> parts = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            if (enemiesByType[i] > 0)
                parts.Add(labels[i] + ": " + enemiesByType[i]);
        }
        waveHUDText.text = string.Join("  ", parts);
    }

    // Devuelve el nombre seg�n la oleada (pr�xima a mostrar)
    string GetProcerName(int nextWaveIndex)
    {
        if (nextWaveIndex < 2) return procerNames[0];
        if (nextWaveIndex < 5) return procerNames[1];
        if (nextWaveIndex < 7) return procerNames[2];
        return procerNames[3];
    }
}
