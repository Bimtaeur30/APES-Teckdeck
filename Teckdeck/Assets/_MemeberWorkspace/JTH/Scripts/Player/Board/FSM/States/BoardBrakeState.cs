using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM.States
{
    public class BoardBrakeState : AbstractBoardState
    {
        private readonly BoardTrigger _boardTrigger;
        private float _pushDir;

        public BoardBrakeState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
            _boardTrigger = owner.GetModule<BoardTrigger>();
            Debug.Assert(_boardTrigger != null, "BoardTrigger 모듈이 없습니다.");
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);

            Player.PlayerInput.OnMovementChange += HandleMovementChange;
            _boardTrigger.OnBrakeStarted += HandleBrakeStart;
            _pushDir = Player.PlayerInput.CurrentMove.y;
            ControlMovement.Turn(Player.PlayerInput.CurrentMove.x);
        }

        public override void Update()
        {
            base.Update();

            if (Brakable.ShouldBrake(_pushDir) == false)
                Player.ChangeState(BoardState.Push, 0.1f);
        }

        public override void Exit()
        {
            Player.PlayerInput.OnMovementChange -= HandleMovementChange;
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
                ChangeStateBySpeedBand();
        }
    }
}
