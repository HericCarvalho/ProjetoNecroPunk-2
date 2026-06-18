using UnityEngine;
using System.Collections;


public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public EnemyWave[] waves;

    int currentWave = 0;

    bool spawning = false;

    bool waveActive = false;

    int enemiesToSpawn;
    int enemiesKilled;


    void Awake()
    {
        instance = this;
    }
    public void StartWave()
    {
        if (spawning || waveActive)
            return;

        if (currentWave >= waves.Length)
        {
            Debug.Log("Todas as waves já foram concluídas!");
            return;
        }

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        if (currentWave >= waves.Length)
            yield break;

        spawning = true;
        waveActive = true;

        EnemyWave wave = waves[currentWave];

        enemiesKilled = 0;
        enemiesToSpawn = 0;

        foreach (EnemyGroup group in wave.groups)
        {
            enemiesToSpawn += group.count;
        }

        foreach (EnemyGroup group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnDelay);
            }
        }

        spawning = false;

        yield return new WaitUntil(() => enemiesKilled >= enemiesToSpawn);

        waveActive = false;
        spawning = false;
        currentWave++;

        if (currentWave >= waves.Length)
        {
            StartCoroutine(HandleVictory());
        }
        Debug.Log(
    $"Wave {currentWave + 1} iniciou com {enemiesToSpawn} inimigos"
);
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPos = EnemyPath.instance.GetWaypoint(0).position;

        GameObject enemy = ObjectPool.instance.GetObject(enemyPrefab, spawnPos);

        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();

        if (eh != null)
        {
            eh.SetPrefabReference(enemyPrefab);
        }
    }
    public bool IsWaveActive()
    {
        return waveActive || spawning;
    }
    public int GetCurrentWaveIndex()
    {
        return currentWave;
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }
    public void RegisterEnemyDeath()
    {
        enemiesKilled++;

        Debug.Log(
            $"Mortos: {enemiesKilled}/{enemiesToSpawn}"
        );

        LevelStatsManager.instance.RegisterKill();
    }
    IEnumerator HandleVictory()
    {
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(1f);

        Time.timeScale = 1f;

        yield return new WaitForSeconds(1f);

        float performance = (float)BaseHealth.instance.currentHealth / BaseHealth.instance.maxHealth * 100f;

        LevelManager.instance.WinGame();
    }
    public int GetRemainingEnemies()
    {
        return enemiesToSpawn - enemiesKilled;
    }
    public int GetTotalEnemiesInWave()
    {
        return enemiesToSpawn;
    }
    public bool IsWaveRunning()
    {
        return waveActive || spawning;
    }

    public EnemyWave GetCurrentWave()
    {
        if (currentWave >= waves.Length)
            return null;

        return waves[currentWave];
    }
}