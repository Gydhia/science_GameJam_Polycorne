using System;
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
        private BoxSO PreviousBoxSO;

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

        public void Start()
        {
            if (this.BoxSO == null)
                throw new Exception("BoxSO is not defined for this Box");            
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

                    this.HandsLeft = new Hand[nbhands];
                    this.HandsRight = new Hand[nbhands];

                    for (int j = 0; j < this.HandsLeft.Length; j++)
                    {
                        Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                        newHand.transform.localPosition = new Vector3(0, (j * (canvascardheight / (float)nbhands)) + (canvascardheight * 0.5f / nbhands), 0);
                        this.HandsLeft[j] = newHand;
                    }

                    for (int j = 0; j < this.HandsRight.Length; j++)
                    {
                        Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                        newHand.transform.localPosition = new Vector3(canvascardhlength, (j * (canvascardheight / (float)nbhands)) + (canvascardheight * 0.5f / nbhands), 0);
                        this.HandsRight[j] = newHand;
                    }


                    //cardspace init
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
                            this.CardSpaces[i, j].Card = new Card(this.BoxSO.Cards[i, j]);
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

                    this.PreviousBoxSO = this.BoxSO;
                }
            }
        }
    }
}
