using System;
using Cinemachine;
using GameEvents;
using UnityEngine;

namespace LevelModule.Scripts
{
    public class CameraHandler : MonoBehaviour, IGameEventListener
    {
        [SerializeField] private GameEvent initializeCameraEvent;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        // Start is called before the first frame update
        public Transform target; // Assign this to the player in the inspector
        public float smoothSpeed = 0.125f; // Smoothing factor
        public Vector3 offset; // Offset from the target (player)
        public float margin = 0.2f; // Margin inside which player can move freely, adjust to your needs

        void Update()
        {
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(target.position);

            // Check if the player is outside the viewport
            if (viewportPoint.x < margin || viewportPoint.x > 1 - margin || viewportPoint.y < margin || viewportPoint.y > 1 - margin)
            {
                Vector3 desiredPosition = target.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
                transform.position = smoothedPosition;
            }
        }

        private void OnDestroy()
        {
            initializeCameraEvent.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player == null)
                return;

            //cinemachineVirtualCamera.m_LookAt = player.transform;
           // cinemachineVirtualCamera.Follow = player.transform;
        }
    }
}
