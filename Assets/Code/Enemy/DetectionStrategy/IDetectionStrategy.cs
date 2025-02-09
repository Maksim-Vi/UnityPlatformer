using UnityEngine;
using Utilits;

namespace Platformer
{
    public interface IDetectionStrategy
    {
        bool Execute(Transform player, Transform detector, CountdownTimer timer);
        bool DetectAttackZone(Transform player, Transform detector);
    }
}
