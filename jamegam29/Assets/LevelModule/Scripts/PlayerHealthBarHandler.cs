using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelModule.Scripts
{
    public class PlayerHealthBarHandler : MonoBehaviour
    {
        [SerializeField] private Image healthBarFillImage;
        [SerializeField] private TextMeshProUGUI healthTitleText;

        public void UpdateHealthBar(int health)
        {
            healthTitleText.text = health.ToString();
            StartCoroutine(LerpFillAmount(health / 100f, 0.3f)); // 0.5f is the duration for the lerp, change as needed
        }
        
        private IEnumerator LerpFillAmount(float targetFill, float duration)
        {
            float time = 0;
            float startFill = healthBarFillImage.fillAmount;

            while (time < duration)
            {
                time += Time.deltaTime;
                float normalizedTime = time / duration; // Calculate the normalized time (0 to 1)
        
                // Lerp the fill amount and apply to the health bar
                healthBarFillImage.fillAmount = Mathf.Lerp(startFill, targetFill, normalizedTime);
        
                yield return null; // Wait for the next frame
            }

            healthBarFillImage.fillAmount = targetFill; // Ensure the final value is accurate
        }
    }
}
