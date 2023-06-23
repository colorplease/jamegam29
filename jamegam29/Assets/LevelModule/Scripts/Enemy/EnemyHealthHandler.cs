using UnityEngine;

namespace LevelModule.Scripts
{
    public class EnemyHealthHandler : MonoBehaviour, IDamageable
    {
        public bool explodeOnDeath;
        public GameObject explosionPrefab; // Prefab for the explosion effect
        public float maxHealth = 100f; // The maximum health of the GameObject
        public float explosionRadius = 5f; // Radius of the explosion effect
        public int explosionDamage = 20; // Damage of the explosion effect
        private float currentHealth; // The current health of the GameObject
        
        private void Start()
        {
            currentHealth = maxHealth; // Start with full health
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            // If the health drops to 0 or below, trigger death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // Instantiate the explosion effect when this enemy dies
            if (explodeOnDeath)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

                // Get all colliders within the explosion radius
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

                // For each collider...
                foreach (Collider2D hit in colliders)
                {
                    // If the collider has the "Player" tag...
                    if (hit.CompareTag("Player"))
                    {
                        // Cause the player to take damage
                        hit.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                    }
                }
            }
        
            // Destroy this enemy
            Destroy(gameObject);
        }
    }
}