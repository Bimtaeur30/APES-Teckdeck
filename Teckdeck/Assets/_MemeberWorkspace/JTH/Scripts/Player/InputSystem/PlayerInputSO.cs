using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace JTH.Player.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/InputSO", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<Vector2> OnMovementChange;
        public event Action OnSprintKeyPressed;
        public event Action OnJumpKeyPressed;
        public Vector2 CurrentMove { get; private set; }

        private Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            if(_controls != null)
                _controls.Player.Disable();
        }

        public void OnMove(CallbackContext context)
        {
            CurrentMove = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(CurrentMove);
        }

        public void OnSprint(CallbackContext context)
        {
            if (context.performed)
                OnSprintKeyPressed?.Invoke();
        }

        public void OnJump(CallbackContext context)
        {
            if (context.performed)
                OnJumpKeyPressed?.Invoke();
        }
    }
}