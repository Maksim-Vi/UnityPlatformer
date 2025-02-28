using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
        [SerializeField] public LayerMask ledgeLayer;

        [Header("References climbing grap settings")]
        [SerializeField] public int rayAmount = 10;
        [SerializeField] public float rayLength = 0.5f;
        [SerializeField] public float rayOffset = 0.15f;
        [SerializeField] public float rayHight = 0.5f;
        
        [Space(5)]
        [SerializeField] public float rayYHandCorrection = 0.1f;
        [SerializeField] public float rayZHandCorrection = 0.1f;
        
        [Header("References climbing move settings")]
        [SerializeField] public int rayHopAmount = 10;
        [SerializeField] public float rayHopLength = 0.5f;
        [SerializeField] public float rayHopOffset = 0.15f;
        [SerializeField] public float rayHopHight = 0.5f;
        [SerializeField] public float rayOffsetDown = 0.5f;
       
        [Space(5)] 
        [SerializeField] public float frowardHopPos = 0.1f;
        [SerializeField] public float upHopPos = 0.1f;

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

        Vector2 verticalInp;
        public void OnMove(Vector2 vector)
        {
            verticalInp = vector;
        }

        private void Update() {
            CheckingRay();
            StateConditionCheck();

            HopUpDown();
            CheckOnGroundRay();
        }

        RaycastHit hopLedgeDownHit;
        private void HopUpDown()
        {
            if(isClimbing)
            {
                if (verticalInp.y < -0.1f)
                {
                    HopDownRayCheck();
                }
                else if (verticalInp.y > 0.1f)
                {
                    HopUpRayCheck();
                }
            }
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

        private void OnClimb() {
            if (!isClimbing) {
                if (canGrabLedge && rayLedgeDownHit.point != Vector3.zero) {
                    Quaternion lookRot = Quaternion.LookRotation(-rayLedgeForwardHit.normal);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime);

                    GrabLedge();
                }
            } else {
                if(verticalInp == Vector2.zero) ThrowLedge();
            }
        }

        public RaycastHit rayLedgeForwardHit;
        public RaycastHit rayLedgeDownHit;
        public void CheckOnGroundRay()
        {
            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 rayPosition = transform.position + Vector3.up * rayHight + Vector3.up * rayOffset * i;

                Debug.DrawRay(rayPosition, transform.forward, Color.red);
                if (Physics.Raycast(rayPosition, transform.forward, out rayLedgeForwardHit, rayLength, ledgeLayer, QueryTriggerInteraction.Ignore))
                {
                    canGrabLedge = true;

                    Debug.DrawRay(rayLedgeForwardHit.point + Vector3.up * 0.5f, Vector3.down * 0.7f, Color.blue);
                    Physics.Raycast(rayLedgeForwardHit.point + Vector3.up * 0.5f, Vector3.down, out rayLedgeDownHit, 0.7f, ledgeLayer);
                    return;
                }
                else
                {
                    canGrabLedge = false;
                }
            }
        }

        private void HopUpRayCheck()
        {
            for (int i = 0; i < rayAmount; i++)
            {
            Vector3 rayPosition = transform.position + (Vector3.up * rayHopHight + Vector3.up * rayOffsetDown + Vector3.up * rayHopOffset * i);
                Debug.DrawRay(rayPosition, transform.forward, Color.green);

                if (Physics.Raycast(rayPosition, transform.forward, out rayLedgeForwardHit, rayHopLength, ledgeLayer, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawRay(rayLedgeForwardHit.point + Vector3.up * 0.5f, Vector3.down, Color.green);

                    if (Physics.Raycast(rayLedgeForwardHit.point + Vector3.up * 0.5f, Vector3.down, out hopLedgeDownHit, 1f, ledgeLayer))
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            HopUp();
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
                Vector3 rayPosition = transform.position + (Vector3.up * rayHopHight - Vector3.up * rayOffsetDown - Vector3.up * rayHopOffset * i);
                Debug.DrawRay(rayPosition, transform.forward, Color.green);

                if (Physics.Raycast(rayPosition, transform.forward, out rayLedgeForwardHit, rayHopLength, ledgeLayer, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawRay(rayLedgeForwardHit.point + Vector3.up * 0.5f, Vector3.down, Color.green);

                    if (Physics.Raycast(rayLedgeForwardHit.point + Vector3.up * 0.5f, Vector3.down, out hopLedgeDownHit, 1f, ledgeLayer))
                    {
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            HopDown();
                        }
                    }
                    
                    break;
                }
            }
        }

        void GrabLedge() { 
            isClimbing = true;
            clambPlayerState = ClambPlayerState.Clambing;
            playerController.animatior.CrossFade("Idle To Braced Hang", 0f);
        } 

        public void GrabLedgeTarget()
        { 
            Vector3 handPos = rayLedgeDownHit.point + transform.forward * rayZHandCorrection + transform.up * rayYHandCorrection;
            playerController.animatior.MatchTarget(handPos, transform.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.33f, 0.45f);
        }

        public void ThrowLedge() { 
            clambPlayerState = ClambPlayerState.Normal;
            playerController.animatior.CrossFade("Braced Hang Drop Ground", 0.05f);
        }

        public async void ThrowLedgeTarget()
        {
            await UniTask.Delay(200);

            playerController.animatior.CrossFade("Idle_Move", 0.1f);
            isClimbing = false;
        }

        public void HopUp()
        {
            playerController.animatior.CrossFade("Braced Hang Hop Up", 0.2f);
        }

        public void HopUpTarget()
        {
            Vector3 handDropPos = hopLedgeDownHit.point + transform.forward * frowardHopPos + transform.up * upHopPos;
            playerController.animatior.MatchTarget(handDropPos, transform.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.40f, 0.60f);
        }

        public void HopDown()
        {
            playerController.animatior.CrossFade("Braced Hang Drop", 0.2f);
        }

        public void HopDownTarget()
        {
            Vector3 handDropPos = hopLedgeDownHit.point + transform.forward * frowardHopPos + transform.up * upHopPos;
            playerController.animatior.MatchTarget(handDropPos, transform.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.25f, 0.40f);
        }

        void OnDrawGizmos() {
            if (hopLedgeDownHit.point != Vector3.zero) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hopLedgeDownHit.point, 0.05f);
            }
        }
    }
}