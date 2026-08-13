using AnimatorSystem;
using ModuleSystem;

namespace _Shared.Systems.FsmSystem.Runtime
{
    public abstract class AbstractState
    {
        protected readonly ModuleOwner Owner;
        
        protected readonly int StateClipHash; //해당 상태의 애니메이션 클립 해시
        protected readonly IAnimatorRenderer Renderer;

        public AbstractState(ModuleOwner owner, int stateClipHash)
        {
            Owner = owner;
            
            StateClipHash = stateClipHash;
            Renderer = owner.GetModule<IAnimatorRenderer>();
        }

        public virtual void Enter(float transitionDuration, int layerIndex = 0)
        {
            Renderer.PlayClip(StateClipHash, 0f, transitionDuration, layerIndex);
        }

        public virtual void Update() {}
        public virtual void Exit() {}
    }
}