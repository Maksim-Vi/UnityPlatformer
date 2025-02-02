
using System;

namespace Utilits
{
    public abstract class Timer
    {
        protected float initailTime;
        protected float Time {get; set;}
        public bool IsRunning {get; set;}

        public float Progress => Time / initailTime;
        public Action OnTimerStart = delegate {};
        public Action OnTimerStop = delegate {};

        protected Timer(float value)
        {
            initailTime = value;
            IsRunning = false;
        }

        public void Start() 
        {
            Time = initailTime;

            if(!IsRunning){
                IsRunning = true;
                OnTimerStart?.Invoke();
            }
        }
        
        public void Stop() 
        {
            if(IsRunning){
                IsRunning = false;
                OnTimerStop?.Invoke();
            }
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public abstract void Tick(float deltaTime);
    }
}