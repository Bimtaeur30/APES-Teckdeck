using _Shared.Systems.ObjectPool.Runtime;
using CoreSystem;
using UnityEngine;

public class SpawnEnemyEvent : GameEvent
{
    public PoolItemSO EnemyItem;
    public Vector3 PlayerPos;
    public int Count;

    public SpawnEnemyEvent Init(PoolItemSO enemyItem, Vector3 playerPos, int count)
    {
        EnemyItem = enemyItem;
        PlayerPos = playerPos;
        Count = count;
        return this;
    }
}
