using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JTH.Player.Board.FSM;
using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.Movement
{
    public class BoardJumpMove : MonoBehaviour, IModule, IJumpable
    {
        [SerializeField] private BoardMovementSO movementData;
        [SerializeField] private Rigidbody rbCompo;
        
        private PlayerController _player;
        private float _lastJumpTime;
        private CancellationTokenSource _jumpCts;
        private IGroundChecker _groundChecker;

        public bool CanJump => _groundChecker.IsGrounded
                               && (Mathf.Approximately(movementData.JumpCooldown, 0)
                                   || Time.time - _lastJumpTime >= movementData.JumpCooldown)
                               && _jumpCts == null;
        public Action OnJumpEnded { get; set; }
        public bool IsJumping => _jumpCts != null;
        
        public void Initialize(ModuleOwner owner)
        {
            _player = (PlayerController)owner;
            _groundChecker = owner.GetModule<IGroundChecker>();
            Debug.Assert(_groundChecker != null, "플레이어 지면 체크 모듈이 없습니다.");
            
            _player.PlayerInput.OnJumpKeyPressed += HandlePlayerJump;
        }

        private void OnDestroy()
        {
            _player.PlayerInput.OnJumpKeyPressed -= HandlePlayerJump;
        }

        private void HandlePlayerJump()
        {
            if (CanJump == true)
                _player.ChangeState(BoardState.Jump, 0.1f);
        }

        public void Jump()
        {
            if (CanJump == false)
                return;

            rbCompo.useGravity = false;
            _jumpCts = new CancellationTokenSource();
            _lastJumpTime = Time.time;
            _groundChecker.OnGrounded += HandleGrounded;
            JumpAsync(_jumpCts.Token).Forget();
        }
        
        private async UniTaskVoid JumpAsync(CancellationToken ct)
        {
            float currentDuration = 0;
            float jumpDuration = movementData.JumpDuration;
            Vector3 up = _player.transform.up;
            
            try
            {
                while (currentDuration < jumpDuration && jumpDuration > 0)
                {
                    float percent = currentDuration / jumpDuration;
                    currentDuration += Time.fixedDeltaTime;
                    Vector3 velocity = rbCompo.linearVelocity;
                    float targetUp = movementData.JumpYVelocity.Evaluate(percent) * movementData.JumpPower;
                    float currentUp = Vector3.Dot(velocity, up);
                    rbCompo.linearVelocity = velocity + up * (targetUp - currentUp);
                    await UniTask.WaitForFixedUpdate(ct);
                }
                rbCompo.useGravity = true;

                if (_jumpCts != null && _groundChecker.IsGrounded)
                    HandleGrounded();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void HandleGrounded()
        {
            if (_jumpCts == null)
                return;
            rbCompo.useGravity = true;
            _jumpCts.Cancel();
            _jumpCts.Dispose();
            _jumpCts = null;

            OnJumpEnded?.Invoke();
            _groundChecker.OnGrounded -= HandleGrounded;
        }
    }
}