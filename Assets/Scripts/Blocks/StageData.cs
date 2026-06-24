using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game/Stage")]
public class StageData : ScriptableObject
{
    public WaveData[] waves;
}

[Serializable]
public class WaveData
{
    public enum WaveType 
    {
        Normal, 
        Boss, 
        Special 
    }

    public WaveType waveType = WaveType.Normal;

    public EnemyEntry[] enemies;
}

[Serializable]
public class EnemyEntry
{
    public GameObject enemyPrefab;
    public int count = 1;
    public float spawnDelay = 0.5f;
}
