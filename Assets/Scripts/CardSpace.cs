﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class CardSpace : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public Card Card;
    public Box Box;

    public Vector2Int positionInBox;
    public string CopyFromBox;
    public float OverlapWeight = 1;

    [Range(0, 100)]
    public float Padding = 0;
    public SpriteRenderer SpriteRenderer;

    public Color ColorEmpty = Color.green;
    public Color ColorUsed = Color.blue;

    public bool DropEnabled = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }


    public void Start()
    {
        if (this.Card != null)
        {
            this.Card.transform.position = this.transform.position;
        }
        this.DropEnabled = true;
    }

#if UNITY_EDITOR
    public void OnValidate()
    {

        /*if (!Application.isPlaying)
        {
            //float pixerperunit = Board.Instance.UICanvas.referencePixelsPerUnit;
            float canvascardwidth = 100 - Padding;
            float canvascardhlength = 100 - Padding;

            //this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvascardhlength, canvascardwidth);
            this.SpriteRenderer.size = new Vector2(canvascardhlength, canvascardwidth);
        }*/
    }
#endif

    public void OnDrop(PointerEventData eventData)
    {
        if (this.DropEnabled == false)
            return;

        if (string.IsNullOrEmpty(this.CopyFromBox) && this.Box != Board.Instance.StartStation && !Board.Instance.IsEndStation(this.Box))
        {
            Debug.Log(eventData);
            if (eventData.pointerDrag != null)
            {
                if(SoundController.Instance != null)
                    SoundController.Instance.PlaySound(SoundController.SoundNames.MergeCard);
                eventData.pointerDrag.GetComponent<DragnDrop>().rootRectTransform.position = this.GetComponent<RectTransform>().position;
                Card card = eventData.pointerDrag.GetComponent<DragnDrop>().rootRectTransform.gameObject.GetComponent<Card>();
                card.RegisterCardSpace(this);
                // replicate the card where it is supposed to be
                if (!string.IsNullOrEmpty(this.Box.CardNameForPlayer))
                {
                    var other_cardspace = FindObjectsOfType<CardSpace>().Where(cs => cs.CopyFromBox == this.Box.CardNameForPlayer);
                    foreach (var carspace_to_copy in other_cardspace)
                    {
                        var cardcopy = GameObject.Instantiate(card, Board.Instance.CardDeckContainer.transform);
                        cardcopy.transform.position = carspace_to_copy.transform.position;
                        cardcopy.RegisterCardSpace(carspace_to_copy);
                    }
                }
            }
            else // nothing here
                eventData.pointerDrag.GetComponent<DragnDrop>().OnCancelDrag(eventData);
        }
        else // This box is READONLY
        {
            eventData.pointerDrag.GetComponent<DragnDrop>().OnCancelDrag(eventData);
        }
    }

    public void RegisterCard(Card card)
    {
        this.Card = card;
    }

    public void UnregisterCard()
    {
        this.Card = null;
    }
}
