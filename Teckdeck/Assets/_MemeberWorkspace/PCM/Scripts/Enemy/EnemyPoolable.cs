using _Shared.Systems.ObjectPool.Runtime;
using Assets._MemeberWorkspace.PCM.Scripts.Spawn.Interface;
using Enemy;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPoolable : AbstractMonoPoolable, ISpawnable
{
    [SerializeField] private PoolManagerSO poolManager;
    private AbstractEnemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<AbstractEnemy>();
        _enemy.OnDeath.AddListener(ReturnToPool);
    }

    private void OnDestroy()
    {
        _enemy.OnDeath.RemoveListener(ReturnToPool);
    }

    public void ReturnToPool()
    {
        poolManager.Push(this);
    }

    public override void ResetItem()
    {
        base.ResetItem();
    }

    public void OnSpawn(Vector3 pos)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            pos = hit.position;

        transform.SetParent(null);
        transform.position = pos;

        NavMeshAgent agent = _enemy.NavMovement?.NavAgent;
        if (agent != null)
            agent.Warp(pos);

        _enemy.PrepareForSpawn();
    }
}
