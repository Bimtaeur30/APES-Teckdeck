using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace _MemeberWorkspace.KTJ._02_Script.Player.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/InputSO", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<Vector2> OnMovementChange;
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

        public void OnJump(CallbackContext context)
        {
            if (context.performed)
                OnJumpKeyPressed?.Invoke();
        }
        
        public Vector2 GetMouseScreenPosition()
        {
            return Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : Vector2.zero;
        }
    }
}