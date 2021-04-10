using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hand : MonoBehaviour
    {
        public Track ConnectedTrack;
        public bool ConnectEndOfTrack = false;
        public int Index;
        public bool LeftHand = false;

        public void Update()
        {
            if (this.ConnectedTrack != null)
            {
                Vector3 handposition;
                if (this.ConnectedTrack.line.useWorldSpace)
                    handposition = this.transform.position;
                else
                    return; //   handposition = this.transform.localPosition;

                if (ConnectEndOfTrack)
                {
                    this.ConnectedTrack.line.SetPosition(this.ConnectedTrack.line.positionCount - 1, handposition);
                    this.ConnectedTrack.HandAtEnd = this;
                }
                else
                {
                    this.ConnectedTrack.line.SetPosition(0, handposition);
                    this.ConnectedTrack.HandAtBeginning = this;
                }
            }
        }
    }
}
