using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using GameEvents;
using UnityEngine;

namespace LevelModule.Scripts
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private float minHeight;
       [SerializeField] private Transform target; // Player's Transform
       [SerializeField] private List<Transform> levelStartTransforms; // Transform to indicate where each level begins
       [SerializeField] private Vector3 offset; // Offset from the target (player)
       [SerializeField] private float smoothSpeed = 0.125f; // Speed at which the camera follows the player
       [SerializeField] private float zoomOutSize = 10f; // Camera size when zooming out for transition
       [SerializeField] private float zoomInSize = 5f; // Camera size when zooming in after transition

        [SerializeField] private PlayerController playerController;
        // These are the sizes and speed for zooming.
        [SerializeField] private float  zoomSpeed = 2f;
        [SerializeField] private float transitionDelay = 0.25f;

        private Camera camera; // Reference to the Camera component
        private int currentLevelIndex;
        private bool inTransition = false;

        private void Start()
        {
            camera = GetComponent<Camera>();
            currentLevelIndex = 0;
        }

        private void Update()
        {
            Vector3 playerViewportPosition = Camera.main.WorldToViewportPoint(target.position);
            
            Debug.Log("ViewPoint Position: " + playerViewportPosition.y );
            
            if (inTransition)
                return;
            
            // Check if player is out view and a level exists below us
            if (IsPlayerOutOfYView() && currentLevelIndex < levelStartTransforms.Count)
            {
                // Handle player out of view situation
                inTransition = true;
                playerController.FreezePlayer(true);
              

                LevelTransition(levelStartTransforms[currentLevelIndex].position);
            }
        }
        
        private bool IsPlayerOutOfYView()
        {
            Vector3 playerViewportPosition = Camera.main.WorldToViewportPoint(target.position);
            
            Debug.Log("ViewPoint Position: " + playerViewportPosition.y );

            // Check if player's y position is outside of the viewport
            // playerViewportPosition.y will be between 0 and 1 if the player is inside the vertical bounds of the camera
            if (playerViewportPosition.y < minHeight|| playerViewportPosition.y > 1)
            {
                return true;
            }

            return false;
        }
        
        private IEnumerator Co_StartCameraTransition()
        {
            yield return new WaitForSeconds(transitionDelay);
           
           
        }
        
        
        private void LevelTransition(Vector3 newLevelPosition)
        {
            // Transition consists of two parts: zooming out and moving to new position, then zooming back in

            // Create a new Vector3 with new x and y values, but keep the original z value
            Vector3 newPosition = newLevelPosition + offset;
            newPosition.z = transform.position.z; // Keep the original z value

            // Start the sequence
            Sequence sequence = DOTween.Sequence();

            // Add a Tween to the sequence that moves the camera to the new position and zoom out at the same time
            sequence.Append(transform.DOMove(newPosition, 1 / smoothSpeed).SetEase(Ease.InOutSine));
            sequence.Join(camera.DOOrthoSize(zoomOutSize, 1 / zoomSpeed).SetEase(Ease.InOutSine));

            // Add a Tween to the sequence that zooms back in
            sequence.Append(camera.DOOrthoSize(zoomInSize, 1 / zoomSpeed).SetEase(Ease.InOutSine));

            // Handle what happens after the transition
            sequence.OnComplete(() =>
            {
                Debug.Log("Transition Finished");
                inTransition = false;
                currentLevelIndex++;
                playerController.FreezePlayer(false);
            });
        }
        
    }
}