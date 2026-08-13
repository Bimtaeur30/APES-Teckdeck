using UnityEngine;

namespace ModuleSystem
{
    public abstract class Module : MonoBehaviour, IModule
    {
        protected ModuleOwner Owner;
        public virtual void Initialize(ModuleOwner owner)
        {
            Owner = owner;
        }
    }
}