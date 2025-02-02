using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(fileName = "CollactableData", menuName = "Platformer/CollactableData", order = 0)]
    public class CollactableData : EntityData
    {
        public int scores;
    }
}