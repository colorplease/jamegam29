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
        [SerializeField] private GameObject mainMenuRef;
        [SerializeField] private Button nextButton;
        [SerializeField] private Image pageImageRef;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI pageText;
        [SerializeField] private List<PageData> pageData;
    
        private CanvasGroup activePage;
        
        private Vector2 originalPosition; // The original position of the image
        private float originalAlpha; // The original alpha of the image
    
        private int storyIndex;
        private int numberOfPages;
    
        // Start is called before the first frame update
        private void Start()
        {
            numberOfPages = pageData.Count;
            nextButton.interactable = false;
            
            // Store the original position and alpha of the image
            originalPosition = pageImageRef.rectTransform.anchoredPosition;
            originalAlpha = pageImageRef.color.a;

            ShowPage();
        }
    
        private void ShowPage()
        {
            pageImageRef.sprite = pageData[storyIndex].pageImage;
            
            // Create a sequence to combine multiple tweens
            Sequence sequence = DOTween.Sequence();
            
            // Add a tween to the sequence to fade the image in
            sequence.Join(_canvasGroup.DOFade(1, 1f));

            // Start the sequence
            sequence.Play().OnComplete(() =>
            {
                StartCoroutine(PlayText());
            });
            
        }
    
        private IEnumerator PlayText()
        {
            foreach (char c in pageData[storyIndex].pageText) 
            {
                pageText.text += c;
                yield return new WaitForSeconds (0.02f);
            }

            nextButton.interactable = true;
        }

        public void ButtonEvt_Next()
        {
            storyIndex++;
            pageText.text = "";
            
            nextButton.interactable = false;
            
            // Return the image to its original position and alpha
            pageImageRef.rectTransform.anchoredPosition = originalPosition;
            pageImageRef.color = new Color(pageImageRef.color.r, pageImageRef.color.g, pageImageRef.color.b, originalAlpha);


            if (storyIndex == numberOfPages)
            {
                gameObject.SetActive(false);
                mainMenuRef.SetActive(true);
                return;
            }

            _canvasGroup.alpha = 0;
            ShowPage();
        }
    }
}
