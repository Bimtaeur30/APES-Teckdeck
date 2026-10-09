using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Shared.Systems.RegistrySystem.Runtime
{
    public class RegistryRuntime : MonoBehaviour
    {
        [SerializeField] private RegistrySO registrySO;
        //플레이 중 인스펙터에서 사본을 보고 조정하기 위한 목록. registryDict와 같은 사본을 가리킨다.
        [SerializeField] private List<RegistryEntry> runtimeEntries = new List<RegistryEntry>();

        private Dictionary<int, IRegistryItem> registryDict;

        public RegistrySO RegistrySO => registrySO;

        private void Awake()
        {
            if (registryDict == null)
                GenerateEntryInstances();
        }

        public void GenerateEntryInstances()
        {
            registryDict = new Dictionary<int, IRegistryItem>();
            runtimeEntries.Clear();

            if (registrySO == null)
                return;

            foreach (RegistryEntry entry in registrySO.entries)
            {
                Type type = entry.registryItem != null
                    ? entry.registryItem.GetType()
                    : RegistryItemValidator.ResolveType(entry.typeName);
#if UNITY_EDITOR
                WarnEntry(entry, type);
#endif
                if (type == null || entry.registryItem == null)
                    continue;

                string json = JsonUtility.ToJson(entry.registryItem);
                IRegistryItem item = (IRegistryItem)JsonUtility.FromJson(json, type);

                if (item is IRegistryCreatedReceiver receiver)
                    receiver.OnRuntimeCreated();

                registryDict.Add(entry.enumValue, item);
                runtimeEntries.Add(new RegistryEntry
                {
                    enumValue = entry.enumValue,
                    enumKeyName = entry.enumKeyName,
                    typeName = entry.typeName,
                    registryItem = item
                });
            }
        }

        //다른 스크립트의 Awake가 이 컴포넌트의 Awake보다 먼저 조회해도 되도록, 아직 만들지 않았으면 여기서 만든다.
        public bool TryGetItem(int key, out IRegistryItem item)
        {
            if (registryDict == null)
                GenerateEntryInstances();

            return registryDict!.TryGetValue(key, out item);
        }

        public bool TryGetItem<T>(int key, out T item) where T : class, IRegistryItem
        {
            item = TryGetItem(key, out IRegistryItem found) ? found as T : null;
            return item != null;
        }

#if UNITY_EDITOR
        //경고는 항목 코드를 잘못 짰다는 개발용 정보라 에디터에서만 찍는다.
        //같은 타입을 쓰는 RegistryRuntime이 여러 개여도 한 번만 찍히도록 이미 찍은 타입 이름을 기억한다.
        private static readonly HashSet<string> WarnedTypeNames = new();

        //Enter Play Mode Options로 도메인 리로드를 끄면 static이 남으므로 플레이마다 비운다.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetWarnedTypeNames() => WarnedTypeNames.Clear();

        private void WarnEntry(RegistryEntry entry, Type type)
        {
            string msg = type == null
                ? $"타입을 찾을 수 없어 건너뜁니다: {entry.typeName}"
                : RegistryItemValidator.GetWarningMsg(type);
            if (msg == null)
                return;

            string warnKey = type != null ? RegistryItemValidator.GetTypeName(type) : $"{registrySO.name}/{entry.enumKeyName}";
            if (!WarnedTypeNames.Add(warnKey))
                return;

            Debug.LogWarning($"[Registry] {registrySO.name}/{entry.enumKeyName}: {msg.Replace("\n", " / ")}", registrySO);
        }
#endif
    }
}
