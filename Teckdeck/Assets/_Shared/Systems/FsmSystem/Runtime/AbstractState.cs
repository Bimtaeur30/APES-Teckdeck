using ModuleSystem;

namespace _Shared.Systems.FsmSystem.Runtime
{
    public abstract class AbstractState
    {
        protected readonly ModuleOwner Owner;

        public AbstractState(ModuleOwner owner)
        {
            Owner = owner;
        }

        public virtual void Enter(float transitionDuration, int layerIndex = 0) { }
        public virtual void Update() {}
        public virtual void Exit() {}
    }
}