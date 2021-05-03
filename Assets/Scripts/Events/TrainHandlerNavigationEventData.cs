using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrainHandlerNavigationEventData
{
    public delegate void Event(TrainHandlerNavigationEventData data);

    public Train Train;
    public TrainHandler TrainHandler;
    public Hand ViaHand;

    public TrainHandlerNavigationEventData(Train train, TrainHandler trainHandler, Hand viaHand)
    {
        Train = train;
        TrainHandler = trainHandler;
        ViaHand = viaHand;
    }
}