using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameEvents;
using LevelModule.Scripts.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField]float durationJump;
    [SerializeField]float jumpPower;
    [SerializeField]int enemyToSpawn;

    private GameObject playerObjRef;

    private EnemyController _activeController;
    private EnemyController.EnemyType activeEnemyType;
    
    // Start is called before the first frame update

    private void Awake()
    {
        playerObjRef = GameObject.FindWithTag("Player");
    }
    
    public void SpawnSelectedEnemy()
    {
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

    private void SpawnEnemy(EnemyController.EnemyType enemyType)
    {
        // Get the camera's bounds
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        float leftBound = camera.transform.position.x - halfWidth - 2;
        float rightBound = camera.transform.position.x + halfWidth + 2;
        float lowerBound = camera.transform.position.y - halfHeight - 2;
        float upperBound = camera.transform.position.y + halfHeight + 0.1f;

        // Generate a random spawn position
        float spawnX = Random.value < 0.5 ? Random.Range(leftBound, camera.transform.position.x - halfWidth) : Random.Range(camera.transform.position.x + halfWidth, rightBound);
        float spawnY = Random.value < 0.5 ? Random.Range(lowerBound, camera.transform.position.y - halfHeight) : Random.Range(camera.transform.position.y + halfHeight, upperBound);
        
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        var enemyController = enemy.GetComponent<EnemyController>();
        _activeController = enemyController;
        activeEnemyType = enemyType;
        
        enemy.transform.DOJump(spawnPoints[0].position, jumpPower, 1, durationJump).SetEase(Ease.Flash).OnComplete(() =>
        {
            //enemyController.InitializeEnemy(enemyType);
            _activeController.InitializeEnemy(activeEnemyType);
            // Flip the enemy sprite to face the player
            SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                bool playerIsToLeft = playerObjRef.transform.position.x < enemy.transform.position.x;
                spriteRenderer.flipX = playerIsToLeft;
            }
        });
    }

    public void InitializeEnemy()
    {
        
    }
    
  
}
