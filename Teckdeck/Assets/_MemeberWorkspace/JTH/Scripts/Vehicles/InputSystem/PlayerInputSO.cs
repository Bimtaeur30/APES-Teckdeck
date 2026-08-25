using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace JTH.Vehicles.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/InputSO", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<Vector2> OnMovementChange;
        public event Action OnSprintKeyPressed;
        public event Action<bool> OnSpaceKeyChange;
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

        void Controls.IPlayerActions.OnJump(CallbackContext context)
        {
            OnSpace(context);
        }

        public void OnSpace(CallbackContext context)
        {
            if (context.started)
                OnSpaceKeyChange?.Invoke(true);
            else if (context.canceled)
                OnSpaceKeyChange?.Invoke(false);
        }
    }
}
