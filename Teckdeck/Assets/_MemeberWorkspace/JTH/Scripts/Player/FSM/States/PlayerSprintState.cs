using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;

namespace JTH.Player.FSM.States
{
    public class PlayerSprintState : AbstractState
    {
        public PlayerSprintState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }
    }
}