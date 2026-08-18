using AnimatorSystem;
using JTH.Player.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board.Movement
{
    public class BoardPushMove : MonoBehaviour, IModule, IPushable
    {
        [field: SerializeField] public HashDataSO FrontPushHash { get; private set; }
        [field: SerializeField] public HashDataSO BackPushHash { get; private set; }
        [SerializeField] private BoardMovementSO movementData;
        [SerializeField] private Rigidbody rbCompo;

        private ModuleOwner _owner;
        private IControlMovement _movement;
        private float _pushDirection;

        public bool IsPushing { get; private set; }

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _movement = owner.GetModule<IControlMovement>();
            Debug.Assert(movementData != null, "플레이어 이동 데이터가 없습니다.");
            Debug.Assert(rbCompo != null, "플레이어 Rigidbody가 없습니다.");
        }

        public void Push(float direction)
        {
            _pushDirection = direction;
            IsPushing = true;
        }

        public void EndPush()
        {
            IsPushing = false;
            _pushDirection = 0f;
        }

        private void FixedUpdate()
        {
            if (IsPushing == false || rbCompo == null || _owner == null)
                return;

            if (_movement != null && _movement.IsManualMoving)
                return;

            rbCompo.AddForce(
                _owner.transform.forward * (movementData.PushPower * _pushDirection),
                ForceMode.Acceleration);
        }
    }
}
