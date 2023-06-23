using LevelModule.Scripts.Enemy;
using UnityEngine;

namespace LevelModule.Scripts
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Data", order = 0)]
    public class EnemyData : ScriptableObject
    {
        public EnemyController.EnemyType enemyType;
        
        [Tooltip("Prefab for enemy if its ranged base")]
        public GameObject projectilePrefab; // 
        
        [Tooltip("damage dealt by range attacks")]
        public int rangeAttackDamage = 1; // damage dealt by attack
        
        [Tooltip("Cooldown for range abilities")]
        public float rangeAttackCooldown = 1f; // Cooldown for range abilities
        
        [Tooltip("damage dealt by melee attacks")]
        public int meleeAttackDamage = 1; // damage dealt by attack
        
        [Tooltip("radius of attack if melee based")]
        public float meleeAttackRadius = 1f; // radius of attack if melee based

        public float knockBackDistance = 1f;
        public float knockBackDuration = 0.25f;
        public float moveSpeed = 1f;
    }
}