using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static LevelBuild;

namespace Platformer
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControllCamera = delegate { };
        public event UnityAction DisableMouseControllCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction Climb = delegate { };
        public event UnityAction Attack = delegate { };


        LevelBuild inputActions;

        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable() 
        {
            if(inputActions == null)
            {
                inputActions = new LevelBuild();
                inputActions.Player.SetCallbacks(this);
            }
        }

        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
           Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        private bool IsDeviceMouse(InputAction.CallbackContext context)
        {
            return context.control.device.name == "Mouse";
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
           switch (context.phase)
           {
            case InputActionPhase.Started:
                EnableMouseControllCamera.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControllCamera.Invoke();
                break;
            default:
                break;
           }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
           {
            case InputActionPhase.Started:
                Jump.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;
            default:
                break;
           }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started)
            {
                Attack.Invoke();
            }
            
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Started)
            {
               Climb.Invoke();
            }
        }
    }
}