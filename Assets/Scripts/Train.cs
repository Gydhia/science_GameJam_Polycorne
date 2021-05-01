using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Train : MonoBehaviour
{
    public Guid GUID;

    public Track currentTrack;
    public TrainHandler currentTrainHandler;

    public int lastWaypoint;
    public float percentToNextWaypoint;
    public float speed;
    public float speedDecreaseOverTimeValue = 0;

    public event NavigationNotification OnArrivedAtEndOfTracks; // event
    public event NavigationNotification OnArrivedAtBeginningOfTracks; // event

    public Animator Animator;

    public void Start()
    {
        this.GUID = System.Guid.NewGuid();
    }

    public void Update()
    {
        if (this.currentTrack != null)
        {
            //Debug.Log(this.currentTrack + " " + this.lastWaypoint + " " + this.currentTrack.line.positionCount);
            if (this.speed > 0)
            {
                if (this.lastWaypoint == this.currentTrack.line.positionCount - 1)
                    this.ArrivedAtEndOfTracks();
                else if (this.lastWaypoint == this.currentTrack.line.positionCount - 2 && this.percentToNextWaypoint >= 1)
                {
                    this.ArrivedAtEndOfTracks();
                }
                else
                    this.MoveAlongPath(Time.deltaTime * this.speed);

                /*if(this.speedDecreaseOverTimeValue > 0 && this.speed > 100)
                {
                    this.speed -= this.speedDecreaseOverTimeValue;
                }*/
            }
            //ERROR: cannot reach this one as the value is already greater than 0 when the next frame execute this code
            else if (this.speed < 0)
            {
                if (this.lastWaypoint == 0)
                    this.ArrivedAtBeginningOfTracks();
                else if (this.lastWaypoint == 1 && this.percentToNextWaypoint <= 0)
                {
                    this.ArrivedAtBeginningOfTracks();
                }
                else
                    this.MoveAlongPath(Time.deltaTime * this.speed);
            }
        }

        if (this.currentTrack != null)
        {
            Vector3 positionOnLine;
            if (this.speed > 0)
                positionOnLine = Vector3.Lerp(this.currentTrack.line.GetPosition(this.lastWaypoint), this.currentTrack.line.GetPosition(this.lastWaypoint + 1), this.percentToNextWaypoint);
            else
                positionOnLine = Vector3.Lerp(this.currentTrack.line.GetPosition(this.lastWaypoint - 1), this.currentTrack.line.GetPosition(this.lastWaypoint), this.percentToNextWaypoint);

            if (!this.currentTrack.line.useWorldSpace)
                positionOnLine += this.currentTrack.line.transform.position;
            this.transform.position = positionOnLine;

        }
        else
        {
            if(SoundController.Instance != null)
                SoundController.Instance.PlaySound(SoundController.SoundNames.WhispCrash);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void ArrivedAtEndOfTracks()
    {
        Track tracksTheTrainIsLeaving = this.currentTrack;
        this.currentTrack = null;
        //Debug.Log("ArrivedAtEndOfTracks " + tracksTheTrainIsLeaving + " " + tracksTheTrainIsLeaving.TrainHandler);
        tracksTheTrainIsLeaving.Train_OnArrivedAtEndOfTracks(this);


        //if (this.OnArrivedAtEndOfTracks != null)
        //this.OnArrivedAtEndOfTracks.Invoke(this, tracksTheTrainIsLeaving.HandAtEnd);
        /*        this.currentPath.StopWatchingTrain();
                GameObject.Destroy(this.gameObject);*/
    }
    private void ArrivedAtBeginningOfTracks()
    {
        Track tracksTheTrainIsLeaving = this.currentTrack;
        this.currentTrack = null;
        //Debug.Log("ArrivedAtBeginningOfTracks " + tracksTheTrainIsLeaving + " " + tracksTheTrainIsLeaving.TrainHandler);
        tracksTheTrainIsLeaving.Train_OnArrivedAtStartOfTracks(this);
        //if (this.OnArrivedAtBeginningOfTracks != null)
        //this.OnArrivedAtBeginningOfTracks.Invoke(this, tracksTheTrainIsLeaving.HandAtBeginning);
        /*        this.currentPath.StopWatchingTrain();
                GameObject.Destroy(this.gameObject);*/
    }

    public void OnDestroy()
    {
        Board.Instance.Trains.Remove(this);
    }

    public void PlaceOnHand(Hand hand)
    {
        if (hand == null || hand.ConnectedTrack == null)
            return;

        //if the train is placed on a hand, than it means that it arrives on the train handler of the hand
        hand.TrainHandler.TrainEnter(this);
        hand.ConnectedTrack.CurrentCrossingTrains.Add(this);
        //Debug.Log("Train arrives: " + hand + " - " + hand.TrainHandler);

        if (hand.ConnectEndOfTrack)
        {
            this.currentTrack = hand.ConnectedTrack;
            this.lastWaypoint = hand.ConnectedTrack.line.positionCount - 1;
            this.percentToNextWaypoint = 1;
            this.speed = -Math.Abs(this.speed);
            //this.currentPath.StartWatchingTrain(this);
        }
        else
        {
            this.currentTrack = hand.ConnectedTrack;
            this.lastWaypoint = 0;
            this.percentToNextWaypoint = 0;
            this.speed = Math.Abs(this.speed);
            //this.currentPath.StartWatchingTrain(this);
        }
    }

    public void MoveAlongPath(float distance)
    {
        float lengthTillNextWP;
        if (distance > 0)
            lengthTillNextWP = (this.currentTrack.line.GetPosition(this.lastWaypoint + 1) - this.currentTrack.line.GetPosition(this.lastWaypoint)).magnitude;
        else if (distance < 0)
            lengthTillNextWP = (this.currentTrack.line.GetPosition(this.lastWaypoint - 1) - this.currentTrack.line.GetPosition(this.lastWaypoint)).magnitude;
        else
            return;

        this.percentToNextWaypoint += distance / lengthTillNextWP;

        //Debug.Log("MoveAlongPath:" + this.percentToNextWaypoint);

        if (this.percentToNextWaypoint >= 1)
        {
            float extralength = (this.percentToNextWaypoint - 1) * lengthTillNextWP;
            this.percentToNextWaypoint = 0;
            this.lastWaypoint++;
            if (this.lastWaypoint < this.currentTrack.line.positionCount - 1)
                this.MoveAlongPath(extralength);
            else
                this.ArrivedAtEndOfTracks();
        }
        //ERROR cannot reached this condtion as the train is always greater than 0
        else if (this.percentToNextWaypoint <= 0)
        {
            float extralength = (this.percentToNextWaypoint) * lengthTillNextWP;
            this.percentToNextWaypoint = 1;
            this.lastWaypoint--;
            if (this.lastWaypoint > 0)
                this.MoveAlongPath(extralength);
            else
                this.ArrivedAtBeginningOfTracks();
        }

        //if (this.percentToNextWaypoint >= 1)
        //{
        //    float extralength = (this.percentToNextWaypoint - 1) * lengthTillNextWP;
        //    this.percentToNextWaypoint = 0;
        //    if (this.speed > 0)
        //    {
        //        this.lastWaypoint++;
        //        if (this.lastWaypoint < this.currentPath.line.positionCount - 1)
        //            this.MoveAlongPath(extralength);
        //        else
        //            this.ArrivedAtEndOfTracks();
        //    }
        //    else if (this.speed < 0)
        //    {
        //        this.lastWaypoint--;
        //        if (this.lastWaypoint > 0)
        //            this.MoveAlongPath(extralength);
        //        else
        //            this.ArrivedAtBeginningOfTracks();
        //    }
        //}
    }
}
