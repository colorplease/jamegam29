using System.Collections;
using DG.Tweening;
using GameEvents;
using UnityEngine;
using UnityEngine.U2D;
using PixelPerfectCamera = UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

namespace LevelModule.Scripts
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] ScreenShake screenShake;
        [SerializeField] private PixelPerfectCamera pixelPerfectCamera;
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

        private int levelZoom;
      
        private bool inTransition = false;

       

        private bool hasReceivedZoomData;
       
        
        private Camera camera;
     
        private void Awake()
        {
            camera = Camera.main;
            
           
            
        }

        private void Start()
        {
            
           
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
            levelTransitionEvent.Raise();

            Vector3 newPosition = _levelManager.GetCurrentLevelPosition() + offset;
            newPosition.z = transform.position.z;

            float t = 0f;
            Vector3 startCameraPosition = transform.position;
            int startPPU = pixelPerfectCamera.assetsPPU;
            
            Debug.Log("Zoom out size " + levelZoom);

            // Move the camera and zoom out
            while (t < 1)
            {
                t += Time.deltaTime * smoothSpeed;
                transform.position = Vector3.Lerp(startCameraPosition, newPosition, t);
                pixelPerfectCamera.assetsPPU = (int)Mathf.Lerp(startPPU, levelZoom, t);
                yield return null;
            }

            Debug.Log("Transition Finished");
           
            playerController.FreezePlayer(false);
            levelTransitionFinishedEvent.Raise();
            inTransition = false;
        }

        public void UpdateLevelZoom(int zoom)
        {
            levelZoom = zoom;
        }
    }
}