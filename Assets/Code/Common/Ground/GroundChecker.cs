
using UnityEngine;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
       [SerializeField] LayerMask _groundLayer;
        [SerializeField] float _groundDistance = 0.1f; // Small offset to detect ground properly
        [SerializeField] float _groundDistanceDetect = 0.5f; // Small offset to detect ground properly

        public bool IsGround { get; private set; }

        private void Update() 
        {
            IsOnGround();
        }

        private void IsOnGround()
        {
            // IsGround = Physics.SphereCast(transform.position, _groundDistanceDetect, Vector3.down, out _, _groundDistance, _groundLayer);
            IsGround = Physics.Raycast(transform.position, Vector3.down, _groundDistance, _groundLayer);
        }
    }
}