using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Platformer
{

    public enum ClambPlayerState {
        Normal, Clambing
    }

    public class PlayerClimbingSystem : MonoBehaviour 
    {
        [HideInInspector] public PlayerController pController => playerController;
        [HideInInspector] public ClimbingGrabManager CGManager => _climbingGrabManager;
        [HideInInspector] public LayerMask ledgeLayer => _ledgeLayer;
        private PlayerController playerController;

        [Header("Info view")]
        public ClambPlayerState clambPlayerState = ClambPlayerState.Normal;
        public bool isClimbing;
        public bool canGrabLedge = false;
        [SerializeField] public LayerMask _ledgeLayer;

        [Header("References climbing grap settings")]
        [SerializeField] public int rayAmount = 10;
        [SerializeField] public float rayLength = 0.5f;
        [SerializeField] public float rayOffset = 0.15f;
        [SerializeField] public float rayHight = 0.5f;
        [SerializeField] public float rayYHandCorrection = 0.1f;
        [SerializeField] public float rayZHandCorrection = 0.1f;
        
        [Header("References climbing move settings")]
        [SerializeField] public int rayHopAmount = 10;
        [SerializeField] public float rayHopLength = 0.5f;
        [SerializeField] public float rayHopOffset = 0.15f;
        [SerializeField] public float rayHopHight = 0.5f;

        private ClimbingGrabManager _climbingGrabManager;
        private ClimbingMoveManager _climbingMoveManager;

        void Start() {
            clambPlayerState = ClambPlayerState.Normal;

            _climbingGrabManager = new ClimbingGrabManager(this, rayAmount, rayLength, rayOffset, rayHight, rayYHandCorrection, rayZHandCorrection);
            _climbingMoveManager = new ClimbingMoveManager(this, rayHopAmount, rayHopLength, rayHopOffset, rayHopHight);
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
            _climbingMoveManager.OnUpdate();
            
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
                if (canGrabLedge && _climbingGrabManager.rayLedgeDownHit.point != Vector3.zero) {
                    Quaternion lookRot = Quaternion.LookRotation(-_climbingGrabManager.rayLedgeForwardHit.normal);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime);

                    GrabLedge();
                }
            } else {
                ThrowLedge();
            }
        }

        void GrabLedge() { 
            isClimbing = true;
            _climbingGrabManager.hasMatchedTarget = false;
            clambPlayerState = ClambPlayerState.Clambing;
            playerController.animatior.CrossFade("Idle To Braced Hang", 0.08f);
        } 

        async void ThrowLedge() { 
            isClimbing = false;
            _climbingGrabManager.hasMatchedTarget = false;
            playerController.animatior.CrossFade("Braced Hang Drop Ground", 0.1f);

            await UniTask.Delay(500);
            clambPlayerState = ClambPlayerState.Normal;
        }

        public void HopUp()
        {
            playerController.animatior.CrossFade("Braced Hang Hop Up", 0.2f);
        }

        public void HopDown()
        {
            playerController.animatior.CrossFade("Braced Hang Drop", 0.2f);
        }

        // void OnDrawGizmos() {
        //     if (_climbingGrabManager.rayLedgeDownHit.point != Vector3.zero) {
        //         Gizmos.color = Color.yellow;
        //         Gizmos.DrawSphere(_climbingGrabManager.rayLedgeDownHit.point, 0.05f);
        //     }
        // }
    }
}