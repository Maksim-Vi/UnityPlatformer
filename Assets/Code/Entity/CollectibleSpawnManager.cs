
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilits;

namespace Platformer
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField] CollactableData[] collactableData;
        [SerializeField] GameObject spawnPlace;
        [SerializeField] int spawnInterval = 1;

        EntitySpawner<Collectible> spawner;

        CountdownTimer spawnTimer;
        int counter;

        protected override async void Awake() 
        {
            base.Awake();

            spawner = new EntitySpawner<Collectible>(
                new EntityFactory<Collectible>(collactableData, spawnPlace),
                spawnPointStrategy
            );

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                await UniTask.Delay(spawnInterval);
                Spawn();
            }
        }

        private void Start() {
            //spawnTimer.Start();
        }

        private void Update() {
            //spawnTimer.Tick(Time.deltaTime);
        }

        public override void Spawn()
        {
            spawner.Create();
        }
    }
}