using JTH.Vehicles.Board.FSM;

namespace JTH.Vehicles.Board
{
    public class BoardController : VehicleController
    {
        public BoardState CurrentState => (BoardState)StateMachine.CurrentStateIdx;
        
        public void ChangeState(BoardState newStateIndex, float transitionDuration)
        {
            base.ChangeState((int)newStateIndex, transitionDuration);
        }
    }
}