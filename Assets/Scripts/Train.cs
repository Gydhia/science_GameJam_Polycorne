using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Train : MonoBehaviour
{
    public Track currentPath;
    public int lastWaypoint;
    public float percentToNextWaypoint;
    public float speed;

    public event NavigationNotification OnArrivedAtEndOfTracks; // event
    public event NavigationNotification OnArrivedAtBeginningOfTracks; // event

    public void Start()
    {

    }

    public void Update()
    {
        if (this.currentPath != null)
        {
            if (this.speed > 0)
            {
                if (this.lastWaypoint == this.currentPath.line.positionCount - 1)
                    this.ArrivedAtEndOfTracks();
                else if (this.lastWaypoint == this.currentPath.line.positionCount - 2 && this.percentToNextWaypoint >= 1)
                {
                    this.ArrivedAtEndOfTracks();
                }
                else
                    this.MoveAlongPath(Time.deltaTime * this.speed);
            }
            else if (this.speed < 0)
            {
                if (this.lastWaypoint == 0)
                    this.ArrivedAtBeginningOfTracks();
                else if (this.lastWaypoint == 1 && this.percentToNextWaypoint >= 1)
                {
                    this.ArrivedAtBeginningOfTracks();
                }
                else
                    this.MoveAlongPath(Time.deltaTime * this.speed);
            }
        }

        if (this.currentPath != null)
        {
            Vector3 positionOnLine;
            if (this.speed > 0)
                positionOnLine = Vector3.Lerp(this.currentPath.line.GetPosition(this.lastWaypoint), this.currentPath.line.GetPosition(this.lastWaypoint + 1), this.percentToNextWaypoint);
            else
                positionOnLine = Vector3.Lerp(this.currentPath.line.GetPosition(this.lastWaypoint - 1), this.currentPath.line.GetPosition(this.lastWaypoint), this.percentToNextWaypoint);

            if (!this.currentPath.line.useWorldSpace)
                positionOnLine += this.currentPath.line.transform.position;
            this.transform.position = positionOnLine;

        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void ArrivedAtEndOfTracks()
    {
        var tracksTheTrainIsLeaving = this.currentPath;
        this.currentPath = null;
        if (this.OnArrivedAtEndOfTracks != null)
            this.OnArrivedAtEndOfTracks.Invoke(this, tracksTheTrainIsLeaving.HandAtEnd);
        /*        this.currentPath.StopWatchingTrain();
                GameObject.Destroy(this.gameObject);*/
    }
    private void ArrivedAtBeginningOfTracks()
    {
        var tracksTheTrainIsLeaving = this.currentPath;
        this.currentPath = null;
        if (this.OnArrivedAtBeginningOfTracks != null)
            this.OnArrivedAtBeginningOfTracks.Invoke(this, tracksTheTrainIsLeaving.HandAtBeginning);
        /*        this.currentPath.StopWatchingTrain();
                GameObject.Destroy(this.gameObject);*/
    }

    public void OnDestroy()
    {

    }

    public void PlaceOnHand(Hand hand)
    {
        if (hand == null || hand.ConnectedTrack == null)
            return;

        if (hand.ConnectEndOfTrack)
        {
            this.currentPath = hand.ConnectedTrack;
            this.lastWaypoint = hand.ConnectedTrack.line.positionCount - 1;
            this.percentToNextWaypoint = 1;
            this.speed = -Math.Abs(this.speed);
            this.currentPath.StartWatchingTrain(this);
        }
        else
        {
            this.currentPath = hand.ConnectedTrack;
            this.lastWaypoint = 0;
            this.percentToNextWaypoint = 0;
            this.speed = Math.Abs(this.speed);
            this.currentPath.StartWatchingTrain(this);
        }
    }

    public void MoveAlongPath(float distance)
    {
        float lengthTillNextWP;
        if (distance > 0)
            lengthTillNextWP = (this.currentPath.line.GetPosition(this.lastWaypoint + 1) - this.currentPath.line.GetPosition(this.lastWaypoint)).magnitude;
        else if (distance < 0)
            lengthTillNextWP = (this.currentPath.line.GetPosition(this.lastWaypoint - 1) - this.currentPath.line.GetPosition(this.lastWaypoint)).magnitude;
        else
            return;

        this.percentToNextWaypoint += distance / lengthTillNextWP;

        if (this.percentToNextWaypoint >= 1)
        {
            float extralength = (this.percentToNextWaypoint - 1) * lengthTillNextWP;
            this.percentToNextWaypoint = 0;
            this.lastWaypoint++;
            if (this.lastWaypoint < this.currentPath.line.positionCount - 1)
                this.MoveAlongPath(extralength);
            else
                this.ArrivedAtEndOfTracks();
        }
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
