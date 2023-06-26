using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        public int coinsCounter = 0;

        public GameObject playerGameObject;
        private PlayerUnityController _playerUnity;
        public GameObject deathPlayerPrefab;
        public Text coinText;

        void Start()
        {
            _playerUnity = GameObject.Find("Player").GetComponent<PlayerUnityController>();
        }

        void Update()
        {
            if(coinText != null)
                coinText.text = coinsCounter.ToString();
            
            if(_playerUnity.deathState == true)
            {
                playerGameObject.SetActive(false);
                GameObject deathPlayer = (GameObject)Instantiate(deathPlayerPrefab, playerGameObject.transform.position, playerGameObject.transform.rotation);
                deathPlayer.transform.localScale = new Vector3(playerGameObject.transform.localScale.x, playerGameObject.transform.localScale.y, playerGameObject.transform.localScale.z);
                _playerUnity.deathState = false;
                Invoke("ReloadLevel", 3);
            }
        }

        private void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
