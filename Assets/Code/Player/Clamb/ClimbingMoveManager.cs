
using System.Threading.Tasks;
using UnityEngine;

namespace Platformer
{
    public class ClimbingMoveManager
    {
        private PlayerClimbingSystem playerClimbingSystem;
        private PlayerController pController;

        private int rayAmount = 10;
        private float rayLength = 0.5f;
        private float rayOffset = 0.15f;
        private float rayHight = 0.5f;
        private float rayVerticalGap = 0f;

        RaycastHit hopLedgeForwardHit;
        RaycastHit hopLedgeDownHit;
        public bool hasMatchedTarget;
        private Transform transform;

        public ClimbingMoveManager(
            PlayerClimbingSystem playerClimbingSystem,
            int rayAmount, 
            float rayLength, 
            float rayOffset, 
            float rayHight
        ){
            this.playerClimbingSystem = playerClimbingSystem;
            this.pController = playerClimbingSystem.pController;
            this.transform = pController.transform;

            // settings
            this.rayAmount = rayAmount;
            this.rayLength = rayLength;
            this.rayOffset = rayOffset;
            this.rayHight = rayHight;
        }

        public void OnUpdate()
        {
            MathTargetToLedge();
            if (playerClimbingSystem.isClimbing)
            {
                HopUpDown();
            }
        }

        private void HopUpDown()
        {
            var verticalInp = Input.GetAxis("Vertical");

            if (verticalInp < -0.1f)
            {
                HopDownRayCheck();
            }
            else if (verticalInp > 0.1f)
            {
                HopUpRayCheck();
            }
            
        }

        private void HopUpRayCheck()
        {
            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 rayPosition = transform.position + Vector3.up * rayHight + Vector3.up * rayVerticalGap + Vector3.up * rayOffset * i;
                Debug.DrawRay(rayPosition, transform.forward, Color.green);

                if (Physics.Raycast(rayPosition, transform.forward, out playerClimbingSystem.CGManager.rayLedgeForwardHit, rayLength, playerClimbingSystem.ledgeLayer, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawRay(playerClimbingSystem.CGManager.rayLedgeForwardHit.point + Vector3.up * 0.35f, Vector3.down, Color.green);

                    if (Physics.Raycast(playerClimbingSystem.CGManager.rayLedgeForwardHit.point + Vector3.up * 0.35f, Vector3.down, out hopLedgeDownHit, 0.5f, playerClimbingSystem.ledgeLayer))
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            playerClimbingSystem.HopUp();
                        }
                    }

                    break;
                }
            }
        }

        private void HopDownRayCheck()
        {
            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 rayPosition = transform.position + Vector3.up * rayHight - Vector3.up * rayVerticalGap - Vector3.up * rayOffset * i;
                Debug.DrawRay(rayPosition, transform.forward, Color.green);

                if (Physics.Raycast(rayPosition, transform.forward, out playerClimbingSystem.CGManager.rayLedgeForwardHit, rayLength, playerClimbingSystem.ledgeLayer, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawRay(playerClimbingSystem.CGManager.rayLedgeForwardHit.point + Vector3.up * 0.35f, Vector3.down, Color.green);

                    if (Physics.Raycast(playerClimbingSystem.CGManager.rayLedgeForwardHit.point + Vector3.up * 0.35f, Vector3.down, out hopLedgeDownHit, 0.5f, playerClimbingSystem.ledgeLayer))
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            playerClimbingSystem.HopDown();
                        }
                    }
                    
                    break;
                }
            }
        }

        private async void MathTargetToLedge() 
        { 
            if (hasMatchedTarget) return;

            AnimatorStateInfo animState = pController.animatior.GetCurrentAnimatorStateInfo(0);
            bool isCorrectState = animState.IsName("Braced Hang Hop Up") && !pController.animatior.IsInTransition(0);
            
            if (!isCorrectState) return; 
            Vector3 handDropPos = playerClimbingSystem.CGManager.rayLedgeDownHit.point +  (transform.forward * 0 + transform.up * 0);
            float moveDuration = 0.1f;
            await MovePlayerToTarget(handDropPos, moveDuration);
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
    }
}