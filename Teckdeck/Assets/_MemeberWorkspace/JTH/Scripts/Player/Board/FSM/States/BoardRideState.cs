using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM.States
{
    public class BoardRideState : AbstractBoardState
    {
        public BoardRideState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            Player.PlayerInput.OnMovementChange += HandleMovementChange;
            ControlMovement.Turn(Player.PlayerInput.CurrentMove.x);
        }

        public override void Exit()
        {
            Player.PlayerInput.OnMovementChange -= HandleMovementChange;
            base.Exit();
        }

        private void HandleMovementChange(Vector2 moveDir)
        {
            ControlMovement.Turn(moveDir.x);

            if (Mathf.Abs(moveDir.y) > InputDeadZone)
                ChangeStateByPushDir(moveDir.y);
        }
    }
}
