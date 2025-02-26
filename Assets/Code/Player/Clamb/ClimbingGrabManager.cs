using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Platformer
{
    public class ClimbingGrabManager
    {
        private PlayerClimbingSystem playerClimbingSystem;
        private PlayerController pController;
    
        private int rayAmount = 10;
        private float rayLength = 0.5f;
        private float rayOffset = 0.15f;
        private float rayHight = 0.5f;
        private float rayYHandCorrection = 0.1f;
        private float rayZHandCorrection = 0.1f;

        private Transform transform;

        private bool _hasMatchedTarget;

        public ClimbingGrabManager(
            PlayerClimbingSystem playerClimbingSystem, 
            int rayAmount, 
            float rayLength, 
            float rayOffset, 
            float rayHight,
            float rayYHandCorrection, 
            float rayZHandCorrection
        ){
            this.playerClimbingSystem = playerClimbingSystem;
            this.pController = playerClimbingSystem.pController;
            this.transform = pController.transform;

            // settings
            this.rayAmount = rayAmount;
            this.rayLength = rayLength;
            this.rayOffset = rayOffset;
            this.rayHight = rayHight;
            this.rayYHandCorrection = rayYHandCorrection;
            this.rayZHandCorrection = rayZHandCorrection;
        }

        public void OnUpdate()
        {
            MathTargetToLedge();
        }

        private async void MathTargetToLedge() {
            if (_hasMatchedTarget) return;

            AnimatorStateInfo animState = pController.animatior.GetCurrentAnimatorStateInfo(0);
            bool isCorrectState = animState.IsName("Idle To Braced Hang") && !pController.animatior.IsInTransition(0);
            
            if (!isCorrectState) return; 

            Vector3 handPos = playerClimbingSystem.rayLedgeDownHit.point + (transform.forward * rayZHandCorrection + transform.up * rayYHandCorrection);
            float moveDuration = 0.1f;
            await MovePlayerToTarget(handPos, moveDuration);

            _hasMatchedTarget = true;
        }

        public async Task MovePlayerToTarget(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            transform.position = targetPosition;
        }

        public void CheckOnGroundRay()
        {
            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 rayPosition = transform.position + Vector3.up * rayHight + Vector3.up * rayOffset * i;

                Debug.DrawRay(rayPosition, transform.forward, Color.red);
                if (Physics.Raycast(rayPosition, transform.forward, out playerClimbingSystem.rayLedgeForwardHit, rayLength, playerClimbingSystem.ledgeLayer, QueryTriggerInteraction.Ignore))
                {
                    playerClimbingSystem.canGrabLedge = true;

                    Debug.DrawRay(playerClimbingSystem.rayLedgeForwardHit.point + Vector3.up * 0.2f, Vector3.down * 0.5f, Color.blue);
                    Physics.Raycast(playerClimbingSystem.rayLedgeForwardHit.point + Vector3.up * 0.2f, Vector3.down, out playerClimbingSystem.rayLedgeDownHit, 0.5f, playerClimbingSystem.ledgeLayer);
                    return;
                }
                else
                {
                    playerClimbingSystem.canGrabLedge = false;
                }
            }
        }
    }
}