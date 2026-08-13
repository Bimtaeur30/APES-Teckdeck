using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;

namespace JTH.Player.FSM.States
{
    public class PlayerJumpState : AbstractState
    {
        public PlayerJumpState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }
    }
}