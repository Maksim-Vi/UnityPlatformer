
using UnityEngine;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float _groundDistance = 0.01f;
        [SerializeField] LayerMask _groundLayer;

        public bool IsGround { get; private set;}

        private void Update() 
        {
            IsGround = Physics.SphereCast(transform.position, _groundDistance, Vector3.down, out _, _groundDistance, _groundLayer);
        }
    }
}