using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrainNavigationEventData
{
    public delegate void Event(TrainNavigationEventData data);

    public Train Train;
    public Track Tracks;
    public Hand ViaHand;

    public TrainNavigationEventData(Train train, Track tracks, Hand viaHand)
    {
        Train = train;
        Tracks = tracks;
        ViaHand = viaHand;
    }
}