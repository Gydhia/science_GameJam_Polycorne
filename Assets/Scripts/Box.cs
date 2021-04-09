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
            this.CardSpaces = new CardSpace[this.BoxSO.TrackLenght * this.BoxSO.TrackWidth];
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
                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.BoxSO.TrackLenght, this.BoxSO.TrackWidth);
                this.SpriteRenderer.size = new Vector2(this.BoxSO.TrackLenght, this.BoxSO.TrackWidth);

                foreach (Transform child in this.Handscontainer)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        GameObject.DestroyImmediate(child.gameObject);

                    };
                }

                //hands init
                this.HandsLeft = new Hand[this.BoxSO.TrackWidth];
                this.HandsRight = new Hand[this.BoxSO.TrackWidth];

                for (int j = 0; j < this.HandsLeft.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.Handscontainer);
                    newHand.transform.localPosition = new Vector3(0, ((this.BoxSO.TrackWidth / this.HandsLeft.Length) * j) + 0.5f, 0);
                    this.HandsLeft[j] = newHand;
                }

                for (int j = 0; j < this.HandsRight.Length; j++)
                {
                    Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.Handscontainer);
                    newHand.transform.localPosition = new Vector3(this.BoxSO.TrackLenght, ((this.BoxSO.TrackWidth / this.HandsRight.Length) * j) + 0.5f, 0);
                    this.HandsRight[j] = newHand;
                }
            }
        }
    }
}
