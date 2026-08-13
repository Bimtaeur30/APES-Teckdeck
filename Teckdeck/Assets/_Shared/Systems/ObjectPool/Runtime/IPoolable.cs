using UnityEngine;

namespace _Shared.Systems.ObjectPool.Runtime
{
    public interface IPoolable
    {
        PoolItemSO Item { get; set; }
        GameObject GameObject { get; }
        void ResetItem();
    }
}