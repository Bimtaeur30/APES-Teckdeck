using ModuleSystem;
using UnityEngine;

namespace JTH.Player
{
    public class PlayerMovement : MonoBehaviour, IModule, IControlMovement
    {
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private Rigidbody rbCompo;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSpeed = 1f;
        [SerializeField] private float maxTurnSpeed = 10f;
        [SerializeField] private float decay = 15f;
        
        private float _verticalVelocity;
        private float _turnSpeed;
        private Vector2 _movementDirection;
        private ModuleOwner _owner;
        
        public bool CanManualMove { get; set; } = false;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }

        public void SetMovementDirection(Vector2 inputDirection)
        {
            _movementDirection = inputDirection;
        }

        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            _owner.transform.SetPositionAndRotation(position, rotation);
        }

        private void Update()
        {
            CalculationRotation();
            
            _owner.transform.RotateAround(transform.position, _owner.transform.up, _turnSpeed * Time.deltaTime);
        }

        private void CalculationRotation()
        {
            float targetSpeed = _movementDirection.x;
            
            _turnSpeed = Mathf.Lerp(
                _turnSpeed,
                targetSpeed,
                1f - Mathf.Exp(-decay * Time.deltaTime)
            );
        }

        private void FixedUpdate()
        {
            rbCompo.AddForce(transform.forward * _movementDirection.y, ForceMode.Acceleration);
        }
    }
}