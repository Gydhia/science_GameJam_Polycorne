using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Box : TrainHandler
    {
        public BoxSO BoxSO;
        public BoxSO PreviousBoxSO;

        public UnityEngine.UI.Text Label;
        public string CardNameForPlayer = "";
        
        public void Start()
        {
            if (this.BoxSO == null)
                throw new Exception("BoxSO is not defined for this Box");

            if (this.CardSpaces == null)
                this.CardSpaces = new CardSpace[this.BoxSO.CardSpaceLength, this.BoxSO.CardSpaceHeight];

            base.Start();
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

                    //hands init
                    this.RegenerateHands();

                    //cardspace init
                    this.RegenerateCardsspace();

                    this.PreviousBoxSO = this.BoxSO;
                }
            }
        }

        public override void RegenerateHands()
        {
            if (this.HandsContainer == null)
                return;

            float pixerperunit = GameObject.FindObjectOfType<Board>().UICanvas.referencePixelsPerUnit;
            float canvascardheight = this.BoxSO.CardSpaceHeight * pixerperunit;
            float canvascardhlength = this.BoxSO.CardSpaceLength * pixerperunit;

            foreach (Transform child in this.HandsContainer)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    GameObject.DestroyImmediate(child.gameObject);

                };
            }

            int handcount = GameObject.FindObjectOfType<Board>().HandsCount;
            int nbhands = this.BoxSO.CardSpaceHeight * handcount;


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
            if (this.CardSpaceContainer == null)
                return;

            float pixerperunit = GameObject.FindObjectOfType<Board>().UICanvas.referencePixelsPerUnit;
            float canvascardheight = this.BoxSO.CardSpaceHeight * pixerperunit;
            float canvascardhlength = this.BoxSO.CardSpaceLength * pixerperunit;


            foreach (Transform child in this.CardSpaceContainer)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    GameObject.DestroyImmediate(child.gameObject);

                };
            }

            this.CardSpaces = new CardSpace[this.BoxSO.CardSpaceLength, this.BoxSO.CardSpaceHeight];
            /*for (int i = 0; i < this.BoxSO.CardSpaceLength; i++)
            {
                for (int j = 0; j < this.BoxSO.CardSpaceHeight; j++)
                {
                    this.CardSpaces[i, j] = new CardSpace();
                    this.CardSpaces[i, j].Card = new Card(this.BoxSO.Cards[i, j]);
                }
            }*/

            for (int k = 0; k < this.BoxSO.CardSpaceLength; k++)
            {
                for (int l = 0; l < this.BoxSO.CardSpaceHeight; l++)
                {
                    CardSpace cardspace = GameObject.Instantiate<CardSpace>(this.CardSpacePrefab, this.CardSpaceContainer);
                    float x = (k * (canvascardhlength / (float)this.BoxSO.CardSpaceLength)) + (canvascardhlength * 0.5f / (float)this.BoxSO.CardSpaceLength);
                    float y = (l * (canvascardheight / (float)this.BoxSO.CardSpaceHeight)) + (canvascardheight * 0.5f / (float)this.BoxSO.CardSpaceHeight);
                    cardspace.transform.localPosition = new Vector3(x, y, 0);
                    cardspace.Box = this;
                    cardspace.positionInBox = new Vector2Int(k, l);
                    this.CardSpaces[k, l] = cardspace;

                }
            }
        }
    }
}
