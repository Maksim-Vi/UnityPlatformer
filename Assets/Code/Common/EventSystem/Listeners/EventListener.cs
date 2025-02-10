using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
    public class EventListener<T> : MonoBehaviour 
    {
        [SerializeField] EventChannel<T> evetChannel;
        [SerializeField] UnityEvent<T> unityEvent;

        private void Awake() 
        {
            evetChannel.Register(this);
        }

        private void OnDestroy() 
        {
            evetChannel.Degister(this);
        }

        public void Raise(T value)
        {
            unityEvent?.Invoke(value);
        }
    }

    public class EventListener : EventListener<Empty> {}
}