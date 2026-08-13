using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;

namespace JTH.Player.FSM.States
{
    public class PlayerRideState : AbstractState
    {
        public PlayerRideState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }
    }
}