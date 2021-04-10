using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [ExecuteAlways]
    public class CardSpace : MonoBehaviour, IDropHandler
    {
        public Card Card;
        public Box Box;

        public Vector2Int positionInBox;
        public string CopyFromBox;

        [Range(0, 1)]
        public float Padding = 0;
        //public SpriteRenderer SpriteRenderer;

        public Color ColorEmpty = Color.green;
        public Color ColorUsed = Color.blue;


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
            if (string.IsNullOrEmpty(this.CopyFromBox))
            {
                Debug.Log(eventData);
                if (eventData.pointerDrag != null)
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().position = this.GetComponent<RectTransform>().position;
                    Card card = eventData.pointerDrag.GetComponent<Card>();
                    this.Card = card;
                    this.Card.CardSpace = this;
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
            }
            else
            {
                // can't move here, this is locked.
            }
            eventData.Reset();
        }

    }
}
