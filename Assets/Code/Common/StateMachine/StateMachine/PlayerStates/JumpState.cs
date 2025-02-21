using System.Threading.Tasks;
using UnityEngine;

namespace Platformer
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController playerController, Animator animator) : base(playerController, animator)
        {
        }

        public override void OnEnter()
        {
            _animator.CrossFade(JumpHash, crossFadeDuration);
        }

        public override void OnFixedUpdate()
        {
            _playerController.HandleJump();
            _playerController.HandleMovement();
        }
    }
}