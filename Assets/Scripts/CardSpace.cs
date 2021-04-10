using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteAlways]
    public class CardSpace : MonoBehaviour
    {
        public Card Card;

        [Range(0, 1)]
        public float Padding = 0;
        //public SpriteRenderer SpriteRenderer;

        public Color ColorEmpty = Color.green;
        public Color ColorUsed = Color.blue;


        public void Start()
        {
            if (this.Card != null)
            {
                this.Card.transform.position = this.transform.position;
            }
        }


        public void OnValidate()
        {
            if (!Application.isPlaying)
            {
                float pixerperunit = Board.Instance.UICanvas.referencePixelsPerUnit;
                float canvascardwidth = 1 - Padding;
                float canvascardhlength = 1 - Padding;

                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(canvascardhlength, canvascardwidth);
                //this.SpriteRenderer.size = new Vector2(canvascardhlength, canvascardwidth);
            }
        }



    }
}
