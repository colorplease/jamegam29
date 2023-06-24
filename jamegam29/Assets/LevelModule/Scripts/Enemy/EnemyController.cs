using System;
using System.Collections;
using DG.Tweening;
using GameEvents;
using LevelModule.Scripts.Projectile;
using UnityEngine;

namespace LevelModule.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public enum EnemyType
        {
            Harp,
            SoyFish,
            GoldenGoose,
            BeanstalkTendrils,
            
        }

        public float rotationSpeed;
        public EnemyData harpData;
        public EnemyData goldenGooseData;
        public EnemyData soyFishData;
        public EnemyData beanStalkData;
        
        private EnemyData enemyData;
        
        public Transform projectileSpawnPosition;

        public GameEvent OnLevelTransitionEvent;
        public GameEvent OnLevelTransitionFinishedEvent;
        
        private EnemyHealthHandler enemyHealthHandler;
        private SpriteRenderer _spriteRenderer;
        private LevelTransitionEventHandler _levelTransitionEventHandler;
        private LevelTransitionFinishedEventHandler _levelTransitionFinishedEventHandler;
        
        private GameObject player; // Reference to the player
        private bool _isCoolingDown;
        private bool isFrozen;
       

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            enemyHealthHandler = GetComponent<EnemyHealthHandler>();
            _levelTransitionEventHandler = new LevelTransitionEventHandler(this);
            _levelTransitionFinishedEventHandler = new LevelTransitionFinishedEventHandler(this);
            isFrozen = true;
        }

        private void OnEnable()
        {
            OnLevelTransitionEvent.RegisterListener(_levelTransitionEventHandler);
            OnLevelTransitionFinishedEvent.RegisterListener(_levelTransitionFinishedEventHandler);
        }

        private void OnDestroy()
        {
            OnLevelTransitionEvent.UnregisterListener(_levelTransitionEventHandler);
            OnLevelTransitionFinishedEvent.UnregisterListener(_levelTransitionFinishedEventHandler);
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        public void InitializeEnemy(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Harp:
                    enemyData = harpData;
                    break;
                case EnemyType.SoyFish:
                    enemyData = soyFishData;
                    break;
                case EnemyType.GoldenGoose:
                    enemyData = goldenGooseData;
                    break;
                case EnemyType.BeanstalkTendrils:
                    enemyData = beanStalkData;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }
            enemyHealthHandler.Initialize();
            isFrozen = false;
        }

        private void Update()
        {
            if (!enemyHealthHandler.IsAlive() || isFrozen)
                return;
            
            switch(enemyData.enemyType)
            {
                case EnemyType.Harp:
                    AttackHarp();
                    break;
                case EnemyType.SoyFish:
                    AttackSoyFish();
                    break;
                case EnemyType.GoldenGoose:
                    AttackGoldenGoose();
                    break;
                case EnemyType.BeanstalkTendrils:
                    AttackBeanstalkTendrils();
                    break;
            }
        }

        private void AttackHarp()
        {
            DetectCollisions();
            
            // Calculate the direction towards the player
            Vector2 rotationDirection = (player.transform.position - transform.position).normalized;
            Vector2 shootDirection = transform.right;
            
            
            // Flip the sprite if player is on the other side
            if (player.transform.position.x > transform.position.x)
            {
                _spriteRenderer.flipX = false;
                shootDirection = transform.right;
            }
            else if (player.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
                shootDirection = -transform.right;
            }
            
            // Get direction to player
            Vector2 directionToPlayer = player.transform.position - transform.position;

            // Get the target angle
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            
            // If sprite is initially flipped, we need to adjust our calculations
            if(_spriteRenderer.flipX)
            {
                targetAngle -= 180;
            }

            // Smoothly rotate towards the target
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * rotationSpeed);
        
            // Apply the rotation
            transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);

            // Check and handle sprite flip
            if (_spriteRenderer.flipX)
            {
                if ((targetAngle > -90 && targetAngle < 90) && !_spriteRenderer.flipY)
                {
                    _spriteRenderer.flipY = true;
                }
                else if ((targetAngle < -90 || targetAngle > 90) && _spriteRenderer.flipY)
                {
                    _spriteRenderer.flipY = false;
                }
            }
            else
            {
                if ((targetAngle > 90 || targetAngle < -90) && !_spriteRenderer.flipY)
                {
                    _spriteRenderer.flipY = true;
                }
                else if ((targetAngle < 90 && targetAngle > -90) && _spriteRenderer.flipY)
                {
                    _spriteRenderer.flipY = false;
                }
            }

            
            if (_isCoolingDown)
                return;
            
            GameObject note = ProjectilePooler.Instance.GetMusicalNote();
            note.transform.position = transform.position;
            note.SetActive(true);
            
            note.GetComponent<EnemyProjectile>().InitializeProjectile(shootDirection, enemyData.rangeAttackDamage,EnemyProjectile.ProjectileType.MusicalNote);

            StartCoroutine(Co_Cooldown());
        }

        private void AttackGoldenGoose()
        {
            DetectCollisions();
            
            Vector2 shootDirection = transform.right;
            // Flip the sprite if player is on the other side
            if (player.transform.position.x > transform.position.x)
            {
                _spriteRenderer.flipX = false;
                shootDirection = transform.right;
            }
            else if (player.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
                shootDirection = -transform.right;
            }
            
            if (_isCoolingDown)
                return;
            
          
            GameObject egg = ProjectilePooler.Instance.GetEgg();
            egg.transform.position = transform.position;
            egg.SetActive(true);
            
            egg.GetComponent<EnemyProjectile>().InitializeProjectile(shootDirection, enemyData.rangeAttackDamage,EnemyProjectile.ProjectileType.Egg);
            
            StartCoroutine(Co_Cooldown());
        }

      
        private void AttackBeanstalkTendrils()
        {
            DetectCollisions();
        }
        
        private void AttackSoyFish()
        {
            // Floats towards you
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * enemyData.moveSpeed * Time.deltaTime;
            
            DetectCollisions();
        }

        private void DetectCollisions()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, enemyData.meleeAttackRadius);

            // For each collider...
            foreach (Collider2D hit in hits)
            {
                // ...check if it's the player...
                if (hit.CompareTag("Player"))
                {
                    // ...and if so, deal damage to it
                    // // Knock the player back
                    // Vector3 knockBackDirection = (hit.transform.position - transform.position).normalized;
                    // Vector3 knockBackTarget = hit.transform.position + knockBackDirection * enemyData.knockBackDistance;
                    // hit.transform.DOMove(knockBackTarget, enemyData.knockBackDuration);
                    hit.GetComponent<PlayerHealthHandler>().TakeDamage(enemyData.meleeAttackDamage);
                }
            }
        }

        private IEnumerator Co_Cooldown()
        {
            _isCoolingDown = true;
            yield return new WaitForSeconds(enemyData.rangeAttackCooldown);
            _isCoolingDown = false;
        }

        private void FreezeEnemy(bool freeze)
        {
            isFrozen = freeze;
        }

        private void OnDrawGizmos()
        {
            if (enemyData == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.meleeAttackRadius);
        }
        
        private class LevelTransitionEventHandler : IGameEventListener
        {
            private readonly EnemyController _enemyController;

            public LevelTransitionEventHandler (EnemyController enemyController)
            {
                _enemyController = enemyController;
            }

            public void OnEventRaised()
            {
                _enemyController.FreezeEnemy(true);
            }
        }
        
        private class LevelTransitionFinishedEventHandler : IGameEventListener
        {
            private readonly EnemyController _enemyController;

            public LevelTransitionFinishedEventHandler(EnemyController enemyController)
            {
                _enemyController = enemyController;
            }

            public void OnEventRaised()
            {
                _enemyController.FreezeEnemy(false);
            }
        }
    }
}
