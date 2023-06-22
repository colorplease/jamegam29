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
        private void Start()
        {
            initializeCameraEvent.RegisterListener(this);
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

            cinemachineVirtualCamera.m_LookAt = player.transform;
            cinemachineVirtualCamera.Follow = player.transform;
        }
    }
}
