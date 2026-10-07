using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Shared.Systems.RegistrySystem.Runtime
{
    [Serializable]
    public class RegistryEntry
    {
        public int enumValue;
        public string enumKeyName;
        [SerializeReference] public IRegistryItem registryItem;
    }
    
    [CreateAssetMenu(fileName = "RegistrySO", menuName = "Lib/Registry/RegistrySO", order = 0)]
    public class RegistrySO : ScriptableObject
    {
        public string baseTypeName;
        [FormerlySerializedAs("components")] public List<RegistryEntry> entries = new List<RegistryEntry>();
        public string enumName;
        public string enumFolderGuid;
        public int lastEnumValue = -1;
        public List<string> lastEnums = new List<string>();
        public List<string> skipNamespaces = new List<string>();
    }
}