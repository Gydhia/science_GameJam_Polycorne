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
        public BoxSO PreviousBoxSO;

        /// <summary>
        /// List of space available for cards
        /// </summary>
        public CardSpace[,] CardSpaces;
        public CardSpace CardSpacePrefab;
        public Transform CardSpaceContainer;

        /// <summary>
        /// First half on left
        /// Second half on right
        /// </summary>
        public Hand[] HandsLeft;
        public Hand[] HandsRight;
        public Hand HandPrefab;
        public Transform HandsContainer;

        public SpriteRenderer SpriteRenderer;
        public double[] OutputDistribution;
        public System.Random rand;

        public IEnumerable<Hand> AllHands => this.HandsLeft.Concat(this.HandsRight);

        public void Start()
        {
            if (this.BoxSO == null)
                throw new Exception("BoxSO is not defined for this Box");

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

        public void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (this.BoxSO != null && this.BoxSO != this.PreviousBoxSO)
                {
                    float pixerperunit = GameObject.FindObjectOfType<Board>().UICanvas.referencePixelsPerUnit;
                    float canvascardheight = this.BoxSO.CardSpaceHeight * pixerperunit;
                    float canvascardhlength = this.BoxSO.CardSpaceLength * pixerperunit;

                    this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvascardhlength, canvascardheight);
                    this.SpriteRenderer.size = new Vector2(canvascardhlength, canvascardheight);

                    foreach (Transform child in this.HandsContainer)
                    {
                        UnityEditor.EditorApplication.delayCall += () =>
                        {
                            GameObject.DestroyImmediate(child.gameObject);

                        };
                    }

                    //hands init
                    int handcount = GameObject.FindObjectOfType<Board>().HandsCount;
                    int nbhands = this.BoxSO.CardSpaceHeight * handcount;


                    if (Board.Instance.StartStation != this)
                    {
                        this.HandsLeft = new Hand[nbhands];
                        for (int j = 0; j < this.HandsLeft.Length; j++)
                        {
                            Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                            newHand.transform.localPosition = new Vector3(0, (j * (canvascardheight / (float)nbhands)) + (canvascardheight * 0.5f / nbhands), 0);
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

                    //cardspace init
                    if (this.CardSpaceContainer != null)
                    {
                        foreach (Transform child in this.CardSpaceContainer)
                        {
                            UnityEditor.EditorApplication.delayCall += () =>
                            {
                                GameObject.DestroyImmediate(child.gameObject);

                            };
                        }

                        this.CardSpaces = new CardSpace[this.BoxSO.CardSpaceLength, this.BoxSO.CardSpaceHeight];
                        for (int i = 0; i < this.BoxSO.CardSpaceLength; i++)
                        {
                            for (int j = 0; j < this.BoxSO.CardSpaceHeight; j++)
                            {
                                this.CardSpaces[i, j] = new CardSpace();
                                // TEMPORARY: Generate a card from prefab
                                this.CardSpaces[i, j].Card = GameObject.Instantiate(Resources.Load<Card>("prefabs/card"));
                            }
                        }

                        for (int k = 0; k < this.BoxSO.CardSpaceLength; k++)
                        {
                            for (int l = 0; l < this.BoxSO.CardSpaceHeight; l++)
                            {
                                CardSpace cardspace = GameObject.Instantiate<CardSpace>(this.CardSpacePrefab, this.CardSpaceContainer);
                                float x = (k * (canvascardhlength / (float)this.BoxSO.CardSpaceLength)) + (canvascardhlength * 0.5f / (float)this.BoxSO.CardSpaceLength);
                                float y = (l * (canvascardheight / (float)this.BoxSO.CardSpaceHeight)) + (canvascardheight * 0.5f / (float)this.BoxSO.CardSpaceHeight);
                                cardspace.transform.localPosition = new Vector3(x, y, 0);
                                this.CardSpaces[k, l] = cardspace;
                            }
                        }
                    }

                    this.PreviousBoxSO = this.BoxSO;
                }
            }
        }

    }
}
