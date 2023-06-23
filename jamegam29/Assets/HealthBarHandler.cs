using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private Image healthBarFillImage;
    [SerializeField] private TextMeshProUGUI healthTitleText;

    public void UpdateHealthBar(int health)
    {
        healthTitleText.text = health.ToString();
        healthBarFillImage.fillAmount = health / 100f;
    }
}
