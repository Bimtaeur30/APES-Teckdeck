using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM.States
{
    public class BoardIdleState : AbstractBoardState
    {
        public BoardIdleState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            ControlMovement.Turn(0f);
            Player.PlayerInput.OnMovementChange += HandleMovementChange;
        }

        public override void Exit()
        {
            base.Exit();
            Player.PlayerInput.OnMovementChange -= HandleMovementChange;
        }

        private void HandleMovementChange(Vector2 moveDir)
        {
            ControlMovement.Turn(moveDir.x);

            if (Mathf.Abs(moveDir.y) > InputDeadZone)
                Player.ChangeState(BoardState.Push, transitionDuration: 0.1f);
        }
    }
}
