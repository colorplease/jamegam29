using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHudHandler : MonoBehaviour, IGameEventListener
{
    [SerializeField] private GameEvent playerDeathEvent;
    [SerializeField] private GameObject gameOverView;
    [SerializeField] private Image transitionImage;
    [SerializeField] private float transitionDuration = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        playerDeathEvent.RegisterListener(this);
    }
    

    public void OnEventRaised()
    {
        if(gameOverView != null)
            gameOverView.SetActive(true);
    }

    public void ButtonEvt_Retry()
    {
        StartCoroutine(ReloadSceneCoroutine());
    }
    
   

    // How long the transition takes
  
    

    private IEnumerator ReloadSceneCoroutine()
    {
        gameOverView.SetActive(false);
        // Fade in (from transparent to solid black)
        for(float t = 0f; t < transitionDuration; t += Time.deltaTime)
        {
            transitionImage.color = Color.Lerp(Color.clear, Color.black, t / transitionDuration);
            yield return null;
        }

        // Make sure it's fully black
        transitionImage.color = Color.black;

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Wait a frame so the scene loads before we start the next transition
        yield return null;

        // Fade out (from solid black to transparent)
        for(float t = 0f; t < transitionDuration; t += Time.deltaTime)
        {
            transitionImage.color = Color.Lerp(Color.black, Color.clear, t / transitionDuration);
            yield return null;
        }

        // Make sure it's fully transparent
        transitionImage.color = Color.clear;
    }
}
