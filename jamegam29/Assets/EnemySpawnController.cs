using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LevelModule.Scripts.Enemy;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private Transform offScreenSpawnPoint;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField]float durationJump;
    [SerializeField]float jumpPower;
    [SerializeField]int enemyToSpawn;
    
    // Start is called before the first frame update
    void Start()
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

    public void SpawnEnemy(EnemyController.EnemyType enemyType)
    {
        var enemy = Instantiate(enemyPrefab, offScreenSpawnPoint);
        enemy.transform.DOJump(spawnPoints[0].position, jumpPower, 1, durationJump).SetEase(Ease.Flash).OnComplete(() =>
        {
            enemy.GetComponent<EnemyController>().InitializeEnemy(enemyType);
        });
    }
}
