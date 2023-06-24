using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LevelModule.Scripts.Enemy;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField]float durationJump;
    [SerializeField]float jumpPower;
    [SerializeField]int enemyToSpawn;

    private GameObject playerObjRef;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerObjRef = GameObject.FindWithTag("Player");
        //1 tendril
        //2 harp
        //3 soyfish
        //4 goose
        switch(enemyToSpawn)
        {
            case 1:
            SpawnEnemy(EnemyController.EnemyType.BeanstalkTendrils);
            break;
            case 2:
            SpawnEnemy(EnemyController.EnemyType.Harp);
            break;
            case 3:
            SpawnEnemy(EnemyController.EnemyType.SoyFish);
            break;
            case 4:
            SpawnEnemy(EnemyController.EnemyType.GoldenGoose);
            break;
        }
    }

    public void SpawnEnemy(EnemyController.EnemyType enemyType)
    {
        // Get the camera's bounds
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        float leftBound = camera.transform.position.x - halfWidth - 1;
        float rightBound = camera.transform.position.x + halfWidth + 1;
        float lowerBound = camera.transform.position.y - halfHeight - 1;
        float upperBound = camera.transform.position.y + halfHeight + 1;

        // Generate a random spawn position
        float spawnX = Random.value < 0.5 ? Random.Range(leftBound, camera.transform.position.x - halfWidth) : Random.Range(camera.transform.position.x + halfWidth, rightBound);
        float spawnY = Random.value < 0.5 ? Random.Range(lowerBound, camera.transform.position.y - halfHeight) : Random.Range(camera.transform.position.y + halfHeight, upperBound);
        
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        enemy.transform.DOJump(spawnPoints[0].position, jumpPower, 1, durationJump).SetEase(Ease.Flash).OnComplete(() =>
        {
            var enemyController = enemy.GetComponent<EnemyController>();
            enemyController.InitializeEnemy(enemyType);

            // Flip the enemy sprite to face the player
            SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                bool playerIsToLeft = playerObjRef.transform.position.x < enemy.transform.position.x;
                spriteRenderer.flipX = playerIsToLeft;
            }
        });
    }
}
