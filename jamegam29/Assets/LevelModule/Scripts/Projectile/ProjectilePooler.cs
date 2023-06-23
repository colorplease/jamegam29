using System.Collections.Generic;
using UnityEngine;

namespace LevelModule.Scripts.Projectile
{
    public class ProjectilePooler : MonoBehaviour
    {
        public static ProjectilePooler Instance; // Singleton instance

        public GameObject musicalNotePrefab; // Prefab for musical note projectile
        public GameObject eggPrefab; // Prefab for egg projectile

        private Queue<GameObject> musicalNotePool;
        private Queue<GameObject> eggPool;

        public int poolSize = 10; // Initial size of the pool

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            musicalNotePool = new Queue<GameObject>();
            eggPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject musicalNoteObject = Instantiate(musicalNotePrefab);
                musicalNoteObject.SetActive(false);
                musicalNotePool.Enqueue(musicalNoteObject);

                GameObject eggObject = Instantiate(eggPrefab);
                eggObject.SetActive(false);
                eggPool.Enqueue(eggObject);
            }
        }

        public GameObject GetMusicalNote()
        {
            if (musicalNotePool.Count == 0)
            {
                GameObject newMusicalNote = Instantiate(musicalNotePrefab);
                newMusicalNote.SetActive(false);
                musicalNotePool.Enqueue(newMusicalNote);
            }

            GameObject musicalNote = musicalNotePool.Dequeue();
           // musicalNote.SetActive(true);
            return musicalNote;
        }

        public GameObject GetEgg()
        {
            if (eggPool.Count == 0)
            {
                GameObject newEgg = Instantiate(eggPrefab);
                newEgg.SetActive(false);
                eggPool.Enqueue(newEgg);
            }

            GameObject egg = eggPool.Dequeue();
           // egg.SetActive(true);
            return egg;
        }

        public void ReturnToPool(GameObject projectile, EnemyProjectile.ProjectileType projectileType)
        {
            projectile.SetActive(false);

            if (projectileType == EnemyProjectile.ProjectileType.MusicalNote)
            {
                musicalNotePool.Enqueue(projectile);
            }
            else if (projectileType == EnemyProjectile.ProjectileType.Egg)
            {
                eggPool.Enqueue(projectile);
            }
        }
    }
}