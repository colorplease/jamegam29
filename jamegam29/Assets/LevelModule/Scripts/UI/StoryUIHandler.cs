using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelModule.Scripts.UI
{
    public class StoryUIHandler : MonoBehaviour
    {
        [SerializeField] private CanvasGroup pageCanvasGroup;
        [SerializeField] private Button nextButton;
        [SerializeField] private Image pageImageRef;
        [SerializeField] private TextMeshProUGUI pageText;
        [SerializeField] private List<PageData> pageData;
    
        private CanvasGroup activePage;
    
        private int storyIndex;
        private int numberOfPages;
    
        // Start is called before the first frame update
        private void Start()
        {
            numberOfPages = pageData.Count;
            nextButton.interactable = false;
            ShowPage();
        }
    
        private void ShowPage()
        {
            pageImageRef.sprite = pageData[storyIndex].pageImage;
            pageCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
            {
                StartCoroutine(PlayText());
            });
        }
    
        private IEnumerator PlayText()
        {
            foreach (char c in pageData[storyIndex].pageText) 
            {
                pageText.text += c;
                yield return new WaitForSeconds (0.03f);
            }

            nextButton.interactable = true;
        }

        public void ButtonEvt_Next()
        {
            storyIndex++;
            pageText.text = "";
            
            nextButton.interactable = false;
        
            if(storyIndex == numberOfPages)
                gameObject.SetActive(false);
        
            pageCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.Flash).OnComplete(ShowPage);
        }
    }
}
