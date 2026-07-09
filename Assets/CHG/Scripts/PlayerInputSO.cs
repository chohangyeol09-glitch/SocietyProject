using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CHG.Scripts
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "CHG/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {

        public event Action<Vector2> OnMoved;
        public event Action OnMovedEnded;
        public event Action OnStopStated;
        public event Action OnStopEnded;
        
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
            _controls.Player.Disable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMoved?.Invoke(context.ReadValue<Vector2>());
            if  (context.canceled)
                OnMovedEnded?.Invoke();
        }

        public void OnStop(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnStopStated?.Invoke();
            if (context.canceled)
                OnStopEnded?.Invoke();
                
        }
    }
}