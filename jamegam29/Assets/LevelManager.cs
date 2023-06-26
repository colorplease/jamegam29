using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using LevelModule.Scripts;
using LevelModule.Scripts.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;


public class LevelManager : MonoBehaviour
{
    [SerializeField] private CameraHandler _cameraHandler;
    [SerializeField] private bool runTutorial;
    [SerializeField] private List<GameObject> tutorialPrefabs;
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private int killsRequiredPerRoom;
    [SerializeField] private GameEvent enemyKilledEvent;
    [SerializeField] private GameEvent levelTransitionEvent;
    [SerializeField] private GameEvent levelCompletedEvent;

    private EnemyDeathEventHandler _enemyDeathEventHandler;
    private LevelTransitionEventHandler _levelTransitionEventHandler;
    
    private GameObject activeLevel;
    private int currentKillCount;

    private int tutorialCompletionIndex;

    private bool isFirstRun;
    private AudioSource _audioSource;
  

    private void Awake()
    {
        _enemyDeathEventHandler = new EnemyDeathEventHandler(this);
        _levelTransitionEventHandler = new LevelTransitionEventHandler(this);
        
        enemyKilledEvent.RegisterListener(_enemyDeathEventHandler);
        levelTransitionEvent.RegisterListener(_levelTransitionEventHandler);
        _audioSource = GetComponent<AudioSource>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (runTutorial)
        {
            CreateRoomTutorial();
        }
        else
        {
            CreateRoom();
        }
       
    }

    private void OnDestroy()
    {
        enemyKilledEvent.UnregisterListener(_enemyDeathEventHandler);
        levelTransitionEvent.UnregisterListener(_levelTransitionEventHandler);
    }

    private void IncrementKills()
    {
        currentKillCount++;

        if (currentKillCount == killsRequiredPerRoom)
        {
            currentKillCount = 0;
            levelCompletedEvent.Raise();
            SpriteRenderer barrer = GameObject.FindGameObjectWithTag("finish").GetComponent<SpriteRenderer>();
            barrer.enabled = false;

            if (runTutorial)
            {
                tutorialCompletionIndex++;

                if (tutorialCompletionIndex == tutorialPrefabs.Count)
                {
                    runTutorial = false;
                }
            }
        }
    }

    private void CreateRoomTutorial()
    {
        Vector3 newLevelPosition = Vector3.zero;
        
        // Load new level
        if (activeLevel != null)
        {
            newLevelPosition = activeLevel.transform.position;
            newLevelPosition.y += -11f;
            
            CleanupPreviousLevel();
        }
        
        killsRequiredPerRoom = 0;
        activeLevel = Instantiate(tutorialPrefabs[tutorialCompletionIndex], newLevelPosition, Quaternion.identity);;

        var enemies = activeLevel.GetComponentsInChildren<EnemySpawnController>();

        foreach (var enemy in enemies)
        {
            enemy.SpawnSelectedEnemy();
            killsRequiredPerRoom++;
        }
    }

    private void CreateRoom()
    {
        Vector3 newLevelPosition = Vector3.zero;
        
        // Load new level
        if (activeLevel != null)
        {
            newLevelPosition = activeLevel.transform.position;
            newLevelPosition.y += -11f;
            
            CleanupPreviousLevel();
        }

        killsRequiredPerRoom = 0;
        var randomIndex = Random.Range(0, roomPrefabs.Count);
        var level = Instantiate(roomPrefabs[randomIndex], newLevelPosition, Quaternion.identity);
        activeLevel = level;
        _cameraHandler.UpdateLevelZoom(   activeLevel.GetComponent<RoomData>().zoomAmount);
     

        var enemies = level.GetComponentsInChildren<EnemySpawnController>();

        foreach (var enemy in enemies)
        {
           enemy.SpawnSelectedEnemy();
           killsRequiredPerRoom++;
        }
    }

    public Vector3 GetCurrentLevelPosition()
    {
        if(activeLevel == null)
            return Vector3.zero;
        
        return activeLevel.transform.position;
    }

    public void PlayRoomCompleteJingle()
    {
        
    }

    private void CleanupPreviousLevel()
    {
        var enemies = FindObjectsOfType<EnemySpawnController>();
        
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        if (activeLevel != null)
        {
            Destroy(activeLevel);
        }
    }
    
    private class EnemyDeathEventHandler : IGameEventListener
    {
        private readonly LevelManager _levelManager;

        public EnemyDeathEventHandler(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public void OnEventRaised()
        {
           _levelManager.IncrementKills();
        }
    }

    private class LevelTransitionEventHandler : IGameEventListener
    {
        private readonly LevelManager _levelManager;

        public LevelTransitionEventHandler(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public void OnEventRaised()
        {
            if (_levelManager.runTutorial)
            {
                _levelManager.CreateRoomTutorial();
            }
            else
            {
                _levelManager.CreateRoom();
            }
           
        }
    }
    
}
