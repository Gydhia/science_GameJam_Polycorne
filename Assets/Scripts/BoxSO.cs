using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteAlways]
    [CreateAssetMenu(menuName = "Box")]
    public class BoxSO : ScriptableObject
    {
        public int TrackLenght;
        public int TrackWidth;
        public CardSO[] Cards;

        public void Awake()
        {
            this.Cards = new CardSO[this.TrackLenght * this.TrackWidth];
        }
    }
}
