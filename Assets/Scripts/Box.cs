using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Box : MonoBehaviour
    {
        public BoxSO BoxSO;

        /// <summary>
        /// List of space available for cards
        /// </summary>
        public CardSpace[] CardSpaces;

        /// <summary>
        /// First half on left
        /// Second half on right
        /// </summary>
        public Hand[] HandsLeft;
        public Hand[] HandsRight;
        public Hand HandPrefab;

        public SpriteRenderer SpriteRenderer;
        public Transform Handscontainer;
        public double[] OutputDistribution;
        public System.Random rand;

        public IEnumerable<Hand> AllHands => this.HandsLeft.Concat(this.HandsRight);

        public void Start()
        {
            if (this.BoxSO == null)
                throw new Exception("BoxSO is not defined for this Box");

            this.rand = new System.Random();
            //cards init
            this.CardSpaces = new CardSpace[this.BoxSO.CardSpaceLenght * this.BoxSO.CardSpaceHeight];
            for(int i = 0; i < this.BoxSO.Cards.Length ; i++)
            {
                this.CardSpaces[i] = new CardSpace();
                this.CardSpaces[i].Card = new Card(this.BoxSO.Cards[i]);
            }

            foreach(var hand in this.AllHands)
                if(hand.ConnectEndOfTrack)
                    hand.ConnectedTrack.OnTrainArrivedAtEnd += TrainArrived;

        }

        private void TrainArrived(Train Train, Hand Hand)
        {
            //TODO: register stats about arrived trains

            // HERE IS A FAKE BEHAVIOUR:
            Hand exit = null;
            if (Hand.LeftHand)
                exit = this.HandsRight.First(h => h.Index == Hand.Index);
            else
                exit = this.HandsLeft.First(h => h.Index == Hand.Index);

            if(exit != null)
                Train.PlaceOnHand(exit);
        }

        public void SendManyTrains(int HowMany)
        {
            StartCoroutine(this.sendManyTrains(100));
        }

        private IEnumerator sendManyTrains(int HowMany)
        {
            for (int i = 0; i < HowMany; i++)
            {
                this.SendTrain();
                yield return i / (float)HowMany;
            }
            yield break;
        }

        public void SendTrain()
        {
            var train = GameObject.Instantiate<Train>(Resources.Load<Train>("prefabs/train"));
            int trackID = this.DecideOutput();
            train.PlaceOnHand(this.HandsRight.ElementAt(trackID));
        }

        private int DecideOutput()
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

        public void OnValidate()
        {
            if (this.BoxSO != null)
            {
                float pixerperunit = GameObject.FindObjectOfType<Board>().UICanvas.referencePixelsPerUnit;
                float canvascardwidth = this.BoxSO.CardSpaceHeight * pixerperunit;
                float canvascardhlength = this.BoxSO.CardSpaceLenght * pixerperunit;

                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvascardhlength, canvascardwidth);
                this.SpriteRenderer.size = new Vector2(canvascardhlength, canvascardwidth);

                foreach (Transform child in this.Handscontainer)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        if (Application.isPlaying)
                            GameObject.Destroy(child.gameObject);
                        else
                            GameObject.DestroyImmediate(child.gameObject);
                    };
                }

                //hands init
                int handcount = GameObject.FindObjectOfType<Board>().HandsCount;
                int nbhands = this.BoxSO.CardSpaceHeight * handcount;

                this.HandsLeft = new Hand[nbhands];
                this.HandsRight = new Hand[nbhands];

                for (int j = 0; j < this.HandsLeft.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.Handscontainer);
                    newHand.transform.localPosition = new Vector3(0, (j * (canvascardwidth / (float)nbhands)) + (canvascardwidth * 0.5f / nbhands), 0);
                    newHand.Index = j;
                    newHand.LeftHand = true;
                    newHand.name = "LEFT HAND #" + j;
                    this.HandsLeft[j] = newHand;
                }

                for (int j = 0; j < this.HandsRight.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.Handscontainer);
                    newHand.transform.localPosition = new Vector3(canvascardhlength, (j * (canvascardwidth / (float)nbhands)) + (canvascardwidth * 0.5f / nbhands), 0);
                    newHand.Index = j;
                    newHand.LeftHand = false;
                    newHand.name = "RIGHT HAND #" + j;
                    this.HandsRight[j] = newHand;
                }
            }
        }

    }
}
