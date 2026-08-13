using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;

namespace JTH.Player.FSM.States
{
    public class AgentState : AbstractState
    {
        protected readonly int StateClipHash; //해당 상태의 애니메이션 클립 해시
        protected readonly IRenderer Renderer;
        
        public AgentState(ModuleOwner owner, int stateClipHash) : base(owner)
        {
            StateClipHash = stateClipHash;
            Renderer = owner.GetModule<IRenderer>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);

            Renderer.PlayClip(StateClipHash, 0f, transitionDuration, layerIndex);
        }
    }
}