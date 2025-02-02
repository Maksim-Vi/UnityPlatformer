using UnityEngine;

namespace Platformer
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController _playerController;
        protected readonly Animator _animator;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int MovementHash = Animator.StringToHash("RunIdle");
        protected static readonly int JumpHash = Animator.StringToHash("`JumpIdle");

        protected const float crossFadeDuration = 0.1f;

        protected BaseState(PlayerController playerController, Animator animator)
        {
            _playerController = playerController;
            _animator = animator;
        }

        public virtual void OnEnter()
        {
            //noop
        }

        public virtual void OnExit()
        {
            //noop
        }

        public virtual void OnFixedUpdate()
        {
            //noop
        }

        public virtual void OnUpdate()
        {
            //noop
        }
    }
}