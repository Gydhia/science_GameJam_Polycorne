using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteAlways]
    [CreateAssetMenu(menuName = "Box")]
    public class BoxSO : ScriptableObject
    {
        public int CardSpaceLength;
        public int CardSpaceHeight;
        public CardSO[,] Cards;

        public void Awake()
        {
            this.Cards = new CardSO[this.CardSpaceLength, this.CardSpaceHeight];
        }

        public void OnValidate()
        {
            this.Cards = new CardSO[this.CardSpaceLength,this.CardSpaceHeight];
        }
    }
}
