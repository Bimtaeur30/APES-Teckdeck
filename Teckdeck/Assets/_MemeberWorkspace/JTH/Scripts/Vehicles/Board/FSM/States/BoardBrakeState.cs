using JTH.Vehicles.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles.Board.FSM.States
{
    public class BoardBrakeState : AbstractVehicleState
    {
        private readonly BoardController Board;
        private readonly IBrakable Brakable;
        private readonly BoardTrigger _boardTrigger;
        private float _pushDir;

        public BoardBrakeState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
            Board = (BoardController)owner;
            Brakable = owner.GetModule<IBrakable>();
            _boardTrigger = owner.GetModule<BoardTrigger>();
            Debug.Assert(_boardTrigger != null, "BoardTrigger 모듈이 없습니다.");
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);

            PlayerInput.OnMovementChange += HandleMovementChange;
            _boardTrigger.OnBrakeStarted += HandleBrakeStart;
            _pushDir = PlayerInput.CurrentMove.y;
            ControlMovement.Turn(PlayerInput.CurrentMove.x);
        }

        public override void Update()
        {
            base.Update();

            if (Brakable.ShouldBrake(_pushDir) == false)
                Board.ChangeState(BoardState.Push, 0.1f);
        }

        public override void Exit()
        {
            PlayerInput.OnMovementChange -= HandleMovementChange;
            _boardTrigger.OnBrakeStarted -= HandleBrakeStart;
            if (Brakable.IsBraking)
                Brakable.EndBrake();
            base.Exit();
        }

        private void HandleBrakeStart()
        {
            Brakable.Brake();
        }

        private void HandleMovementChange(Vector2 moveDir)
        {
            ControlMovement.Turn(moveDir.x);
            _pushDir = moveDir.y;

            if (Mathf.Abs(moveDir.y) < InputDeadZone)
                BoardStateUtil.ChangeStateBySpeedBand(Board, ControlMovement);
        }
    }
}
