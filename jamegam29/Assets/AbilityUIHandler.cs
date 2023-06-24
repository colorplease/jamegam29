using LevelModule.Scripts.AbilityData;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIHandler : MonoBehaviour
{
    public enum AbilityType
    {
        Gun1Ability,
        Gun2Ability,
        Gun3Ability
    }

    [SerializeField] private AbilityData gun1Data;
    [SerializeField] private AbilityData gun2Data;
    [SerializeField] private AbilityData gun3Data;
    
    [SerializeField] private Image ability1FillIcon;
    [SerializeField] private Image ability2FillIcon;
    [SerializeField] private Image ability3FillIcon;
    
    public void UpdateAbilityIcon(float currentCooldown, PlayerController.GunType gunType)
    {
        switch (gunType)
        {
            case PlayerController.GunType.Gun1:
                ability1FillIcon.fillAmount = currentCooldown / gun1Data.abilityCooldown;
                break;
            case PlayerController.GunType.Gun2:
                ability2FillIcon.fillAmount = currentCooldown / gun2Data.abilityCooldown;
                break;
            case PlayerController.GunType.Gun3:
                ability3FillIcon.fillAmount = currentCooldown / gun3Data.abilityCooldown;
                break;
        }
    }

    public void ResetIcon(PlayerController.GunType gunType)
    {
        switch (gunType)
        {
            case PlayerController.GunType.Gun1:
                ability1FillIcon.fillAmount = 1;
                break;
            case PlayerController.GunType.Gun2:
                ability2FillIcon.fillAmount = 1;
                break;
            case PlayerController.GunType.Gun3:
                ability3FillIcon.fillAmount = 1;
                break;
        }
    }
}
