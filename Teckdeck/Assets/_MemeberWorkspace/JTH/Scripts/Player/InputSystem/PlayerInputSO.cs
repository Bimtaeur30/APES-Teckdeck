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
            OnMovementChange?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSprint(CallbackContext context)
        {
            if (context.performed)
                OnSprintKeyPressed?.Invoke();
        }
    }
}