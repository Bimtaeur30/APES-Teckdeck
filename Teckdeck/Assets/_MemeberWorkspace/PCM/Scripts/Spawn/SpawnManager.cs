using _Shared.Systems.ObjectPool.Runtime;
using CoreSystem;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnCell
{
    public Vector3 Min;
    public Vector3 Max;

    public Vector3 RandomPoint()
    {
        return new Vector3(
            Random.Range(Min.x, Max.x),
            Min.y,
            Random.Range(Min.z, Max.z));
    }
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform ground;
    [SerializeField] private int cellCount = 6;
    [SerializeField] private float notSpawnedEnemyRadius;
    [SerializeField] private float maxEnemyRadius;
    [SerializeField] private PoolManagerSO managerSO;
    [SerializeField] private EventChannelSO spawnChannel;

    private readonly List<SpawnCell> spawnZone = new();
    private readonly List<int> _validCells = new();

    private void Awake()
    {
        managerSO?.InitializePool(transform);
        CellCalculate();
    }

    private void OnEnable()
    {
        spawnChannel?.AddListener<SpawnEnemyEvent>(HandleSpawnEnemy);
    }

    private void OnDisable()
    {
        spawnChannel?.RemoveListener<SpawnEnemyEvent>(HandleSpawnEnemy);
    }

    private void HandleSpawnEnemy(SpawnEnemyEvent evt)
    {
        SelectSpawnPoint(evt.EnemyItem, evt.PlayerPos, evt.Count);
    }

    public void CellCalculate()
    {
        spawnZone.Clear();

        Vector3 center = ground.position;
        Vector3 size = ground.lossyScale;
        float minX = center.x - size.x * 0.5f;
        float minZ = center.z - size.z * 0.5f;
        float cellX = size.x / cellCount;
        float cellZ = size.z / cellCount;

        for (int x = 0; x < cellCount; x++)
        {
            for (int z = 0; z < cellCount; z++)
            {
                spawnZone.Add(new SpawnCell
                {
                    Min = new Vector3(minX + cellX * x, center.y, minZ + cellZ * z),
                    Max = new Vector3(minX + cellX * (x + 1), center.y, minZ + cellZ * (z + 1))
                });
            }
        }
    }

    public bool OverlapsPlayer(Vector3 playerPos, SpawnCell spawnPos)
    {
        float closestX = Mathf.Clamp(playerPos.x, spawnPos.Min.x, spawnPos.Max.x);
        float closestZ = Mathf.Clamp(playerPos.z, spawnPos.Min.z, spawnPos.Max.z);
        float dx = playerPos.x - closestX;
        float dz = playerPos.z - closestZ;
        float targetDistance = dx * dx + dz * dz;
        return targetDistance > notSpawnedEnemyRadius * notSpawnedEnemyRadius
            && targetDistance < maxEnemyRadius * maxEnemyRadius;
    }

    public void SelectSpawnPoint(PoolItemSO enemyItem, Vector3 playerPos, int count)
    {
        if (managerSO == null || enemyItem == null)
            return;

        _validCells.Clear();
        for (int i = 0; i < spawnZone.Count; i++)
        {
            if (OverlapsPlayer(playerPos, spawnZone[i]))
                _validCells.Add(i);
        }

        if (_validCells.Count == 0)
            return;

        float minSqr = notSpawnedEnemyRadius * notSpawnedEnemyRadius;
        float maxSqr = maxEnemyRadius * maxEnemyRadius;

        for (int spawned = 0; spawned < count; spawned++)
        {
            if (!TryPickSpawnPoint(playerPos, minSqr, maxSqr, out Vector3 randomSpawnPos))
                return;

            EnemyPoolable enemy = managerSO.Pop<EnemyPoolable>(enemyItem);
            if (enemy == null)
                return;

            enemy.OnSpawn(randomSpawnPos);
        }
    }

    private bool TryPickSpawnPoint(Vector3 playerPos, float minSqr, float maxSqr, out Vector3 point)
    {
        point = default;
        const int attemptCount = 8;

        for (int attempt = 0; attempt < attemptCount; attempt++)
        {
            SpawnCell cell = spawnZone[_validCells[Random.Range(0, _validCells.Count)]];
            Vector3 candidate = cell.RandomPoint();
            float dx = candidate.x - playerPos.x;
            float dz = candidate.z - playerPos.z;
            float distanceSqr = dx * dx + dz * dz;
            if (distanceSqr <= minSqr)
                continue;
            if (maxEnemyRadius > 0f && distanceSqr >= maxSqr)
                continue;

            point = candidate;
            return true;
        }

        return false;
    }
}
