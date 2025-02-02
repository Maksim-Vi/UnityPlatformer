using UnityEngine;

namespace Platformer
{
    public abstract class EntitySpawnManager : MonoBehaviour
    {
        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Liner;
        [SerializeField] protected Transform[] spawnPoints;

        protected ISpawnPointStrategy spawnPointStrategy;

        protected enum SpawnPointStrategyType
        {
            Liner, Random
        }

        protected virtual void Awake() 
        {
            switch (spawnPointStrategyType)
            {
                case SpawnPointStrategyType.Liner:
                    spawnPointStrategy = new LinerSpawnPointStrategy(spawnPoints);
                    break;
                case SpawnPointStrategyType.Random:
                    spawnPointStrategy = new RandomSpawnPointStrategy(spawnPoints);
                    break;
                default:
                    break;
            }
        }

        public abstract void Spawn();
    }
}