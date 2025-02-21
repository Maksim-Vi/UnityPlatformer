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
            if(_playerController.currentSpeed == 0){
                _animator.CrossFade(JumpOnPlaceHash, 0f);
            } else {
                _animator.CrossFade(JumpHash, 0.1f);
            }
        }

        public override void OnFixedUpdate()
        {
            _playerController.HandleJump();
            _playerController.HandleMovement();
        }
    }
}