using UnityEngine;
using UnityEngine.UI;

namespace LevelModule.Scripts.UI
{
    [CreateAssetMenu(fileName = "StoryPage", menuName = "UI/Storypage", order = 0)]
    public class PageData : ScriptableObject
    {
        public Sprite pageImage;
        public string pageText;
    }
}