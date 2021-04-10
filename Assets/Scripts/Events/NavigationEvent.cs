using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public delegate void NavigationNotification(Train Train, Tracks Tracks);

public class NavigationEvent
{
    public Train Train;
    public Tracks Tracks;

    public NavigationEvent(Train train, Tracks tracks)
    {
        Train = train;
        Tracks = tracks;
    }
}