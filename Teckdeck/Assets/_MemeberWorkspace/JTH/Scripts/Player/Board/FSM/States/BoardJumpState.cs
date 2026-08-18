using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM.States
{
    public class BoardJumpState : AbstractBoardState
    {
        private readonly IJumpable _jumpableController;
        
        public BoardJumpState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
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
            float pushDir = Player.PlayerInput.CurrentMove.y;
            if (Mathf.Abs(pushDir) > InputDeadZone)
                ChangeStateByPushDir(pushDir);
            else
                ChangeStateBySpeedBand();
        }
    }
}