using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.Movement
{
    public class BoardBrakeMove : MonoBehaviour, IModule, IBrakable
    {
        [SerializeField] private BoardMovementSO movementData;
        [SerializeField] private Rigidbody rbCompo;

        private ModuleOwner _owner;
        private IControlMovement _movement;

        public bool IsBraking { get; private set; }

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _movement = owner.GetModule<IControlMovement>();
            Debug.Assert(movementData != null, "플레이어 이동 데이터가 없습니다.");
            Debug.Assert(rbCompo != null, "플레이어 Rigidbody가 없습니다.");
        }

        public bool ShouldBrake(float longitudinalInput)
        {
            if (rbCompo == null || _owner == null)
                return false;

            float forwardSpeed = Vector3.Dot(rbCompo.linearVelocity, _owner.transform.forward);
            return forwardSpeed * longitudinalInput < 0f
                   && Mathf.Abs(forwardSpeed) > movementData.StoppedSpeed;
        }

        public void Brake()
        {
            IsBraking = true;
        }

        public void EndBrake()
        {
            IsBraking = false;
        }

        private void FixedUpdate()
        {
            if (IsBraking == false || rbCompo == null || _owner == null)
                return;

            if (_movement != null && _movement.IsManualMoving)
                return;

            Vector3 up = _owner.transform.up;
            Vector3 vel = Vector3.ProjectOnPlane(rbCompo.linearVelocity, up);
            Vector3 vertical = rbCompo.linearVelocity - vel;
            vel = Vector3.MoveTowards(vel, Vector3.zero, movementData.BrakeDecel * Time.fixedDeltaTime);
            rbCompo.linearVelocity = vel + vertical;
        }
    }
}
