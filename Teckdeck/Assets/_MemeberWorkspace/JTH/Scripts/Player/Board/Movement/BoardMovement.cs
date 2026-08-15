using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.Movement
{
    public class BoardMovement : MonoBehaviour, IModule, IControlMovement
    {
        [SerializeField] private BoardMovementSO movementData;
        [SerializeField] private Rigidbody rbCompo;

        private ModuleOwner _owner;
        private IJumpable _jumpMove;

        private float _turnSpeed;
        private float _turnDirection;

        public bool IsManualMoving { get; set; } = false;
        
        public BoardSpeedBand SpeedBand
        {
            get
            {
                if (rbCompo == null || movementData == null)
                    return BoardSpeedBand.Stopped;

                float forwardSpeed = Mathf.Abs(Vector3.Dot(rbCompo.linearVelocity, _owner.transform.forward));

                if (forwardSpeed >= movementData.TuckSpeed)
                    return BoardSpeedBand.Tuck;
                if (forwardSpeed >= movementData.StoppedSpeed)
                    return BoardSpeedBand.Ride;

                return BoardSpeedBand.Stopped;
            }
        }

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _jumpMove = owner.GetModule<IJumpable>();
            Debug.Assert(movementData != null, "플레이어 이동 데이터가 없습니다.");
            Debug.Assert(rbCompo != null, "플레이어 Rigidbody가 없습니다.");
            Debug.Assert(_jumpMove != null, "JumpMove가 없습니다.");
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
            if (movementData == null || IsManualMoving)
                return;

            CalculationRotation();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void CalculationRotation()
        {
            float targetSpeed = _turnDirection * movementData.MaxTurnSpeed;

            _turnSpeed = Mathf.Lerp(
                _turnSpeed,
                targetSpeed,
                1f - Mathf.Exp(-movementData.Decay * Time.deltaTime)
            );
        }

        private void ApplyMovement()
        {
            if (rbCompo == null || _owner == null || IsManualMoving)
                return;

            Vector3 up = _owner.transform.up;

            if (_jumpMove.IsJumping == false)
            {
                Quaternion yaw = Quaternion.AngleAxis(_turnSpeed * Time.fixedDeltaTime, up);
                rbCompo.MoveRotation(yaw * rbCompo.rotation);
                ApplySideGrip(up);
                ApplyBaseResistance(up);
            }
        }

        private void ApplySideGrip(Vector3 up)
        {
            Vector3 vel = Vector3.ProjectOnPlane(rbCompo.linearVelocity, up);
            Vector3 forward = Vector3.ProjectOnPlane(_owner.transform.forward, up);
            //0.01부터 무시 (Epsilon은 너무 작아서 안됨)
            if (vel.sqrMagnitude < 0.0001f || forward.sqrMagnitude < 0.0001f)
                return;

            forward.Normalize();
            float forwardSpeed = Vector3.Dot(vel, forward);
            Vector3 forwardVel = forward * forwardSpeed;
            Vector3 sideVel = vel - forwardVel;
            Vector3 vertical = rbCompo.linearVelocity - vel;
            float absAngle = Mathf.Abs(Vector3.SignedAngle(forward * (forwardSpeed > 0 ? 1 : -1), vel, up));
            if (absAngle < movementData.SnapAngle)
            {
                rbCompo.linearVelocity = forwardVel.normalized * vel.magnitude + vertical;
                return;
            }

            bool isBroken = absAngle >= movementData.BreakAngle
                            && vel.magnitude > movementData.BreakSpeed;

            if (isBroken)
                sideVel = Vector3.MoveTowards(
                    sideVel,
                    Vector3.zero,
                    movementData.KineticDecel * Time.fixedDeltaTime);
            else
                sideVel *= Mathf.Exp(-movementData.GripDecay * Time.fixedDeltaTime);

            rbCompo.linearVelocity = forwardVel + sideVel + vertical;
        }

        private void ApplyBaseResistance(Vector3 up)
        {
            Vector3 vel = Vector3.ProjectOnPlane(rbCompo.linearVelocity, up);
            float speed = vel.magnitude;
            if (speed > movementData.BaseDecelThreshold)
                return;

            if (speed < 0.0001f)
                return;

            Vector3 vertical = rbCompo.linearVelocity - vel;
            vel = Vector3.MoveTowards(vel, Vector3.zero, movementData.BaseDecel * Time.fixedDeltaTime);
            rbCompo.linearVelocity = vel + vertical;
        }
    }
}
