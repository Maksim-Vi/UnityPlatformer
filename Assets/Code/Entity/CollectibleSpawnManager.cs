
using UnityEngine;
using Utilits;

namespace Platformer
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField] CollactableData[] collactableData;
        [SerializeField] float spawnInterval = 1f;

        EntitySpawner<Collectible> spawner;

        CountdownTimer spawnTimer;
        int counter;

        protected override void Awake() 
        {
            base.Awake();

            spawner = new EntitySpawner<Collectible>(
                new EntityFactory<Collectible>(collactableData),
                spawnPointStrategy
            );

            spawnTimer = new CountdownTimer(spawnInterval);

            spawnTimer.OnTimerStop += () =>{
                if(counter++ >= spawnPoints.Length)
                {
                    spawnTimer.Stop();
                    return;
                }
            };

            Spawn();
            counter++;
            spawnTimer.Start();
        }

        private void Start() {
            spawnTimer.Start();
        }

        private void Update() {
            spawnTimer.Tick(Time.deltaTime);
        }

        public override void Spawn()
        {
            spawner.Create();
        }
    }
}