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

        public bool Refresh;

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

        [ExecuteAlways]
        public void Update()
        {
            if (this.Refresh)
            {
                if (this.BoxSO != null)
                {
                    foreach (Transform child in this.gameObject.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    //hands init
                    this.HandsLeft = new Hand[this.BoxSO.TrackWidth];
                    this.HandsRight = new Hand[this.BoxSO.TrackWidth];

                    float height = this.GetComponent<SpriteRenderer>().bounds.size.x;
                    float width = this.GetComponent<SpriteRenderer>().bounds.size.y;
                    for (int j = 0; j < this.HandsLeft.Length; j++)
                    {
                        Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.transform);
                        newHand.transform.localPosition = new Vector3(0, (height / this.HandsLeft.Length) * j, 0);
                        this.HandsLeft[j] = newHand;
                    }

                    for (int j = 0; j < this.HandsRight.Length; j++)
                    {
                        Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.transform);
                        newHand.transform.localPosition = new Vector3(width, (height / this.HandsRight.Length) * j, 0);
                        this.HandsRight[j] = newHand;
                    }
                }
                this.Refresh = false;
            }
        }

        public void OnValidate()
        {
            this.Refresh = true;

            if (this.BoxSO != null)
            {
                this.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(this.BoxSO.TrackWidth, this.BoxSO.TrackLenght);
            }
        }
    }
}
