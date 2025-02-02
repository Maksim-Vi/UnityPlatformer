using UnityEngine;

namespace Platformer
{
    public class RunState : BaseState
    {
        public RunState(PlayerController playerController, Animator animator) : base(playerController, animator)
        {
        }

        public override void OnEnter()
        {
            //_animator.CrossFade(MovementHash, crossFadeDuration);
        }

        public override void OnFixedUpdate()
        {
            _playerController.HandleMovement();
        }
    }
}