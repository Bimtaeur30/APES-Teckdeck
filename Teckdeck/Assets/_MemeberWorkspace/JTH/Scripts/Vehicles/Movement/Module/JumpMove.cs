using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JTH.Vehicles.Movement.SO;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles.Movement.Module
{
    public class JumpMove : MonoBehaviour, IModule, IJumpable
    {
        [SerializeField] private JumpMoveSO movementData;
        [SerializeField] private Rigidbody rbCompo;
        
        private VehicleController _vehicle;
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
            _vehicle = (VehicleController)owner;
            _groundChecker = owner.GetModule<IGroundChecker>();
            Debug.Assert(_groundChecker != null, "플레이어 지면 체크 모듈이 없습니다.");
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
            Vector3 up = _vehicle.transform.up;
            
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