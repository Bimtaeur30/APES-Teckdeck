using JTH.Player.Board.Movement;
using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM
{
    public abstract class AbstractBoardState : AbstractPlayerState
    {
        protected readonly IBrakable Brakable;

        protected AbstractBoardState(ModuleOwner agent, int stateClipHash) : base(agent, stateClipHash)
        {
            Brakable = agent.GetModule<IBrakable>();
            Debug.Assert(Brakable != null, "Brake 모듈이 없습니다.");
        }

        protected void ChangeStateByPushDir(float pushDir)
        {
            if (Brakable.ShouldBrake(pushDir))
                Player.ChangeState(BoardState.Brake, 0.1f);
            else
                Player.ChangeState(BoardState.Push, 0.1f);
        }
        
        protected void ChangeStateBySpeedBand(float transitionDuration = 0.1f)
        {
            switch (ControlMovement.SpeedBand)
            {
                case BoardSpeedBand.Tuck:
                    Player.ChangeState(BoardState.Tuck, transitionDuration);
                    break;
                case BoardSpeedBand.Ride:
                    Player.ChangeState(BoardState.Ride, transitionDuration);
                    break;
                default:
                    Player.ChangeState(BoardState.Idle, transitionDuration);
                    break;
            }
        }
    }
}