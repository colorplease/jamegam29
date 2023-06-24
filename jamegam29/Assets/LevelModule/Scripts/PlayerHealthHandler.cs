using GameEvents;
using UnityEngine;

namespace LevelModule.Scripts
{
    public class PlayerHealthHandler : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100; // The maximum health of the GameObject
        [SerializeField] private PlayerHealthBarHandler playerHealthBarHandler;
        [SerializeField] private GameEvent playerDeathEvent;
        
        private int currentHealth; // The current health of the GameObject
        private bool _isAlive;

        private void Start()
        {
            currentHealth = maxHealth; // Start with full health
            _isAlive = true;
        }

        // Reduce the health of the GameObject
        public void TakeDamage(int damage)
        {
            if (!_isAlive)
                return;
            
            currentHealth -= damage;

            // If the health drops to 0 or below, trigger death
            if (currentHealth <= 0)
            {
                playerHealthBarHandler.UpdateHealthBar(0);
                Die();
            }
            else
            {
                playerHealthBarHandler.UpdateHealthBar(currentHealth);
            }
        }

        //TODO:: Add player death logic
        private void Die()
        {
            Debug.Log("Player has died");
            _isAlive = false;
            playerDeathEvent.Raise();
            gameObject.SetActive(false);
        }

        // Getter for current health
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}