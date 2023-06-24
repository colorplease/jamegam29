using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelModule.Scripts.Projectile
{
    public class EnemyProjectile : MonoBehaviour
    {
        public enum ProjectileType
        {
            MusicalNote,
            Egg
        }
        
        public float speed = 5f; // Speed of the projectile
        public float lifetime = 5f; // Lifetime of the projectile
        public float gravity = -9.8f; // Gravity to apply to the projectile
        public float damageRadius;
        
        private ProjectileType currentProjectileType;
        private Rigidbody2D rb;
  
        private Vector2 direction; // Direction of the projectile
        
        private bool isInitialized;
        private float verticalSpeed = 5f; // Vertical speed for gravity simulation

        private int projectileDamage;
        
        private void Start()
        {
           
            // Start the DestroyAfterLifetime coroutine
            StartCoroutine(DestroyAfterLifetime());
        }

        public void InitializeProjectile(Vector2 shootDirection, int damage,ProjectileType projectileType)
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            
            direction = shootDirection;
            currentProjectileType = projectileType;
            projectileDamage = damage;

            if (projectileType == ProjectileType.Egg)
            {
                // Define a min and max strength
                float minStrength = 0.5f;
                float maxStrength = 1.5f;

                // Generate a random strength
                float randomStrength = Random.Range(minStrength, maxStrength);

                // Apply the random strength to your initial force
                Vector2 initialForce = new Vector2(direction.x * speed * randomStrength, verticalSpeed * randomStrength);

                rb.AddForce(initialForce, ForceMode2D.Impulse);
                
                // Define a min and max spin
                float minSpin = -360f;  // Negative for counterclockwise spin
                float maxSpin = 360f;   // Positive for clockwise spin

                // Generate a random spin
                float randomSpin = Random.Range(minSpin, maxSpin);

                // Apply the random spin
                rb.angularVelocity = randomSpin;
            }
            isInitialized = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isInitialized)
                return;

            switch (currentProjectileType)
            {
                case ProjectileType.MusicalNote:
                    MoveProjectileForward();
                    break;
                case ProjectileType.Egg:
                    //LobProjectile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            DetectCollisions();
          
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("Other collided with " + other.transform.name);
        }

        private void DetectCollisions()
        {
            // Get all colliders within the attack radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);

            // For each collider...
            foreach (Collider2D hit in hits)
            {
                // ...check if it's the player...
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<PlayerHealthHandler>().TakeDamage(projectileDamage);
                    isInitialized = false;
                    ProjectilePooler.Instance.ReturnToPool(gameObject,currentProjectileType);
                }
            }
        }

        private void MoveProjectileForward()
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }

        private void LobProjectile()
        {
            // Apply gravity
            verticalSpeed += gravity * Time.deltaTime;

            // Move horizontally
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        
            // Move vertically
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
        }
        
        // Coroutine that waits for the lifetime of the projectile, then destroys it
        private IEnumerator DestroyAfterLifetime()
        {
            yield return new WaitForSeconds(lifetime);
            
            if (gameObject.activeInHierarchy)
            {
                isInitialized = false;
                ProjectilePooler.Instance.ReturnToPool(gameObject,currentProjectileType);
            }
        }
    }
}