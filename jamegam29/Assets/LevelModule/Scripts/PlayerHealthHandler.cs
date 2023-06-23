using UnityEngine;

namespace LevelModule.Scripts
{
    public class PlayerHealthHandler : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100; // The maximum health of the GameObject
        [SerializeField] private PlayerHealthBarHandler playerHealthBarHandler;
        
        private int currentHealth; // The current health of the GameObject

        private void Start()
        {
            currentHealth = maxHealth; // Start with full health
        }

        // Reduce the health of the GameObject
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            playerHealthBarHandler.UpdateHealthBar(currentHealth);

            // If the health drops to 0 or below, trigger death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        //TODO:: Add player death logic
        private void Die()
        {
            Debug.Log("Player has died");
        }

        // Getter for current health
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}