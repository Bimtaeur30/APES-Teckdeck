using _Shared.Systems.ObjectPool.Runtime;
using CoreSystem;
using UnityEngine;

public class TestWaveManager : MonoBehaviour
{
    [SerializeField] private EventChannelSO spawnChannel;
    [SerializeField] private PoolItemSO enemyItem;
    [SerializeField] private Transform player;
    [SerializeField] private int spawnCount = 3;

    private readonly SpawnEnemyEvent _spawnEvent = new();

    private void Start()
    {
        SpawnWave();
    }

    public void SpawnWave()
    {
        if (spawnChannel == null || enemyItem == null || player == null)
            return;

        spawnChannel.RaiseEvent(_spawnEvent.Init(enemyItem, player.position, spawnCount));
    }
}
