using UnityEngine;

namespace Platformer
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] int maxHealth = 100;
        [SerializeField] FloatEventChannel playerHealthChannel;

        int currentHealth;

        public bool IsDead => currentHealth <= 0;

        private void Awake() 
        {
            currentHealth = maxHealth;
        }

        private void Start() 
        {
            PublishHealthPercentage();
        }

        public void GetHealth(int health)
        {
            if(playerHealthChannel != null && currentHealth < 100){
                currentHealth += health;

                if(currentHealth > maxHealth)
                    currentHealth = 100;
                
                playerHealthChannel.Invoke(currentHealth / (float)maxHealth);
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            PublishHealthPercentage();
        }

        void PublishHealthPercentage()
        {
            if(playerHealthChannel != null){
                playerHealthChannel.Invoke(currentHealth / (float)maxHealth);
            }
        }
    }
}