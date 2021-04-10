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

        public void Start()
        {
            if (this.ConnectedTrack != null)
                if (ConnectEndOfTrack)
                    this.ConnectedTrack.line.SetPosition(this.ConnectedTrack.line.positionCount - 1, this.transform.position);
                else
                    this.ConnectedTrack.line.SetPosition(0, this.transform.position);
        }
    }
}
