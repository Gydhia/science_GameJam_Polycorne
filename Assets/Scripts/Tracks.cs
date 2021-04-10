using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Tracks : MonoBehaviour
{
    public LineRenderer line;

    public event NavigationNotification OnTrainArrivedAtEnd; // event


    public void Start()
    {
        this.line = this.GetComponent<LineRenderer>();

    }

    internal void StartWatchingTrain(Train train)
    {
        train.OnArrivedAtEndOfTracks += Train_OnArrivedAtEndOfTracks;
    }

    private void Train_OnArrivedAtEndOfTracks(Train Train, Tracks Tracks)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtEnd != null)
            this.OnTrainArrivedAtEnd.Invoke(Train, Tracks);
    }

    internal void StopWatchingTrain(Train train)
    {
        train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtEndOfTracks;
    }
}