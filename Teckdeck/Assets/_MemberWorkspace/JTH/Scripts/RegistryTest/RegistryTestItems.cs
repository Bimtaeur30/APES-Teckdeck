using System;
using System.Collections.Generic;
using _Shared.Systems.InspectorSystem.Runtime;
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

        //PHASE-021 ReadOnlyField 검증용
        [SerializeField, ReadOnlyField] private int hitCount = 7;
        [SerializeField, ReadOnlyField] private List<int> history = new List<int> { 1, 2 };
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

    [Serializable]
    public class RegistryTestViaInterfaceItem : IRegistryTestSkill
    {
        [SerializeField] private int a;
    }

    //PHASE-020 OnRuntimeCreated 호출 검증용. 부모가 구현하는 경우
    [Serializable]
    public abstract class RegistryTestInitBase : IRegistryItem, IRegistryCreatedReceiver
    {
        [NonSerialized] public int createdCount;

        public virtual void OnRuntimeCreated() => createdCount++;
    }

    [Serializable]
    public class RegistryTestInitItem : RegistryTestInitBase { }

    [Serializable]
    public class RegistryTestInitOverrideItem : RegistryTestInitBase
    {
        [NonSerialized] public bool overrideCalled;

        public override void OnRuntimeCreated()
        {
            base.OnRuntimeCreated();
            overrideCalled = true;
        }
    }

    //자식만 구현하는 경우
    [Serializable]
    public class RegistryTestChildReceiverItem : RegistryTestItemBase, IRegistryCreatedReceiver
    {
        [NonSerialized] public int createdCount;

        public void OnRuntimeCreated() => createdCount++;
    }
}
