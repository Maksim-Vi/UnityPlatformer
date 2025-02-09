using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyAttackState : EnemyBaseState
    {  
        readonly NavMeshAgent agent;
        readonly Transform player;
        public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            this.agent = agent;
            this.player= player;
        }

        override public void OnEnter()
        {
            animator.CrossFade(AttackHash, crossFadeDuration);
        }
        
        override public void OnUpdate()
        {
            agent.SetDestination(player.position);
            enemy.Attack();
        }
    }
}