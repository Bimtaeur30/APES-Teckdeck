using JTH.Vehicles.Board.Movement;
using JTH.Vehicles.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles.Board.FSM.States
{
    public class BoardTuckState : AbstractVehicleState
    {
        private readonly BoardController Board;
        private readonly IBrakable Brakable;

        public BoardTuckState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
            Board = (BoardController)owner;
            Brakable = owner.GetModule<IBrakable>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            Board.PlayerInput.OnMovementChange += HandleMovementChange;
            ControlMovement.Turn(Board.PlayerInput.CurrentMove.x);
        }

        public override void Update()
        {
            base.Update();

            switch (ControlMovement.SpeedBand)
            {
                case BoardSpeedBand.Ride:
                    Board.ChangeState(BoardState.Ride, 0.1f);
                    break;
                case BoardSpeedBand.Stopped:
                    Board.ChangeState(BoardState.Idle, 0.1f);
                    break;
            }
        }

        public override void Exit()
        {
            Board.PlayerInput.OnMovementChange -= HandleMovementChange;
            base.Exit();
        }

        private void HandleMovementChange(Vector2 moveDir)
        {
            ControlMovement.Turn(moveDir.x);

            if (Mathf.Abs(moveDir.y) > InputDeadZone)
                BoardStateUtil.ChangeStateByPushDir(Board, Brakable, moveDir.y);
        }
    }
}
