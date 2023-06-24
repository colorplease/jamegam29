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
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy(EnemyController.EnemyType.Harp);
    }

    public void SpawnEnemy(EnemyController.EnemyType enemyType)
    {
        var enemy = Instantiate(enemyPrefab, offScreenSpawnPoint);
        enemy.transform.DOJump(spawnPoints[0].position, 2f, 1, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
        {
            enemy.GetComponent<EnemyController>().InitializeEnemy(enemyType);
        });
    }
}
