using System;
using System.Collections;
using UnityEngine;

namespace Platformer
{

    public enum ClambPlayerState {
        Normal, Clambing
    }

    public class PlayerClimbingSystem : MonoBehaviour 
    {
        [HideInInspector] public PlayerController pController => playerController;
        private PlayerController playerController;

        [Header("Info view")]
        public ClambPlayerState clambPlayerState = ClambPlayerState.Normal;
        public bool isClimbing;
        public bool canGrabLedge = false;
        [SerializeField] public LayerMask ledgeLayer;

        [Header("References climbing grap settings")]
        [SerializeField] public int rayAmount = 10;
        [SerializeField] public float rayLength = 0.5f;
        [SerializeField] public float rayOffset = 0.15f;
        [SerializeField] public float rayHight = 0.5f;
        public float rayYHandCorrection = 0.1f;
        public float rayZHandCorrection = 0.1f;
        
        [Header("References climbing move settings")]

        public RaycastHit rayLedgeForwardHit;
        public RaycastHit rayLedgeDownHit;

        private ClimbingGrabManager _climbingGrabManager;

        private bool hasMatchedTarget;

        void Start() {
            clambPlayerState = ClambPlayerState.Normal;

            _climbingGrabManager = new ClimbingGrabManager(this, rayAmount, rayLength, rayOffset, rayHight, rayYHandCorrection, rayZHandCorrection);
        }

        private void OnDisable() {
            playerController.input.Climb -= OnClimb;
        }

        public void Init(PlayerController playerController) {
            this.playerController = playerController;
            playerController.input.Climb += OnClimb;
        }

        private void Update() {
            _climbingGrabManager.OnUpdate();
            
            CheckingRay();
            StateConditionCheck();
        }

        private void CheckingRay() {
            if (!isClimbing && playerController.groundChecker.IsGround) {
                _climbingGrabManager.CheckOnGroundRay();
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