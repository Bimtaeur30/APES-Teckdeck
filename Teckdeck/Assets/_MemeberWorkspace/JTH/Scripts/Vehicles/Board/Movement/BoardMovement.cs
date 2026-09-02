using JTH.Vehicles.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles.Board.Movement
{
    public class BoardMovement : MonoBehaviour, IModule, IControlMovement
    {
        [SerializeField] private BoardMovementSO boardMovementData;
        [SerializeField] private Rigidbody rbCompo;

        private ModuleOwner _owner;
        private IJumpable _jumpable;
        private IDriftable _driftable;
        private IGroundChecker _groundChecker;

        private float _turnSpeed;
        private float _turnDirection;

        public bool IsManualMoving { get; set; } = false;
        
        public BoardSpeedBand SpeedBand
        {
            get
            {
                if (rbCompo == null || boardMovementData == null)
                    return BoardSpeedBand.Stopped;

                float forwardSpeed = Mathf.Abs(Vector3.Dot(rbCompo.linearVelocity, _owner.transform.forward));

                if (forwardSpeed >= boardMovementData.TuckSpeed)
                    return BoardSpeedBand.Tuck;
                if (forwardSpeed >= boardMovementData.StoppedSpeed)
                    return BoardSpeedBand.Ride;

                return BoardSpeedBand.Stopped;
            }
        }

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _jumpable = owner.GetModule<IJumpable>();
            _groundChecker = owner.GetModule<IGroundChecker>();
            _driftable = owner.GetModule<IDriftable>();
            Debug.Assert(boardMovementData != null, "플레이어 이동 데이터가 없습니다.");
            Debug.Assert(rbCompo != null, "플레이어 Rigidbody가 없습니다.");
            Debug.Assert(_jumpable != null, "JumpMove가 없습니다.");
            Debug.Assert(_driftable != null, "DriftMove가 없습니다.");
            Debug.Assert(_groundChecker != null, "GroundChecker가 없습니다.");
        }

        public void Turn(float direction)
        {
            _turnDirection = direction;
        }

        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            _owner.transform.SetPositionAndRotation(position, rotation);
        }

        private void Update()
        {
            if (boardMovementData == null || IsManualMoving)
                return;

            CalculationRotation();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            _driftable.ApplySideGrip(_turnDirection != 0);
        }

        private void CalculationRotation()
        {
            float targetSpeed = _turnDirection * boardMovementData.MaxTurnSpeed;
            float decay = -boardMovementData.Decay;
            
            _turnSpeed = Mathf.Lerp(
                _turnSpeed,
                targetSpeed,
                1f - Mathf.Exp(decay * Time.deltaTime)
            );
        }

        private void ApplyMovement()
        {
            if (rbCompo == null || _owner == null || IsManualMoving)
                return;

            Vector3 up = _owner.transform.up;

            if (_jumpable.IsJumping == false && _groundChecker.IsGrounded)
            {
                float frontVel = Vector3.Dot(rbCompo.linearVelocity, _owner.transform.forward);
                if (Mathf.Abs(frontVel) >= boardMovementData.RotationThreshold || _driftable.DoDrift)
                {
                    float turnSpeed = _turnSpeed;
                    if (_driftable.DoDrift)
                        turnSpeed *= boardMovementData.DoDriftTurnSpeed;
                    else if (_driftable.IsDrifting)
                        turnSpeed *= boardMovementData.DriftTurnSpeed;
                    Quaternion yaw = Quaternion.AngleAxis(turnSpeed * Time.fixedDeltaTime, up);
                    rbCompo.MoveRotation(yaw * rbCompo.rotation);
                }

                ApplyBaseResistance(up);
            }
        }

        private void ApplyBaseResistance(Vector3 up)
        {
            Vector3 vel = Vector3.ProjectOnPlane(rbCompo.linearVelocity, up);
            float speed = vel.magnitude;
            if (speed > boardMovementData.BaseDecelThreshold)
                return;

            if (speed < 0.0001f)
                return;

            Vector3 vertical = rbCompo.linearVelocity - vel;
            vel = Vector3.MoveTowards(vel, Vector3.zero, boardMovementData.BaseDecel * Time.fixedDeltaTime);
            rbCompo.linearVelocity = vel + vertical;
        }
    }
}
