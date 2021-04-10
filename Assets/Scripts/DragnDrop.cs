using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class DragnDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GameObject.FindObjectOfType<Board>().UICanvas;
            //canvasRectTransform = this.gameObject.GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Vector2 delta = ScreenToCanvas(eventData.position) - ScreenToCanvas(eventData.position - eventData.delta);
            //rectTransform.anchoredPosition += delta * canvas.scaleFactor;
            this.rectTransform.anchoredPosition += eventData.delta / this.canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
