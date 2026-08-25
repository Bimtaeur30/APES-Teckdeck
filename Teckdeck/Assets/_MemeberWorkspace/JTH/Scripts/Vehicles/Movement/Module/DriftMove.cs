using JTH.Vehicles.Movement.SO;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles.Movement.Module
{
    public class DriftMove : MonoBehaviour, IModule, IDriftable
    {
        [SerializeField] private DriftMoveSO movementData;
        [SerializeField] private Rigidbody rbCompo;
        
        public bool DoDrift { get; set; }
        
        private bool CanDrift => _jumpable.IsJumping == false
                                 && _groundChecker.IsGrounded;
        
        private ModuleOwner _owner;
        private IJumpable _jumpable;
        private IGroundChecker _groundChecker;
        
        private float _driftStartTime;
        private bool _driftThisFrame;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _jumpable = owner.GetModule<IJumpable>();
            _groundChecker = owner.GetModule<IGroundChecker>();
            Debug.Assert(movementData != null, "Movement data not set");
            Debug.Assert(_jumpable != null, "Jumpable not set");
            Debug.Assert(_groundChecker != null, "GroundChecker not set");
        }

        public void ApplySideGrip(bool isTurning)
        {
            Vector3 up = _owner.transform.up;
            Vector3 vel = Vector3.ProjectOnPlane(rbCompo.linearVelocity, up);
            Vector3 forward = _owner.transform.forward;
            //0.01부터 무시 (Epsilon은 너무 작아서 안됨)
            if (vel.sqrMagnitude < 0.0001f || forward.sqrMagnitude < 0.0001f)
                return;

            forward.Normalize();
            float forwardSpeed = Vector3.Dot(vel, forward);
            Vector3 forwardVel = forward * forwardSpeed;
            Vector3 sideVel = vel - forwardVel;
            Vector3 vertical = rbCompo.linearVelocity - vel;
            float absAngle = Mathf.Abs(Vector3.SignedAngle(forward * (forwardSpeed > 0 ? 1 : -1), vel, up));

            // 드리프트가 켜져있었다면 MinDriftTime을 넘었을 때 CutAngle보다 작으면 false, 꺼져있었다면 드리프트를 할 수 있는 상태에서 
            //각도가 0 이상이면 true
            bool driftLastFrame = _driftThisFrame;
            if ((driftLastFrame && (Time.time - _driftStartTime < movementData.MinDriftTime 
                                    || movementData.DriftCutAngle < absAngle
                                    || isTurning) && CanDrift)
                || (DoDrift && driftLastFrame == false && CanDrift && absAngle > 0))
                _driftThisFrame = true;
            else
                _driftThisFrame = false;
            
            if (driftLastFrame == false && _driftThisFrame)
                _driftStartTime = Time.time;

            rbCompo.linearVelocity = forwardVel + vertical;
            if (_driftThisFrame)
                Drift(forwardVel.normalized, sideVel);
        }
        
        private void Drift(Vector3 forward, Vector3 sideVel)
        {
            float beforeSideVel = sideVel.magnitude;
            sideVel = Vector3.MoveTowards(
                sideVel,
                Vector3.zero,
                movementData.DriftDecel * Time.fixedDeltaTime);
            float lostSideVel = beforeSideVel - sideVel.magnitude;

            rbCompo.linearVelocity += sideVel + forward * (lostSideVel * movementData.RecoverySideVelRate);
        }
    }
}