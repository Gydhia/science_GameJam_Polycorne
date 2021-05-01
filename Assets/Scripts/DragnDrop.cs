using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragnDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    public RectTransform rootRectTransform;
    //private CanvasGroup canvasGroup;
    private RectTransform canvasRectTransform;
    private Vector3 originDrag;
    private Collider2D _collider;
    [SerializeField]
    private Card _card;
    //public EventTrigger EventTrigger;

    public bool DragEnabled = true;

    private void Start()
    {
        //rootRectTransform = this.GetComponentInParent<RectTransform>();
        //canvasGroup = this.GetComponent<CanvasGroup>();
        this.canvas = Board.Instance.BoardCanvas;
        this._collider = this.gameObject.GetComponent<Collider2D>();
        this._card = this.gameObject.transform.parent.GetComponent<Card>();
        //EventTrigger = GameObject.FindObjectOfType<EventTrigger>();
        if (this.canvas != null)
            this.canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("test");
    }

    public void OnCancelDrag(PointerEventData eventData)
    {
        eventData.pointerDrag = null;
        // canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;
        this.rootRectTransform.position = originDrag;

        this._collider.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.DragEnabled == false)
            return;

        //when drag is ongoing, disable the collider, if not the drop is not detected
        this._collider.enabled = false;

        // canvasGroup.alpha = 0.6f;
        //canvasGroup.blocksRaycasts = false;
        originDrag = this.transform.position;
            
        if (this._card != null)
        {
            if (this._card.CardSpace != null)
            {
                if(SoundController.Instance != null)
                    SoundController.Instance.PlaySound(SoundController.SoundNames.DragCard);
                // this cardspace is readonly
                if (!string.IsNullOrEmpty(this._card.CardSpace.CopyFromBox))
                {
                    OnCancelDrag(eventData);
                    return;
                }
                if (!string.IsNullOrEmpty(this._card.CardSpace.Box.CardNameForPlayer))
                {
                    var other_cardspace = FindObjectsOfType<CardSpace>().Where(cs => cs.CopyFromBox == this._card.CardSpace.Box.CardNameForPlayer);
                    foreach (var carspace_to_copy in other_cardspace)
                    {
                        carspace_to_copy.Card.RegisterCardSpace(null);
                        GameObject.Destroy(carspace_to_copy.Card.gameObject);
                    }
                }
                this._card.RegisterCardSpace(null);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.DragEnabled == false)
            return;
        //Vector2 delta = ScreenToCanvas(eventData.position) - ScreenToCanvas(eventData.position - eventData.delta);
        //rectTransform.anchoredPosition += delta * canvas.scaleFactor;
        this.rootRectTransform.anchoredPosition += eventData.delta / this.canvas.scaleFactor;
        //this.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.DragEnabled == false)
            return;
        eventData.pointerDrag = null;
        // canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;
        this._collider.enabled = true;
    }



    /// <summary>
    /// Return the anchored position of the root element clamped inside the canvas position
    /// </summary>
    /// <returns>The anchored clamped position</returns>
    private Vector3 LimitRectBoundsToCanvas()
    {
        Vector3 localPosition = Vector3.zero;
        Vector2 rectPosition = this.rootRectTransform.anchoredPosition;

        if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || (this.canvas.renderMode == RenderMode.ScreenSpaceCamera && this.canvas.worldCamera == null))
        {
            // keep panel inside layout
            localPosition.x = Mathf.Clamp(rectPosition.x, Vector2.zero.x, this.canvasRectTransform.rect.width - this.rootRectTransform.rect.width);
            localPosition.y = Mathf.Clamp(rectPosition.y, -this.canvasRectTransform.rect.height + this.rootRectTransform.rect.height, Vector2.zero.y);
        }

        return localPosition;
    }
    public void PlayHoverSound()
    {
        if(SoundController.Instance != null)
            SoundController.Instance.PlaySound(SoundController.SoundNames.HoverCard);
    }
}
