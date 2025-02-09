using UnityEngine;
using Utilits;

namespace Platformer
{
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        readonly float detectedAngle = 60f;
        readonly float detectedRadius = 5f;
        readonly float innerDetectedRadius = 3f;

        public ConeDetectionStrategy(float detectedAngle, float detectedRadius, float innerDetectedRadius)
        {
            this.detectedAngle = detectedAngle;
            this.detectedRadius = detectedRadius;
            this.innerDetectedRadius = innerDetectedRadius;
        }

        public bool Execute(Transform player, Transform detector, CountdownTimer timer)
        {
            if(timer.IsRunning) return false;

            var directionToPlayer = player.position - detector.position;
            var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);

            if((!(angleToPlayer < detectedAngle / 2f) || !(directionToPlayer.magnitude < detectedRadius))
            && !(directionToPlayer.magnitude < innerDetectedRadius))
                return false;
            
            timer.Start();
            return true;
        }

        public bool DetectAttackZone(Transform player, Transform detector)
        {
            var directionToPlayer = player.position - detector.position;
            float angleToPlayer = Vector3.Angle(detector.forward, directionToPlayer);

            if(angleToPlayer < detectedAngle / 2f && directionToPlayer.magnitude < innerDetectedRadius)
                return true;

            return false;
        }
    }
}
