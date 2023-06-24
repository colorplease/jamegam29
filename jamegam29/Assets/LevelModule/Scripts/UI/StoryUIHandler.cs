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
        [SerializeField] private Image pageImageRef;
        [SerializeField] private TextMeshProUGUI pageText;
        [SerializeField] private List<PageData> pageData;
        [SerializeField] CanvasGroup nextButton;

        [SerializeField]float scrollSpeed;
    
        private CanvasGroup activePage;
    
        private int storyIndex;
        private int numberOfPages;
        bool finishedTyping;
    
        // Start is called before the first frame update
        private void Start()
        {
            numberOfPages = pageData.Count;
            ShowPage();
            finishedTyping = false;
        }
    
        private void ShowPage()
        {
            pageImageRef.sprite = pageData[storyIndex].pageImage;
            pageCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
            {
                if(!finishedTyping)
                {
                    StartCoroutine(PlayText());
                }
            });
        }
    
        private IEnumerator PlayText()
        {
            foreach (char c in pageData[storyIndex].pageText) 
            {
                pageText.text += c;
                yield return new WaitForSeconds (scrollSpeed);
            }

            finishedTyping = true;
        }

        public void ButtonEvt_Next()
        {
            if(finishedTyping)
            {
                storyIndex++;
            pageText.text = "";
            finishedTyping = false;
        
            if(storyIndex == numberOfPages)
                pageCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.Flash);
                nextButton.DOFade(0, 0.5f).SetEase(Ease.Flash);
        
            pageCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.Flash).OnComplete(ShowPage);
            }
            else
            {
                StopAllCoroutines();
                finishedTyping = true;
                pageText.SetText(pageData[storyIndex].pageText);
            }
        }
    }
}
