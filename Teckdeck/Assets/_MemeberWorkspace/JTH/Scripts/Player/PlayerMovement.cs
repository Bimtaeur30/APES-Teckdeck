using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace JTH.Player
{
    public class PlayerMovement : MonoBehaviour, IModule, IControlMovement
    {
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private Rigidbody rbCompo;
        [SerializeField] private float moveSpeed = 5f;
        
        private float _verticalVelocity;
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
        }

        private void CalculationRotation()
        {
            
        }

        private void FixedUpdate()
        {
            rbCompo.AddForce(transform.forward * _movementDirection.y, ForceMode.Acceleration);
        }
    }
}