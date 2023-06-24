using UnityEngine;

namespace LevelModule.Scripts.AbilityData
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Data", order = 0)]
    public class AbilityData : ScriptableObject
    {
        [Tooltip("How long it takes for the ability to cooldown ")]
        public float abilityCooldown;

        public int abilityCharges;
    }
}