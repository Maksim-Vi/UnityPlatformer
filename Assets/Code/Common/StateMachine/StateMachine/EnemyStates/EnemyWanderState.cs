using System;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyWanderState : EnemyBaseState
    {
        readonly NavMeshAgent agent;
        readonly Vector3 startPoint;
        readonly float wanderRadius;

        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
        {
            this.agent = agent;
            this.startPoint = enemy.transform.position;
            this.wanderRadius = wanderRadius;
        }

        override public void OnEnter()
        {
            animator.CrossFade(WalkHash, crossFadeDuration);
        }

        override public void OnUpdate()
        {
            // search closer position if agent dont have HasReachedDestination
            if(HasReachedDestination())
            {
                var randomDictance = UnityEngine.Random.insideUnitSphere * wanderRadius;
                randomDictance += startPoint;
                NavMeshHit hit;

                NavMesh.SamplePosition(randomDictance, out hit, wanderRadius, 1); 
                var finalPosition = hit.position;
                agent.SetDestination(finalPosition); 
            }
        }

        private bool HasReachedDestination()
        {
            return !agent.pathPending 
                && agent.remainingDistance <= agent.stoppingDistance 
                && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}