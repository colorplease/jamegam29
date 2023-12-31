﻿using System;
using System.Collections;
using GameEvents;
using LevelModule.Scripts.Enemy;
using UnityEngine;

namespace LevelModule.Scripts
{
    public class EnemyHealthHandler : MonoBehaviour, IDamageable
    {
        [SerializeField] private AudioSource enemyDeathAudioSource;
        
        [SerializeField] private AudioClip harpDeath;
        [SerializeField] private AudioClip soyFishDeath;
        [SerializeField] private AudioClip goldenGooseDeath;
        [SerializeField] private AudioClip beanStalkDeath;
        
        [SerializeField] private EnemyHealthBarHandler _enemyHealthBarHandler;
        [SerializeField] private GameEvent killConfirmedEvent;
        [SerializeField] private GameObject explosionVFX;
        [SerializeField] private SpriteRenderer enemySprite;
        
        public bool explodeOnDeath;
        public int maxHealth = 100; // The maximum health of the GameObject
        public float explosionRadius = 5f; // Radius of the explosion effect
        public int explosionDamage = 20; // Damage of the explosion effect
        private int currentHealth; // The current health of the GameObject

        private bool _isAlive;
        
        private void Start()
        {
            currentHealth = maxHealth; // Start with full health
        }
        
        public void TakeDamage(int damage)
        {
            if (!_isAlive)
                return;
            
            currentHealth -= damage;
            
            _enemyHealthBarHandler.UpdateHealthBar(currentHealth);

            

            // If the health drops to 0 or below, trigger death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            killConfirmedEvent.Raise();
            
            var enemyType = GetComponent<EnemyController>().GetEnemyType();

            switch (enemyType)
            {
                case EnemyController.EnemyType.Harp:
                    enemyDeathAudioSource.clip = harpDeath;
                    enemyDeathAudioSource.Play();
                    break;
                case EnemyController.EnemyType.SoyFish:
                    enemyDeathAudioSource.clip = soyFishDeath;
                    enemyDeathAudioSource.Play();
                    break;
                case EnemyController.EnemyType.GoldenGoose:
                    enemyDeathAudioSource.clip = goldenGooseDeath;
                    enemyDeathAudioSource.Play();
                    break;
                case EnemyController.EnemyType.BeanstalkTendrils:
                    enemyDeathAudioSource.clip = beanStalkDeath;
                    enemyDeathAudioSource.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Instantiate the explosion effect when this enemy dies
            if (explodeOnDeath)
            {
               
                // //Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                //
                // // Get all colliders within the explosion radius
                // Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                //
                // // For each collider...
                // foreach (Collider2D hit in colliders)
                // {
                //     // If the collider has the "Player" tag...
                //     if (hit.CompareTag("Player"))
                //     {
                //         // Cause the player to take damage
                //         hit.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                //     }
                // }

                StartCoroutine(Co_Explosion());
            }
            else
            {
                // Destroy this enemy
                StartCoroutine(Co_DeathDelay());
            }

            _isAlive = false;

        }

        private IEnumerator Co_Explosion()
        {
            enemySprite.enabled = false;
            explosionVFX.SetActive(true);
            _enemyHealthBarHandler.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }

        private IEnumerator Co_DeathDelay()
        {
            enemySprite.enabled = false;
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        public void Initialize()
        {
            _isAlive = true;
        }
    }
}