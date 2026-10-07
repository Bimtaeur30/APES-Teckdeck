using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Shared.Systems.RegistrySystem.Runtime
{
    public class RegistryRuntime : MonoBehaviour
    {
        [SerializeField] private RegistrySO registrySO;
        
        private Dictionary<int, IRegistryItem> registryDict;

        public void GenerateEntryInstances(params object[] args)
        {
            if (registrySO == null)
                return;
            
            registryDict = new Dictionary<int, IRegistryItem>();

            foreach (var entry in registrySO.entries)
            {
                if (entry.registryItem == null)
                    continue;
                IRegistryItem registryItem = Activator.CreateInstance(
                    entry.registryItem.GetType(), args) as IRegistryItem;
                registryDict.Add(entry.enumValue, registryItem);
            }
        }
    }
}