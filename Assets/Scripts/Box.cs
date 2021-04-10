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

        public void Start()
        {
            if (this.BoxSO == null)
                throw new Exception("BoxSO is not defined for this Box");

            //cards init
            this.CardSpaces = new CardSpace[this.BoxSO.CardSpaceLenght * this.BoxSO.CardSpaceHeight];
            for(int i = 0; i < this.BoxSO.Cards.Length ; i++)
            {
                this.CardSpaces[i] = new CardSpace();
                this.CardSpaces[i].Card = new Card(this.BoxSO.Cards[i]);
            }
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
                    this.HandsLeft[j] = newHand;
                }

                for (int j = 0; j < this.HandsRight.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.Handscontainer);
                    newHand.transform.localPosition = new Vector3(canvascardhlength, (j * (canvascardwidth / (float)nbhands)) + (canvascardwidth * 0.5f / nbhands), 0);
                    this.HandsRight[j] = newHand;
                }
            }
        }
    }
}
