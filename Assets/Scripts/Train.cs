using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Train : MonoBehaviour
{
    public LineRenderer currentPath;
    public int lastWaypoint;
    public float percentToNextWaypoint;
    public float speed;

    public void Start()
    {

    }

    public void Update()
    {
        if (this.currentPath != null)
        {
            if (this.lastWaypoint == this.currentPath.positionCount - 1)
                this.ArrivedAtEndOfTracks();
            else if (this.lastWaypoint == this.currentPath.positionCount - 2 && this.percentToNextWaypoint >= 1)
            {
                this.ArrivedAtEndOfTracks();
            }
            else
                this.MoveAlongPath(Time.deltaTime * this.speed);
        }
        if (this.currentPath != null)
        {
            this.transform.position = Vector3.Lerp(this.currentPath.GetPosition(this.lastWaypoint), this.currentPath.GetPosition(this.lastWaypoint + 1), this.percentToNextWaypoint);
        }
    }

    private void ArrivedAtEndOfTracks()
    {
        this.currentPath = null;
        GameObject.Destroy(this.gameObject);
    }

    public void PlaceOnTracks(LineRenderer tracks)
    {
        this.currentPath = tracks;
        this.lastWaypoint = 0;
        this.percentToNextWaypoint = 0;

    }

    public void MoveAlongPath(float distance)
    {
        var lengthTillNextWP = (this.currentPath.GetPosition(this.lastWaypoint + 1) - this.currentPath.GetPosition(this.lastWaypoint)).magnitude;
        this.percentToNextWaypoint += distance / lengthTillNextWP;
        if (this.percentToNextWaypoint >= 1)
        {
            float extralength = (this.percentToNextWaypoint - 1) * lengthTillNextWP;
            this.lastWaypoint++;
            this.percentToNextWaypoint = 0;
            if (this.lastWaypoint < this.currentPath.positionCount - 1)
                this.MoveAlongPath(extralength);
            else
                this.ArrivedAtEndOfTracks();
        }
    }

}
