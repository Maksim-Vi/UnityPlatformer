using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Platformer
{

    public enum ClambPlayerState {
        Normal, Clambing
    }

    public class PlayerClimbingSystem : MonoBehaviour 
    {
        private PlayerController playerController;

        [Header("Info view")]
        public ClambPlayerState clambPlayerState = ClambPlayerState.Normal;
        public bool isClimbing;
        public bool canGrabLedge = false;

        [Header("References climbing settings")]
        [SerializeField] public int rayAmount = 10;
        [SerializeField] public float rayLength = 0.5f;
        [SerializeField] public float rayOffset = 0.15f;
        [SerializeField] public float rayHight = 0.5f;
        [SerializeField] public LayerMask ledgeLayer;
        public RaycastHit rayLedgeForwardHit;
        public RaycastHit rayLedgeDownHit;
        public float rayYHandCorrection = 0.1f;
        public float rayZHandCorrection = 0.1f;

        private bool hasMatchedTarget;

        void Start() {
            clambPlayerState = ClambPlayerState.Normal;
        }

        private void OnDisable() {
            playerController.input.Climb -= OnClimb;
        }

        public void Init(PlayerController playerController) {
            this.playerController = playerController;
            playerController.input.Climb += OnClimb;
        }

        private void Update() {
            CheckingRay();
            StateConditionCheck();
            MathTargetToLedge();
            // _ = MatchHandPositionAsync();
        }

        private void CheckingRay() {
            if (!isClimbing && playerController.groundChecker.IsGround) {
                CheckOnGroundRay();
            }
        }

        private void StateConditionCheck() {
            switch (clambPlayerState) {
                case ClambPlayerState.Normal:
                    playerController.rb.isKinematic = false;
                    playerController.animatior.applyRootMotion = false;
                    break;
                case ClambPlayerState.Clambing:
                    playerController.rb.isKinematic = true;
                    playerController.animatior.applyRootMotion = true;
                    break;
            }
        }

        private void MathTargetToLedge() {
            if (hasMatchedTarget) return;

            AnimatorStateInfo animState = playerController.animatior.GetCurrentAnimatorStateInfo(0);
            bool isCorrectState = animState.IsName("Idle To Braced Hang") && !playerController.animatior.IsInTransition(0);
            
            if (!isCorrectState) return; 

            Vector3 handPos = rayLedgeDownHit.point + (transform.forward * rayZHandCorrection + transform.up * rayYHandCorrection);
            float moveDuration = 0.1f;
            StartCoroutine(MovePlayerToTarget(handPos, moveDuration));

            hasMatchedTarget = true;
        }

        private IEnumerator MovePlayerToTarget(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
        }

        private void OnClimb() {
            if (!isClimbing) {
                if (canGrabLedge && rayLedgeDownHit.point != Vector3.zero) {
                    Quaternion lookRot = Quaternion.LookRotation(-rayLedgeForwardHit.normal);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime);

                    StartCoroutine(GrabLedge());
                }
            } else {
                StartCoroutine(ThrowLedge());
            }
        }

        private void CheckOnGroundRay() {
            if(!isClimbing && playerController.groundChecker.IsGround) 
            {
                for (int i = 0; i < rayAmount; i++)
                {
                    Vector3 rayPosition = transform.position + Vector3.up * rayHight + Vector3.up * rayOffset * i;

                    Debug.DrawRay(rayPosition, transform.forward, Color.red);

                    if (Physics.Raycast(rayPosition, transform.forward, out rayLedgeForwardHit, rayLength, ledgeLayer, QueryTriggerInteraction.Ignore))
                    {
                        canGrabLedge = true;

                        Debug.DrawRay(rayLedgeForwardHit.point + Vector3.up * 0.2f, Vector3.down * 0.5f, Color.blue);
                        if (Physics.Raycast(rayLedgeForwardHit.point + Vector3.up * 0.2f, Vector3.down, out rayLedgeDownHit, 0.5f, ledgeLayer)) {
                            Debug.Log("Влучили у: " + rayLedgeDownHit.collider.name + " на позиції " + rayLedgeDownHit.point);
                        } else {
                            Debug.Log("Raycast не влучив у ledgeLayer!");
                        }
                        return;
                    }
                    else
                    {
                        canGrabLedge = false;
                    }
                }
            }
        }

        IEnumerator GrabLedge() { 
            isClimbing = true;
            hasMatchedTarget = false;

            clambPlayerState = ClambPlayerState.Clambing;

            playerController.animatior.CrossFade("Idle To Braced Hang", 0.08f);

            yield return null;
        } 

        IEnumerator ThrowLedge() { 
            isClimbing = false;
            hasMatchedTarget = false;
            playerController.animatior.CrossFade("Braced Hang Drop Ground", 0.1f);
            yield return new WaitForSeconds(0.5f);
            clambPlayerState = ClambPlayerState.Normal;
        }

        void OnDrawGizmos() {
            if (rayLedgeDownHit.point != Vector3.zero) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(rayLedgeDownHit.point, 0.05f);
            }
        }
    }
}