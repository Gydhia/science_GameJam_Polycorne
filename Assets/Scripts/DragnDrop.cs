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
        private RectTransform canvasRectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GameObject.FindObjectOfType<Board>().UICanvas;
            canvasRectTransform = this.gameObject.GetComponent<RectTransform>();
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
            //this.rectTransform.anchoredPosition += eventData.delta / this.canvas.scaleFactor;
            this.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            //this.rectTransform.anchoredPosition = LimitRectBoundsToCanvas();
        }



        /// <summary>
        /// Return the anchored position of the root element clamped inside the canvas position
        /// </summary>
        /// <returns>The anchored clamped position</returns>
        private Vector3 LimitRectBoundsToCanvas()
        {
            Vector3 localPosition = Vector3.zero;
            Vector2 rectPosition = this.rectTransform.anchoredPosition;

            if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || (this.canvas.renderMode == RenderMode.ScreenSpaceCamera && this.canvas.worldCamera == null))
            {
                // keep panel inside layout
                localPosition.x = Mathf.Clamp(rectPosition.x, Vector2.zero.x, this.canvasRectTransform.rect.width - this.rectTransform.rect.width);
                localPosition.y = Mathf.Clamp(rectPosition.y, -this.canvasRectTransform.rect.height + this.rectTransform.rect.height, Vector2.zero.y);
            }

            return localPosition;
        }
    }
}
