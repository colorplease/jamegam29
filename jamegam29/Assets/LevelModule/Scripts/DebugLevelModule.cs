using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class DebugLevelModule : MonoBehaviour
{
    [SerializeField] private GameEvent initializeCameraEvent;
    // Start is called before the first frame update
    void Start()
    {
        initializeCameraEvent.Raise();
    }
    
}
