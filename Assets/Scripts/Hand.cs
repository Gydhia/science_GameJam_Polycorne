using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteAlways]
    public class Hand : MonoBehaviour
    {
        private Card _card;
        public Track ConnectedTrack;
        [SerializeField]
        private Vector3 _worldPosition;
        public bool ConnectEndOfTrack = false;
        public int Index;
        public bool LeftHand = false;

        public void Start()
        {
            this._card = this.transform.parent.transform.parent.GetComponent<Card>();
            this._worldPosition = this.transform.position;
        }

        public void Update()
        {
            this._worldPosition = this.transform.position;
            if(this._card == null || Application.isPlaying)
            {
                this.SnapTrack();
            }
        }

        public void SnapTrack()
        {
            if (this.ConnectedTrack != null)
            {
                if (ConnectEndOfTrack)
                {
                    //if (this.ConnectedTrack.line.useWorldSpace)
                    this.ConnectedTrack.line.SetPosition(this.ConnectedTrack.line.positionCount - 1, new Vector3(this.transform.position.x, this.transform.position.y, this.ConnectedTrack.line.GetPosition(this.ConnectedTrack.line.positionCount - 1).z));
                    this.ConnectedTrack.HandAtEnd = this;
                }
                else
                {
                    //if (this.ConnectedTrack.line.useWorldSpace)
                    this.ConnectedTrack.line.SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y, this.ConnectedTrack.line.GetPosition(0).z));
                    this.ConnectedTrack.HandAtBeginning = this;
                }
            }
        }
    }
}
