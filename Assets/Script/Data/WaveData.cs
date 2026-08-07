using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWaveData",
    menuName = "Pokemon Gacha Defense/Wave Data"
)]
public class WaveData : ScriptableObject
{
    [Header("웨이브 정보")]
    public int waveNumber = 1;

    [Header("등장 몬스터")]
    public EnemyData enemyData;

    [Header("스폰 설정")]
    public int spawnCount = 20;

    public float spawnInterval = 1.5f;

    [Header("웨이브 진행 시간")]
    public float waveDuration = 30f;
}