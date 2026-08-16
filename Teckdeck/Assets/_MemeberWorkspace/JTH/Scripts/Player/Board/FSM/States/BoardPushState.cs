using AnimatorSystem;
using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.FSM.States
{
    public class BoardPushState : AbstractBoardState
    {
        private readonly IPushable _pushable;
        private readonly BoardTrigger _boardTrigger;
        private float _pushDirection;

        public BoardPushState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
        {
            _pushable = owner.GetModule<IPushable>();
            _boardTrigger = owner.GetModule<BoardTrigger>();
            Debug.Assert(_pushable != null, "Push 모듈이 없습니다.");
            Debug.Assert(_boardTrigger != null, "BoardTrigger 모듈이 없습니다.");
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            //base.Enter(transitionDuration, layerIndex); 실행하면 안된다.
            
            _boardTrigger.OnPushStarted += HandlePushStarted;
            _boardTrigger.OnPushEnded += HandlePushEnded;
            Player.PlayerInput.OnMovementChange += HandleMovementChange;

            ApplyMoveInput(Player.PlayerInput.CurrentMove);
        }

        public override void Exit()
        {
            Player.PlayerInput.OnMovementChange -= HandleMovementChange;
            _boardTrigger.OnPushStarted -= HandlePushStarted;
            _boardTrigger.OnPushEnded -= HandlePushEnded;

            if (_pushable.IsPushing)
                _pushable.EndPush();

            _pushDirection = 0;
            
            base.Exit();
        }

        private void HandleMovementChange(Vector2 moveDir)
        {
            ApplyMoveInput(moveDir);
        }

        private void ApplyMoveInput(Vector2 moveDir)
        {
            ControlMovement.Turn(moveDir.x);

            float nextDirection = GetPushDirection(moveDir.y);
            if (Mathf.Approximately(nextDirection, _pushDirection))
                return;

            _pushDirection = nextDirection;

            if (Mathf.Approximately(_pushDirection, 0f))
            {
                ChangeStateBySpeedBand();
                return;
            }
            
            if (Brakable.ShouldBrake(_pushDirection))
            {
                Player.ChangeState(BoardState.Brake, 0.1f);
                return;
            } 
            if (_pushable.IsPushing)
                _pushable.EndPush();

            PlayPushClip();
        }

        private static float GetPushDirection(float y)
        {
            if (Mathf.Abs(y) <= InputDeadZone)
                return 0f;

            return y > 0f ? 1f : -1f;
        }

        private void PlayPushClip()
        {
            HashDataSO hash = _pushDirection > 0f
                ? _pushable.FrontPushHash
                : _pushable.BackPushHash;
            
            Debug.Assert(hash != null, "Push HashData가 없습니다.");
            Renderer.PlayClip(hash.HashValue, 0f, 0.1f);
        }

        private void HandlePushStarted()
        {
            _pushable.Push(_pushDirection);
        }

        private void HandlePushEnded()
        {
            _pushable.EndPush();
        }
    }
}
