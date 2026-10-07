using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Shared.Systems.RegistrySystem.Runtime
{
    [Serializable]
    public class ComponentListItem
    {
        public int enumValue;
        public string enumKeyName;
        [SerializeReference] public IRegistryItem registryItem;
    }
    
    [CreateAssetMenu(fileName = "ComponentRegistrySO", menuName = "Lib/Registry/ComponentRegistrySO", order = 0)]
    public class ComponentRegistrySO : ScriptableObject
    {
        public string baseTypeName;
        public List<ComponentListItem> components = new List<ComponentListItem>();
        public string enumName;
        public string enumFolderGuid;
        public int lastEnumValue = -1;
        public List<string> lastEnums = new List<string>();
        public List<string> skipNamespaces = new List<string>();
    }
}