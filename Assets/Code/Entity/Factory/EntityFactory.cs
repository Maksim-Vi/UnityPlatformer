using UnityEngine;

namespace Platformer
{
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity
    {
        EntityData[] data;
        GameObject spawnPlace;

        public EntityFactory(EntityData[] data, GameObject spawnPlace)
        {
            this.data = data;
            this.spawnPlace = spawnPlace;
        }

        public T Create(Transform spawnPoints)
        {
            EntityData entityData = data[Random.Range(0, data.Length)];
            GameObject instance = GameObject.Instantiate(entityData.prefab, spawnPoints.position, spawnPoints.rotation, spawnPlace.transform);
            
            return instance.GetComponent<T>();
        }
    }
}