using _Shared.Systems.RegistrySystem.Runtime;
using AnimatorSystem;
using ModuleSystem;
using UnityEngine;

namespace FsmSystem
{
    public abstract class AbstractState : IRegistryItem
    {
        [SerializeField] protected HashDataSO StateAnimParam;
        
        protected ModuleOwner Owner;
        protected IStateTransition Transition { get; private set; }

        protected int StateClipHash; //해당 상태의 애니메이션 클립 해시
        protected IAnimatorRenderer Renderer;

        internal void BindTransition(IStateTransition transition)
        {
            Transition = transition;
        }

        public virtual void InitializeState(ModuleOwner owner)
        {
            Owner = owner;
            
            Renderer = owner.GetModule<IAnimatorRenderer>();
            StateClipHash = StateAnimParam == null ? 0 : StateAnimParam.HashValue;
        }

        public virtual void Enter(float transitionDuration, int layerIndex = 0)
        {
            if(StateClipHash != 0)
                Renderer.PlayClip(StateClipHash, 0f, transitionDuration, layerIndex);
        }

        public virtual void Update() {}
        public virtual void Exit() {}
    }
}
