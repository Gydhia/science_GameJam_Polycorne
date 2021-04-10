using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Track : MonoBehaviour
{
    public LineRenderer line { get; set; }

    public Hand HandAtBeginning { get; set; }
    public Hand HandAtEnd { get; set; }

    public event NavigationNotification OnTrainArrivedAtEnd; // event
    public event NavigationNotification OnTrainArrivedAtStart; // event


    public void Start()
    {
        this.line = this.GetComponent<LineRenderer>();

    }

    internal void StartWatchingTrain(Train train)
    {
        //train.OnArrivedAtEndOfTracks += Train_OnArrivedAtEndOfTracks;
        //train.OnArrivedAtBeginningOfTracks += Train_OnArrivedAtStartOfTracks;
    }

    public void Train_OnArrivedAtEndOfTracks(Train Train)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtEnd != null)
            this.OnTrainArrivedAtEnd.Invoke(Train, this.HandAtEnd);
    }
    public void Train_OnArrivedAtStartOfTracks(Train Train)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtStart != null)
            this.OnTrainArrivedAtStart.Invoke(Train, this.HandAtBeginning);
    }

    internal void StopWatchingTrain(Train train)
    {
        //train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtEndOfTracks;
        //train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtStartOfTracks;
    }
}