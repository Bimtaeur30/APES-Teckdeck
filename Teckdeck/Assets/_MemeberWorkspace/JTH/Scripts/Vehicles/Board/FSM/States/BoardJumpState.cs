using JTH.Vehicles.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles.Board.FSM.States
{
    public class BoardJumpState : AbstractVehicleState
    {
        private readonly BoardController Board;
        private readonly IBrakable Brakable;
        private readonly IJumpable _jumpableController;
        
        public BoardJumpState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
            Board = (BoardController)owner;
            Brakable = owner.GetModule<IBrakable>();
            _jumpableController = owner.GetModule<IJumpable>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            ControlMovement.Turn(0f);
            _jumpableController.OnJumpEnded += HandleJumpEnd;
            _jumpableController.Jump();
        }

        public override void Exit()
        {
            base.Exit();
            
            _jumpableController.OnJumpEnded -= HandleJumpEnd;
        }

        private void HandleJumpEnd()
        {
            float pushDir = Board.PlayerInput.CurrentMove.y;
            if (Mathf.Abs(pushDir) > InputDeadZone)
                BoardStateUtil.ChangeStateByPushDir(Board, Brakable, pushDir);
            else
                BoardStateUtil.ChangeStateBySpeedBand(Board, ControlMovement);
        }
    }
}