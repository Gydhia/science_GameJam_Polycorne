﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [ExecuteAlways]
    public class CardSpace : MonoBehaviour, IDropHandler, IPointerDownHandler
    {
        public Card Card;
        public Box Box;

        public Vector2Int positionInBox;
        public string CopyFromBox;
        public float OverlapWeight = 1;

        [Range(0, 1)]
        public float Padding = 0;
        //public SpriteRenderer SpriteRenderer;

        public Color ColorEmpty = Color.green;
        public Color ColorUsed = Color.blue;

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("test2");
        }


        public void Start()
        {
            if (this.Card != null)
            {
                this.Card.transform.position = this.transform.position;
            }
        }

        public void OnValidate()
        {
            if (!Application.isPlaying)
            {
                float pixerperunit = Board.Instance.UICanvas.referencePixelsPerUnit;
                float canvascardwidth = 1 - Padding;
                float canvascardhlength = 1 - Padding;

                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvascardhlength, canvascardwidth);
                //this.SpriteRenderer.size = new Vector2(canvascardhlength, canvascardwidth);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(this.CopyFromBox) && this.Box != Board.Instance.StartStation && this.Box != Board.Instance.EndStation)
            {
                Debug.Log(eventData);
                if (eventData.pointerDrag != null)
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position;
                    Card card = eventData.pointerDrag.GetComponent<Card>();
                    this.Card = card;
                    this.Card.CardSpace = this;
                    // replicate the card where it is supposed to be
                    if (!string.IsNullOrEmpty(this.Box.CardNameForPlayer))
                    {
                        var other_cardspace = FindObjectsOfType<CardSpace>().Where(cs => cs.CopyFromBox == this.Box.CardNameForPlayer);
                        foreach (var carspace_to_copy in other_cardspace)
                        {
                            var cardcopy = GameObject.Instantiate(card, card.transform.parent);
                            cardcopy.transform.position = carspace_to_copy.transform.position;
                            carspace_to_copy.Card = cardcopy;
                            cardcopy.CardSpace = carspace_to_copy;
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

    }
}
