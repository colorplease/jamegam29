using System.Collections;
using DG.Tweening;
using GameEvents;
using UnityEngine;

namespace LevelModule.Scripts
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] ScreenShake screenShake;
        
        [Tooltip("Camera size when zooming in after transition")]
        [SerializeField] private float[] zoomLevelData;
        [SerializeField] private GameEvent levelTransitionEvent;
        [SerializeField] private GameEvent levelTransitionFinishedEvent;
        [SerializeField] private float minHeight;
        [SerializeField] private Transform target; // Player's Transform
       // [SerializeField] private List<Transform> levelStartTransforms; // Transform to indicate where each level begins
     
        [SerializeField] private Vector3 offset; // Offset from the target (player)
        [SerializeField] private float smoothSpeed = 0.125f; // Speed at which the camera follows the player
        [SerializeField] private float zoomOutSize = 10f; // Camera size when zooming out for transition
        //[SerializeField] private float zoomInSize = 5f; // Camera size when zooming in after transition

        [SerializeField] private PlayerController playerController;

        // These are the sizes and speed for zooming.
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float transitionDelay = 0.25f;
        
        private int currentLevelIndex;
        private bool inTransition = false;

       
        
        private Camera camera;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void Start()
        {
            currentLevelIndex = 0;
        }
        
        private void Update()
        {
            if (inTransition)
                return;

            // Check if player is out view and a level exists below us
            if (IsPlayerOutOfYView())
            {
                // Handle player out of view situation
                inTransition = true;
                playerController.FreezePlayer(true);

                Debug.Log("Player out of view");
                StartCoroutine(LevelTransition());
            }
        }

        private bool IsPlayerOutOfYView()
        {
            Vector3 playerViewportPosition = camera.WorldToViewportPoint(target.position);

            return playerViewportPosition.y < minHeight;
        }

        private IEnumerator LevelTransition()
        {
            //screenShake.RecordCurrentScreenPos();
            
            levelTransitionEvent.Raise();
            
            var newLevelPosition = _levelManager.GetCurrentLevelPosition();
            Debug.Log("Transition Started " + newLevelPosition);

            Vector3 newPosition = newLevelPosition + offset;
            newPosition.z = transform.position.z;

            float t = 0f;
            Vector3 startCameraPosition = transform.position;
            float startOrthoSize = camera.orthographicSize;

            // Move the camera and zoom out
            while (t < 1)
            {
                t += Time.deltaTime * smoothSpeed;
                transform.position = Vector3.Lerp(startCameraPosition, newPosition, t);
                camera.orthographicSize = Mathf.Lerp(startOrthoSize, zoomOutSize, t);
                yield return null;
            }

           

            t = 0f;
            startOrthoSize = camera.orthographicSize;

            // Zoom back in
            while (t < 1)
            {
                t += Time.deltaTime * zoomSpeed;
                camera.orthographicSize = Mathf.Lerp(startOrthoSize, zoomLevelData[currentLevelIndex], t);
                yield return null;
            }

            Debug.Log("Transition Finished");
            currentLevelIndex++;
            playerController.FreezePlayer(false);
            levelTransitionFinishedEvent.Raise();
            inTransition = false;
        }
    }
}