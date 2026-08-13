using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;

namespace JTH.Player.FSM.States
{
    public class PlayerPushState : AbstractState
    {
        public PlayerPushState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }
    }
}