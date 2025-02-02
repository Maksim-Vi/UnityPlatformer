using DG.Tweening;
using UnityEngine;

namespace Platformer
{
    public class SpawnEffect : MonoBehaviour
    {
        [SerializeField] GameObject SpawnVFX;
        [SerializeField] float _animationDuration = 1f;

        private void Start() 
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _animationDuration).SetEase(Ease.OutBack);

            if(SpawnVFX != null){
                Instantiate(SpawnVFX, transform.position, Quaternion.identity);
            }

            // play audio
        }
    }
}