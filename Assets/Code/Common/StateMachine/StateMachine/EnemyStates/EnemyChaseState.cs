using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyChaseState : EnemyBaseState
    { 
        readonly NavMeshAgent agent;
        readonly Transform player;
        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            this.agent = agent;
            this.player = player;
        }

        override public void OnEnter()
        {
            animator.CrossFade(RunHash, crossFadeDuration);

        }
        
        override public void OnUpdate()
        {
            agent.SetDestination(player.position);
        }
    }
}