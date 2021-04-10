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
        train.OnArrivedAtEndOfTracks += Train_OnArrivedAtEndOfTracks;
        train.OnArrivedAtBeginningOfTracks += Train_OnArrivedAtStartOfTracks;
    }

    private void Train_OnArrivedAtEndOfTracks(Train Train, Hand Hand)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtEnd != null)
            this.OnTrainArrivedAtEnd.Invoke(Train, Hand);
    }
    private void Train_OnArrivedAtStartOfTracks(Train Train, Hand Hand)
    {
        this.StopWatchingTrain(Train);
        // forward event to whomever is connected to this track
        if (this.OnTrainArrivedAtStart != null)
            this.OnTrainArrivedAtStart.Invoke(Train, Hand);
    }

    internal void StopWatchingTrain(Train train)
    {
        train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtEndOfTracks;
        train.OnArrivedAtEndOfTracks -= Train_OnArrivedAtStartOfTracks;
    }
}