using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Utilits;

namespace Platformer
{
    public class PlayerDetector : MonoBehaviour 
    {
        [Header("Move and Detect")]
        [SerializeField] float detectionAngle = 60f;
        [SerializeField] float detectionRadius = 5f;
        [SerializeField] float innerDetectionRadius = 3f;
        [SerializeField] float detectionCooldown = 1f;

        [Header("Attack")]
        [SerializeField] float attackRange = 1f;

        public HealthSystem PlayerHealth {get; private set;}
        public Transform Player {get; private set;}

        IDetectionStrategy detectionStrategy;

        CountdownTimer detectionTimer;

        private void Awake() {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealth = Player.GetComponent<HealthSystem>();
        }

        private void Start() 
        {
            detectionTimer = new CountdownTimer(detectionCooldown);
            detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
        }

        private void Update()
        {
            detectionTimer.Tick(Time.deltaTime);
        }

        public bool CanDetectPlayer()
        {
            return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
        }

        public bool CanAttackPlayer()
        {
            var directionToPlayer = Player.position - transform.position;

            return directionToPlayer.magnitude <= attackRange;
        }

        public void SetDetectionStrategy(IDetectionStrategy detectionStrategy)
        {
            this.detectionStrategy = detectionStrategy;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);

            Vector3 forwardConeD = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
            Vector3 backwardConeD = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;

            Gizmos.DrawLine(transform.position, transform.position + forwardConeD);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeD);
        }

        
    }
}
