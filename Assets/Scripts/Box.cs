using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteAlways]
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
        public GameObject HandPrefab;

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

            //hands init
            this.HandsLeft = new Hand[this.BoxSO.TrackWidth];
            this.HandsRight = new Hand[this.BoxSO.TrackWidth];

            float height = this.transform.localScale.y;
            float width = this.transform.localScale.x;
            for (int j = 0; j < this.HandsLeft.Length; j++)
            {
                GameObject newHand = GameObject.Instantiate(this.HandPrefab, this.transform);
                newHand.transform.localPosition = new Vector3(0, (height / this.HandsLeft.Length) * j, 0);
            }

            for (int j = 0; j < this.HandsRight.Length; j++)
            {
                GameObject newHand = GameObject.Instantiate(this.HandPrefab, this.transform);
                newHand.transform.localPosition = new Vector3(width, (height / this.HandsLeft.Length) * j, 0);
            }

            this.gameObject.transform.localScale = new Vector3(this.BoxSO.TrackLenght, this.BoxSO.TrackWidth);
        }
    }
}
