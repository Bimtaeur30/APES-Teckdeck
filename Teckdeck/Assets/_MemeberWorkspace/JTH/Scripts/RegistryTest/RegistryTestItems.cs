using System;
using _Shared.Systems.RegistrySystem.Runtime;
using UnityEngine;

namespace JTH.RegistryTest
{
    //레지스트리 에디터(PHASE-010) 검증용. 확인이 끝나면 지워도 된다.
    [Serializable]
    public abstract class RegistryTestItemBase : IRegistryItem
    {
        [SerializeField] protected float cooldown = 1f;
    }

    [Serializable]
    public class RegistryTestDefaultCtorItem : RegistryTestItemBase
    {
        [SerializeField] private GameObject effectPrefab;
    }

    [Serializable]
    public class RegistryTestArgsCtorItem : RegistryTestItemBase
    {
        [SerializeField] private int comboCount = 3;

        public RegistryTestArgsCtorItem(int comboCount) => this.comboCount = comboCount;
    }

    public class RegistryTestNotSerializableItem : RegistryTestItemBase
    {
    }

    //PHASE-011 베이스 후보 검증용
    public interface IRegistryTestSkill : IRegistryItem { }

    public class RegistryTestViaInterfaceItem : IRegistryTestSkill
    {
        [SerializeField] private int a;
    }

    [Serializable]
    public abstract class RegistryTestInitBase : IInitRegistryItem
    {
        public void Init() { }
    }

    [Serializable]
    public class RegistryTestInitItem : RegistryTestInitBase { }
}
