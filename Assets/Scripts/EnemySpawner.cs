using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Stage Data")]
    public StageData stage;

    [Header("UI")]
    public WaveManager waveManager;

    [Header("Pipes")]
    public Transform[] spawnPipes;
    public Transform[] exitPipes;

    private int currentWaveIndex = 0;
    private List<EnemyBaseV2> aliveEnemies = new List<EnemyBaseV2>();

    private int totalToSpawn;
    private int spawnedCount;

    private void Start()
    {
        StartWave(0);
    }

    private void StartWave(int index)
    {
        currentWaveIndex = index;

        WaveData wave = stage.waves[index];

        totalToSpawn = 0;
        spawnedCount = 0;

        foreach (var e in wave.enemies)
        {
            totalToSpawn += e.count;
        }

        waveManager.OnWaveStarted(index + 1);
        StartCoroutine(SpawnWave(wave));
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var e in wave.enemies)
        {
            for (int i = 0; i < e.count; i++)
            {
                SpawnEnemy(e.enemyPrefab, wave.waveType);
                spawnedCount++;
                yield return new WaitForSeconds(e.spawnDelay);
            }
        }
    }

    private void SpawnEnemy(GameObject prefab, WaveData.WaveType type)
    {
        Transform pipe = spawnPipes[Random.Range(0, spawnPipes.Length)];
        GameObject thingy = Instantiate(prefab, pipe.position, Quaternion.identity);

        EnemyBase enemy = thingy.GetComponent<EnemyBase>();
        enemy.spawner = this;

        //enemy.SetStartingDirection(pipe);

        if (type == WaveData.WaveType.Boss)
        {
            enemy.isBoss = true;
            enemy.bossHP = 5;
        }

        //aliveEnemies.Add(enemy);
    }

    public void EnemyDied(EnemyBase enemy)
    {
        //aliveEnemies.Remove(enemy);

        if (aliveEnemies.Count == 0 && spawnedCount >= totalToSpawn)
        {
            AdvanceWave();
        }
    }

    private void AdvanceWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= stage.waves.Length)
        {
            Debug.Log("Stage complete!");
            return;
        }

        waveManager.ShowNextWaveUI();
        StartWave(currentWaveIndex);
    }
}
