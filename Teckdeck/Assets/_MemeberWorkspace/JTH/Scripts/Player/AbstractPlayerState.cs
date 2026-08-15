using _Shared.Systems.FsmSystem.Runtime;
using JTH.Player.Movement;
using ModuleSystem;

namespace JTH.Player
{
    public abstract class AbstractPlayerState : AbstractState
    {
        protected PlayerController Player;
        protected IControlMovement ControlMovement;
        protected const float InputDeadZone = 0.1f; //입력을 안받는 임계값
        
        protected AbstractPlayerState(ModuleOwner agent, int stateClipHash) : base(agent, stateClipHash)
        {
            Player = agent as PlayerController;
            ControlMovement = agent.GetModule<IControlMovement>();
        }
    }
}