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

    }
}
