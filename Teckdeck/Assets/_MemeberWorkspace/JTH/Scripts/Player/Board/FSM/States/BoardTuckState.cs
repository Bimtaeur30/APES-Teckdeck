using JTH.Player.Board.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM.States
{
    public class BoardTuckState : AbstractBoardState
    {
        public BoardTuckState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            Player.PlayerInput.OnMovementChange += HandleMovementChange;
            ControlMovement.Turn(Player.PlayerInput.CurrentMove.x);
        }

        public override void Update()
        {
            base.Update();

            switch (ControlMovement.SpeedBand)
            {
                case BoardSpeedBand.Ride:
                    Player.ChangeState(BoardState.Ride, 0.1f);
                    break;
                case BoardSpeedBand.Stopped:
                    Player.ChangeState(BoardState.Idle, 0.1f);
                    break;
            }
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
