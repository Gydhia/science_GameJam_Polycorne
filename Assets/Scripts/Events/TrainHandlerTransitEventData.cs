using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrainHandlerTransitEventData
{
    public delegate void Event(TrainHandlerTransitEventData data);

    public Train Train;
    public TrainHandler TrainHandler;
    public Hand FromHand;
    public Hand ToHand;

    public TrainHandlerTransitEventData(Train train, TrainHandler trainHandler, Hand fromHand, Hand toHand)
    {
        Train = train;
        TrainHandler = trainHandler;
        FromHand = fromHand;
        ToHand = toHand;
    }
}