using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Train : MonoBehaviour
{
    public Tracks currentPath;
    public int lastWaypoint;
    public float percentToNextWaypoint;
    public float speed;

    public event NavigationNotification OnArrivedAtEndOfTracks; // event

    public void Start()
    {

    }

    public void Update()
    {
        if (this.currentPath != null)
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

        if (this.currentPath != null)
        {
            this.transform.position = Vector3.Lerp(this.currentPath.line.GetPosition(this.lastWaypoint), this.currentPath.line.GetPosition(this.lastWaypoint + 1), this.percentToNextWaypoint);
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
            this.OnArrivedAtEndOfTracks.Invoke(this, tracksTheTrainIsLeaving);
/*        this.currentPath.StopWatchingTrain();
        GameObject.Destroy(this.gameObject);*/
    }

    public void OnDestroy()
    {

    }

    public void PlaceOnTracks(Tracks tracks)
    {
        this.currentPath = tracks;
        this.lastWaypoint = 0;
        this.percentToNextWaypoint = 0;
        this.currentPath.StartWatchingTrain(this);
    }

    public void MoveAlongPath(float distance)
    {
        var lengthTillNextWP = (this.currentPath.line.GetPosition(this.lastWaypoint + 1) - this.currentPath.line.GetPosition(this.lastWaypoint)).magnitude;
        this.percentToNextWaypoint += distance / lengthTillNextWP;
        if (this.percentToNextWaypoint >= 1)
        {
            float extralength = (this.percentToNextWaypoint - 1) * lengthTillNextWP;
            this.lastWaypoint++;
            this.percentToNextWaypoint = 0;
            if (this.lastWaypoint < this.currentPath.line.positionCount - 1)
                this.MoveAlongPath(extralength);
            else
                this.ArrivedAtEndOfTracks();
        }
    }

}
