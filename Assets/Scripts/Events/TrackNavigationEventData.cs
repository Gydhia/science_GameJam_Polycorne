using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrackNavigationEventData
{
    public delegate void Event(TrackNavigationEventData data);

    public Train Train;
    public Track Tracks;
    public Hand ViaHand;

    public TrackNavigationEventData(Train train, Track tracks, Hand viaHand)
    {
        Train = train;
        Tracks = tracks;
        ViaHand = viaHand;
    }
}