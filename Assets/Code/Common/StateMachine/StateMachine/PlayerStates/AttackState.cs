using UnityEngine;

namespace Platformer
{
    public class AttackState : BaseState
    {
        public AttackState(PlayerController playerController, Animator animator) : base(playerController, animator)
        {
        }

        public override void OnEnter()
        {
            //_animator.CrossFade(AttackHash, crossFadeDuration);
            _playerController.Attack();
        }
        
        public override void OnFixedUpdate()
        {
            _playerController.HandleMovement();
        }
    }
}