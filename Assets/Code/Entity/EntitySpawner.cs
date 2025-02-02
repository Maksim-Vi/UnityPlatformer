namespace Platformer
{
    public class EntitySpawner<T> where T : Entity
    {
        IEntityFactory<T> entityFactory;
        ISpawnPointStrategy spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy)
        {
            this.entityFactory = entityFactory;
            this.spawnPointStrategy = spawnPointStrategy;
        }

        public T Create()
        {
            return entityFactory.Create(spawnPointStrategy.NextSpawnPoint());
        }
    }
}