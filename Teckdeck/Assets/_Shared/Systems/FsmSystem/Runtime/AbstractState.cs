using AnimatorSystem;
using ModuleSystem;

namespace _Shared.Systems.FsmSystem.Runtime
{
    public abstract class AbstractState
    {
        protected readonly ModuleOwner Owner;
        protected IStateTransition Transition { get; private set; }

        protected readonly int StateClipHash; //해당 상태의 애니메이션 클립 해시
        protected readonly IAnimatorRenderer Renderer;

        internal void BindTransition(IStateTransition transition)
        {
            Transition = transition;
        }

        public AbstractState(ModuleOwner owner, int stateClipHash = 0)
        {
            Owner = owner;
            
            StateClipHash = stateClipHash;
            Renderer = owner.GetModule<IAnimatorRenderer>();
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
