using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;

namespace JTH.Player.FSM.States
{
    public class PlayerIdleState : AbstractState
    {
        public PlayerIdleState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
            
        }
    }
}