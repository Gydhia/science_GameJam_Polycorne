using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public delegate void NavigationNotification(Train Train, Hand Hand);

public class NavigationEvent
{
    public Train Train;
    public Track Tracks;

    public NavigationEvent(Train train, Track tracks)
    {
        Train = train;
        Tracks = tracks;
    }
}