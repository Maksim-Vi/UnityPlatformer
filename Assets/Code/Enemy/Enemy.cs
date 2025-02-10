
using KBCore.Refs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Utilits;

namespace Platformer
{

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {
        [SerializeField, Self] PlayerDetector playerDetector;
        [SerializeField, Self] NavMeshAgent agent;
        [SerializeField, Child] Animator animtior;
        [SerializeField] float wanderRadius = 3f;
        [SerializeField] float timeBetweenAttacks = 1f;

        CountdownTimer attackTimer;

        StateMachine stateMachine;

        private void OnValidate() {
            this.ValidateRefs();
        }

        private void Start() {
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animtior, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animtior, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animtior, agent, playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicateBase(()=> playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicateBase(()=> !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicateBase(()=> playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicateBase(()=> !playerDetector.CanAttackPlayer()));
           
            stateMachine.SetState(wanderState);
        }
    
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void Update() 
        {
            attackTimer.Tick(Time.deltaTime);
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if(attackTimer.IsRunning) return;

            attackTimer.Start();

            Debug.Log("ATTAKING");
            
            playerDetector.PlayerHealth.TakeDamage(5);
        }
    }
}
