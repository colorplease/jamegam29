using System.Collections;
using GameEvents;
using UnityEngine;

namespace LevelModule.Scripts
{
    public class PlayerHealthHandler : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100; // The maximum health of the GameObject
        [SerializeField] private PlayerHealthBarHandler playerHealthBarHandler;
        [SerializeField] private GameEvent playerDeathEvent;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;

        [Header("iFrames")] 
        [SerializeField] private float iFramesDuration;

        [SerializeField] private int numberOfFlashes;
        private int currentHealth; // The current health of the GameObject
        private bool _isAlive;
        private bool isInvulnerable;

        private void Start()
        {
            currentHealth = maxHealth; // Start with full health
            _isAlive = true;
        }

        // Reduce the health of the GameObject
        public void TakeDamage(int damage)
        {
            if (!_isAlive || isInvulnerable)
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
                StartCoroutine(Invunerability());
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

        private IEnumerator Invunerability()
        {
            Physics.IgnoreLayerCollision(7,8, true);
            isInvulnerable = true;

            for (int i = 0; i < numberOfFlashes; i++)
            {
                playerSpriteRenderer.color = new Color(1,0,0,0.5f);
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 3));
                playerSpriteRenderer.color = Color.white;
                yield return new WaitForSeconds(1f);
            }

            isInvulnerable = false;
            Physics2D.IgnoreLayerCollision(7,8,true);
        }

        // Getter for current health
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}