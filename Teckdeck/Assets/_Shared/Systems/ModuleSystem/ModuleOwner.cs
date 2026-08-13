using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModuleSystem
{
    public class ModuleOwner : MonoBehaviour
    {
        protected Dictionary<Type, IModule> ModuleDict;

        protected virtual void Awake()
        {
            ModuleDict = GetComponentsInChildren<IModule>().ToDictionary(module => module.GetType());
            InitializeModules();
            AfterInitializeModules();
        }
        
        protected virtual void Start(){}
        
        protected virtual void InitializeModules()
        {
            foreach (IModule module in ModuleDict.Values)
            {
                module.Initialize(this);
            }
        }
        
        protected virtual void AfterInitializeModules()
        {
            foreach (IAfterInitModule module in ModuleDict.Values.OfType<IAfterInitModule>())
            {
                module.AfterInit();
            }
        }
        
        public T GetModule<T>() 
        {
            if (ModuleDict.TryGetValue(typeof(T), out IModule module))
            {
                return (T)module;
            }

            IModule findModule = ModuleDict.Values.FirstOrDefault(moduleType => moduleType is T);
            
            if(findModule is T castedModule)
                return castedModule;

            return default;
        }

    }
}