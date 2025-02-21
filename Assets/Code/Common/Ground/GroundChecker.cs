
using UnityEngine;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
       [SerializeField] LayerMask _groundLayer;
        [SerializeField] float _groundDistance = 0.1f; // Small offset to detect ground properly

        public bool IsGround { get; private set; }

        private void Update() 
        {
            IsOnGround();
        }

        private void IsOnGround()
        {
            IsGround = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out _, _groundDistance, _groundLayer);
        }
    }
}