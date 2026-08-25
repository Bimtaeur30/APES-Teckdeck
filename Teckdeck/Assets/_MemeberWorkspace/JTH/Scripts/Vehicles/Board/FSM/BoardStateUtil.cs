using JTH.Vehicles.Board.Movement;
using JTH.Vehicles.Movement;

namespace JTH.Vehicles.Board.FSM
{
    public static class BoardStateUtil
    {
        public static void ChangeStateByPushDir(
            BoardController board,
            IBrakable brakable,
            float pushDir,
            float transitionDuration = 0.1f)
        {
            if (brakable.ShouldBrake(pushDir))
                board.ChangeState(BoardState.Brake, 0.1f);
            else
                board.ChangeState(BoardState.Push, 0.1f);
        }
        
        public static void ChangeStateBySpeedBand(
            BoardController board,
            IControlMovement controlMovement,
            float transitionDuration = 0.1f)
        {
            switch (controlMovement.SpeedBand)
            {
                case BoardSpeedBand.Tuck:
                    board.ChangeState(BoardState.Tuck, transitionDuration);
                    break;
                case BoardSpeedBand.Ride:
                    board.ChangeState(BoardState.Ride, transitionDuration);
                    break;
                default:
                    board.ChangeState(BoardState.Idle, transitionDuration);
                    break;
            }
        }
    }
}