using System;
using System.Collections;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
namespace Platformer
{   
    public class CameraManager : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Anywhere] CinemachineFreeLook _freeLookVCam;
        
        [Header("Settings")]
        [SerializeField, Range(0.5f, 3f)] float speedMultiplayer = 1f;

        bool isRMBPressed;
        bool cameraMovementLook;

        private void OnEnable()
        {
            input.Look += OnLook;
            input.EnableMouseControllCamera += OnEnableMouseControllCamera;
            input.DisableMouseControllCamera += OnDisableMouseControllCamera;
        }   
        
        private void OnDisable() {
            input.Look -= OnLook;
            input.EnableMouseControllCamera -= OnEnableMouseControllCamera;
            input.DisableMouseControllCamera -= OnDisableMouseControllCamera;
        }

        private void OnEnableMouseControllCamera()
        {
            isRMBPressed = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }

        private void OnDisableMouseControllCamera()
        {
            isRMBPressed = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
            _freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
        }

        IEnumerator DisableMouseForFrame()
        {
            cameraMovementLook = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLook = false;
        }

        private void OnLook(Vector2 camMovement, bool isDeviseMouse)
        {
            if(cameraMovementLook) return;
            if(isDeviseMouse && !isRMBPressed) return;

            float deviceMultiplayer = isDeviseMouse ? Time.fixedDeltaTime : Time.deltaTime;
            _freeLookVCam.m_XAxis.m_InputAxisValue = camMovement.x * speedMultiplayer * deviceMultiplayer;
            _freeLookVCam.m_YAxis.m_InputAxisValue = camMovement.y * speedMultiplayer * deviceMultiplayer;
        }
    }
}