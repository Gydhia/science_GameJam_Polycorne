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
        public TrainHandler TrainHandler;

        public Track ConnectedTrack;
        [SerializeField]
        private Vector3 _worldPosition;
        public bool ConnectEndOfTrack = false;
        public int Index;
        public bool LeftHand = false;

        public void Update()
        {
            this._worldPosition = this.transform.position;
            this.SnapTrack();
            this.GenerateConnectedTrack();
        }

        public void GenerateConnectedTrack()
        {
            if (this.ConnectedTrack != null)
            {
                if (ConnectEndOfTrack)
                {
                    this.ConnectedTrack.HandAtEnd = this;
                }
                else
                {
                    this.ConnectedTrack.HandAtBeginning = this;
                }
            }
        }

        public void SnapTrack(bool force = false)
        {
            if (this.ConnectedTrack != null)
            {
                if (ConnectEndOfTrack)
                {
                    if (this.TrainHandler != null && this.TrainHandler.AreTracksAutoSnapped)
                        this.ConnectedTrack.line.SetPosition(this.ConnectedTrack.line.positionCount - 1, new Vector3(this.transform.position.x, this.transform.position.y, this.ConnectedTrack.line.GetPosition(this.ConnectedTrack.line.positionCount - 1).z));
                }
                else
                {
                    if (this.TrainHandler != null && this.TrainHandler.AreTracksAutoSnapped)
                        this.ConnectedTrack.line.SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y, this.ConnectedTrack.line.GetPosition(0).z));
                }
            }
        }
    }
}
