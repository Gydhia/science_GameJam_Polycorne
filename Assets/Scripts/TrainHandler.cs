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
        /// Define if the tracks shoud be auto-snapped to the hands
        /// </summary>
        public bool AreTracksAutoSnapped = false;

        /// <summary>
        /// List of space available for cards
        /// </summary>
        public CardSpace[,] CardSpaces;

        public List<Train> CurrentCrossingTrains;

        [Header("Cardspace autocreation elements")]
        public CardSpace CardSpacePrefab;
        public Transform CardSpaceContainer;

        [Header("Hand autocreation elements")]
        public Hand HandPrefab;
        public Transform HandsContainer;

        [Header("Hands instances")]
        public Hand[] HandsLeft;
        public Hand[] HandsRight;
        public IEnumerable<Hand> AllHands => this.HandsLeft.Concat(this.HandsRight);

        public double[] OutputDistribution;
        public System.Random rand;

        public void Awake()
        {
            this.CurrentCrossingTrains = new List<Train>();

            if (this.HandsLeft == null)
                this.HandsLeft = new Hand[Board.Instance.HandsCount];

            if (this.HandsRight == null)
                this.HandsRight = new Hand[Board.Instance.HandsCount];

            foreach(Hand hand in this.AllHands)
            {
                hand.TrainHandler = this;
            }
        }

        public void Start()
        {
            this.rand = new System.Random();

            foreach (var hand in this.AllHands)
            {
                if (hand.ConnectedTrack != null)
                {
                    if (hand.ConnectEndOfTrack)
                        hand.ConnectedTrack.OnTrainArrivedAtEnd += TrainArrivedOrLeave;
                    else
                        hand.ConnectedTrack.OnTrainArrivedAtStart += TrainArrivedOrLeave;
                }
            }
        }

        protected virtual void TrainArrivedOrLeave(Train train, Hand hand)
        {
            if (this.CurrentCrossingTrains.Contains(train))
            {
                //train leaves the trainholder
                this.CurrentCrossingTrains.Remove(train);
                Debug.Log("Train leaves: " + hand + " - " + this);

            }
            else
            {
                //train arrives in the trainholder
                this.CurrentCrossingTrains.Add(train);
                Debug.Log("Train arrive: " + hand + " - " + this);
            }

            if (Board.Instance.IsFail(this))
            {
                train.Animator.SetBool("die", true);
                train.speedDecreaseOverTimeValue = 5;
            }
            if (Board.Instance.IsEnd(this))
            {
                Board.Instance.RegisterTrainArrival(train, hand, this);
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

                    if (hand.LeftHand)
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
                            train.PlaceOnHand(this.CardSpaces[0, y].Card.HandsLeft[hand.Index]);
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
                            train.PlaceOnHand(this.CardSpaces[this.CardSpaces.GetLength(0) - 1, y].Card.HandsRight[hand.Index]);
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
