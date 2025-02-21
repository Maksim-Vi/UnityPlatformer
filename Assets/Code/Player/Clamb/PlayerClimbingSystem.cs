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
        private PlayerController playerController;

        [Header("Info view")]
        public ClambPlayerState clambPlayerState = ClambPlayerState.Normal;
        public bool isClimbing;
        public bool canGrabLedge = false;
        [Space(5)] public GameObject climnObj;

        [Header("References climbing settings")]
        [SerializeField] public int rayAmount = 10;
        [SerializeField] public float rayLangth = 0.5f;
        [SerializeField] public float rayOffset = 0.15f;
        [SerializeField] public float rayHight = 0.5f;
       [SerializeField] public LayerMask ledgeLayer;

        RaycastHit rayLedgeHit;


        void Start()
        {
            clambPlayerState = ClambPlayerState.Normal;
        }

        private void OnDisable() 
        {
            playerController.input.Climb -= OnClimb;
        }


        public void Init(PlayerController playerController)
        {
            this.playerController = playerController;
            playerController.input.Climb += OnClimb;
        }

        private void Update() 
        {
            CheckingRay();
            StateConditionCheck();
        }

        private void CheckingRay()
        {
            if(!isClimbing && playerController.groundChecker.IsGround)
            {
                CheckOnGroundRay();
            }
        }

        private void StateConditionCheck()
        {
            switch (clambPlayerState)
            {
                case ClambPlayerState.Normal:
                    if(playerController.rb.isKinematic)
                        playerController.rb.isKinematic = false;

                    playerController.animatior.applyRootMotion = false;
                    break;
                case ClambPlayerState.Clambing:
                    if(!playerController.rb.isKinematic)
                        playerController.rb.isKinematic = true;

                    
                    playerController.animatior.applyRootMotion = true;
                    break;
                default:
                    break;
            }
        }

        private void OnClimb()
        {
            if(!isClimbing)
            {
                if(canGrabLedge)
                    StartCoroutine(GrabLedge());
            }
            else 
            {
                StartCoroutine(ThrowLedge());
            }
        }

        private void CheckOnGroundRay()
        {
            for (int i = 0; i < rayAmount; i++)
            {
                Vector3 rayPosition = transform.position + Vector3.up * rayHight + Vector3.up * rayOffset * i;
            
                Debug.DrawRay(rayPosition, transform.forward, Color.cyan);

                if(Physics.Raycast(rayPosition, transform.forward, out rayLedgeHit, rayLangth, ledgeLayer))
                {
                    climnObj = rayLedgeHit.transform.gameObject;
                    canGrabLedge = true;
                    break;
                }

                canGrabLedge = false;
            }
        }

        IEnumerator GrabLedge()
        { 
            isClimbing = true;
            clambPlayerState = ClambPlayerState.Clambing;
            // set animation from idle to clamb

            yield return new WaitForSeconds(1);
        } 
        
        IEnumerator ThrowLedge()
        { 
            isClimbing = false;
           // set animation from clamb to idle
            yield return new WaitForSeconds(1);
            clambPlayerState = ClambPlayerState.Normal;
        }
    }
}