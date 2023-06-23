using System.Collections;
using DG.Tweening;
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

        public EnemyData enemyData;
        public GameObject player; // Reference to the player
        public Transform projectileSpawnPosition;
        
        private bool _isCoolingDown;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {
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
            
            if (_isCoolingDown)
                return;
            
            // Spits music notes at you
            // This could be more complex depending on your projectile system
            var direction =  _spriteRenderer.flipX ? -transform.right : transform.right;

            GameObject note = ProjectilePooler.Instance.GetMusicalNote();
            note.transform.position = projectileSpawnPosition.position;
            note.SetActive(true);
            
            note.GetComponent<EnemyProjectile>().InitializeProjectile(direction, enemyData.rangeAttackDamage,EnemyProjectile.ProjectileType.MusicalNote);

            StartCoroutine(Co_Cooldown());
        }

        private void AttackGoldenGoose()
        {
            DetectCollisions();
            
            if (_isCoolingDown)
                return;
            
            // Lobs golden eggs at the player
            var direction =  _spriteRenderer.flipX ? -transform.right : transform.right;
          
            GameObject egg = ProjectilePooler.Instance.GetEgg();
            egg.transform.position = projectileSpawnPosition.position;
            egg.SetActive(true);
            
            egg.GetComponent<EnemyProjectile>().InitializeProjectile(direction, enemyData.rangeAttackDamage,EnemyProjectile.ProjectileType.Egg);
            
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
                    // Knock the player back
                    Vector3 knockBackDirection = (hit.transform.position - transform.position).normalized;
                    Vector3 knockBackTarget = hit.transform.position + knockBackDirection * enemyData.knockBackDistance;
                    hit.transform.DOMove(knockBackTarget, enemyData.knockBackDuration);
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

        private void OnDrawGizmos()
        {
            if (enemyData == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.meleeAttackRadius);
        }
    }
}
