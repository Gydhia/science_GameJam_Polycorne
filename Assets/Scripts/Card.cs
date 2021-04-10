using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Card : TrainHandler
    {
        public CardSpace CardSpace;

        protected override void TrainArrived(Train Train, Hand Hand)
        {
            Hand exit = null;
            if (this.CardSpace != null)
            {
                if (Hand.LeftHand)
                {
                    if (this.CardSpace.positionInBox.x == 0)
                        exit = this.CardSpace.Box.HandsLeft[Hand.Index];
                    else
                    {
                        var leftCardSpace = this.CardSpace.Box.CardSpaces[this.CardSpace.positionInBox.x - 1, this.CardSpace.positionInBox.y];
                        if (leftCardSpace != null && leftCardSpace.Card != null)
                            exit = leftCardSpace.Card.HandsRight[Hand.Index];
                    }
                }
                else
                {
                    if (this.CardSpace.positionInBox.x == this.CardSpace.Box.CardSpaces.GetLength(0) - 1)
                        exit = this.CardSpace.Box.HandsRight[Hand.Index];
                    else
                    {
                        var rightCardSpace = this.CardSpace.Box.CardSpaces[this.CardSpace.positionInBox.x + 1, this.CardSpace.positionInBox.y];
                        if (rightCardSpace != null && rightCardSpace.Card != null)
                            exit = rightCardSpace.Card.HandsLeft[Hand.Index];
                    }
                }
            }
            if (exit != null)
                Train.PlaceOnHand(exit);
            else
            {
                // WE4RE LEAVING FROM A CARD WITHOUT A BOX; eXIT;
            }

        }

        public override void RegenerateHands()
        {
            if (this.HandsContainer == null)
                return;

            float pixerperunit = GameObject.FindObjectOfType<Board>().UICanvas.referencePixelsPerUnit;
            float canvascardheight = this.gameObject.GetComponent<RectTransform>().rect.height;
            float canvascardhlength = this.gameObject.GetComponent<RectTransform>().rect.height;

            foreach (Transform child in this.HandsContainer)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    GameObject.DestroyImmediate(child.gameObject);

                };
            }

            int handcount = GameObject.FindObjectOfType<Board>().HandsCount;
            int nbhands = 1 * handcount;


            if (Board.Instance.StartStation != this)
            {
                this.HandsLeft = new Hand[nbhands];
                for (int j = 0; j < this.HandsLeft.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                    newHand.transform.localPosition = new Vector3(0, (j * (canvascardheight / (float)nbhands)) + (canvascardheight * 0.5f / nbhands), 0);
                    newHand.transform.Rotate(new Vector3(0, 0, 180));
                    newHand.Index = j;
                    newHand.LeftHand = true;
                    newHand.name = "LEFT HAND #" + j;
                    this.HandsLeft[j] = newHand;
                }
            }
            if (Board.Instance.EndStation != this)
            {
                this.HandsRight = new Hand[nbhands];
                for (int j = 0; j < this.HandsRight.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                    newHand.transform.localPosition = new Vector3(canvascardhlength, (j * (canvascardheight / (float)nbhands)) + (canvascardheight * 0.5f / nbhands), 0);
                    newHand.Index = j;
                    newHand.LeftHand = false;
                    newHand.name = "RIGHT HAND #" + j;
                    this.HandsRight[j] = newHand;
                }
            }
        }

        public override void RegenerateCardsspace()
        {
            
        }

    }
}
