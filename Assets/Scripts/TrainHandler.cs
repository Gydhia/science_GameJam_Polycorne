using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class TrainHandler : MonoBehaviour
    {

        /// <summary>
        /// List of space available for cards
        /// </summary>
        public CardSpace[,] CardSpaces;

        /// <summary>
        /// List of space available for cards
        /// </summary>
        public CardSpace CardSpacePrefab;
        public Transform CardSpaceContainer;

        /// <summary>
        /// First half on left
        /// Second half on right
        /// </summary>
        public Hand HandPrefab;
        public Transform HandsContainer;

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

        protected virtual void TrainArrived(Train Train, Hand Hand)
        {
            if (Board.Instance.IsFail(this))
            {
                Train.Animator.SetBool("die", true);
            }
            if (Board.Instance.IsEnd(this))
            {
                Board.Instance.RegisterTrainArrival(Train, Hand);
                if (SoundController.Instance != null) 
                    SoundController.Instance.PlaySound(SoundController.SoundNames.WhispArrival);
            }
            else if (Board.Instance.StartStation == this)
            {
                return;
            }
            else
            {
                if (this.CardSpaces != null &&
                    this.CardSpaces.Length > 0 &&
                    this.CardSpaces[0, 0] != null &&
                    this.CardSpaces[0, 0].Card != null)
                {
                    // choose which card to use if overlap.
                    int y = 0;

                    if (Hand.LeftHand)
                    {
                        if (this.CardSpaces.GetLength(1) > 1)
                        {
                            y = -1;
                            float total_weight = 0;
                            for (int i = 0; i < this.CardSpaces.GetLength(1); i++) total_weight += this.CardSpaces[0, i].OverlapWeight;
                            var result = this.rand.NextDouble() * total_weight;
                            while (result > 0)
                            {
                                y++;
                                result -= this.CardSpaces[0, y].OverlapWeight;
                            }
                        }
                        if (this.CardSpaces[0, y].Card != null)
                            Train.PlaceOnHand(this.CardSpaces[0, y].Card.HandsLeft[Hand.Index]);
                    }
                    else
                    {
                        if (this.CardSpaces.GetLength(1) > 1)
                        {
                            y = -1;
                            float total_weight = 0;
                            for (int i = 0; i < this.CardSpaces.GetLength(1); i++) total_weight += this.CardSpaces[this.CardSpaces.GetLength(0) - 1, i].OverlapWeight;
                            var result = this.rand.NextDouble() * total_weight;
                            while (result > 0)
                            {
                                y++;
                                result -= this.CardSpaces[this.CardSpaces.GetLength(0) - 1, y].OverlapWeight;
                            }
                        }
                        if (this.CardSpaces[this.CardSpaces.GetLength(0) - 1, y].Card != null)
                            Train.PlaceOnHand(this.CardSpaces[this.CardSpaces.GetLength(0) - 1, y].Card.HandsRight[Hand.Index]);
                    }
                }
                else
                {
                    // HERE IS A FAKE BEHAVIOUR:
                    // it should fail, but let's connect across the BOX for now
                    //Hand exit;
                    //if (Hand.LeftHand)
                    //    exit = this.HandsRight.First(h => h.Index == Hand.Index);
                    //else
                    //    exit = this.HandsLeft.First(h => h.Index == Hand.Index);
                    //Train.PlaceOnHand(exit);
                }

            }

        }

        public Hand DecideOutput()
        {
            if (this.OutputDistribution == null || this.OutputDistribution.Count() <= 1)
                // if no distribution was set, no weights: return a random output
                return this.HandsRight.ElementAt(this.rand.Next(0, this.HandsRight.Count()));

            double total_weight = this.OutputDistribution.Sum();
            // if no weight was set, no weights: return a random output
            if (total_weight == 0)
                return this.HandsRight.ElementAt(this.rand.Next(0, this.HandsRight.Count()));

            var result = this.rand.NextDouble() * total_weight;

            int chosen_output = -1;
            while (result > 0)
            {
                chosen_output++;
                result -= this.OutputDistribution.ElementAt(chosen_output);
            }
            return this.HandsRight.ElementAt(chosen_output);
        }

        public abstract void RegenerateHands();

        public abstract void RegenerateCardsspace(bool forceReset = false);
    }
}
