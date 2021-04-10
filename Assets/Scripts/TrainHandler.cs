using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class TrainHandler : MonoBehaviour
    {

        /// <summary>
        /// List of space available for cards
        /// </summary>
        public CardSpace[,] CardSpaces;

        /// <summary>
        /// First half on left
        /// Second half on right
        /// </summary>
        public Hand[] HandsLeft;
        public Hand[] HandsRight;

        public double[] OutputDistribution;
        public System.Random rand;

        public IEnumerable<Hand> AllHands => this.HandsLeft.Concat(this.HandsRight);

        public void Start()
        {
            this.rand = new System.Random();

            foreach (var hand in this.AllHands)
            {
                if (hand.ConnectedTrack != null)
                {
                    if (hand.ConnectEndOfTrack)
                        hand.ConnectedTrack.OnTrainArrivedAtEnd += TrainArrived;
                    else
                        hand.ConnectedTrack.OnTrainArrivedAtStart += TrainArrived;
                }
            }


        }

        private void TrainArrived(Train Train, Hand Hand)
        {
            if (Board.Instance.EndStation == this)
            {
                Board.Instance.RegisterTrainArrival(Train, Hand);
            }
            else
            {

                // HERE IS A FAKE BEHAVIOUR:
                if (this.CardSpaces != null && this.CardSpaces[0, 0] != null)
                {
                    if (Hand.LeftHand)
                        Train.PlaceOnHand(this.CardSpaces[0, 0].Card.HandsLeft[Hand.Index]);
                    else
                        Train.PlaceOnHand(this.CardSpaces[0, 0].Card.HandsRight[Hand.Index]);
                }
                //Hand exit = this.Card.HandLeft[0];

                //if (Hand.LeftHand)
                //    exit = this.HandsRight.First(h => h.Index == Hand.Index);
                //else
                //    exit = this.HandsLeft.First(h => h.Index == Hand.Index);

                //if (exit != null)
                //    Train.PlaceOnHand(exit);
            }

        }

        public int DecideOutput()
        {
            if (this.OutputDistribution == null || this.OutputDistribution.Count() <= 1)
                // if no distribution was set, no weights: return a random output
                return this.rand.Next(0, this.HandsRight.Count());

            double total_weight = this.OutputDistribution.Sum();
            // if no weight was set, no weights: return a random output
            if (total_weight == 0)
                return this.rand.Next(0, this.HandsRight.Count());

            var result = this.rand.NextDouble() * total_weight;

            int chosen_output = -1;
            while (result > 0)
            {
                chosen_output++;
                result -= this.OutputDistribution.ElementAt(chosen_output);
            }
            return chosen_output;
        }
    }
}
