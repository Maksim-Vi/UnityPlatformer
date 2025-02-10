using System;
using UnityEngine;

namespace Platformer
{
    public class Collectible : Entity
    {
        [SerializeField] int scores = 1;
        [SerializeField] IntEventChannel scoreChannel;

        private void OnTriggerEnter(Collider other) 
        {
            if(other.CompareTag("Player")){
                Destroy(gameObject);
                scoreChannel.Invoke(scores);
            }
        }
    }
}