namespace Utilits
{
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) {}
        public override void Tick(float deltaTime)
        {
            if(IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }
            
            if(IsRunning && Time <= 0)
            {
                Stop();
            }
        }

        public bool IsFinish => Time <= 0;
        public void Reset() => Time = initailTime;
        public void Reset(float newTime) {
            initailTime = newTime;
            Reset();
        }
    }
}