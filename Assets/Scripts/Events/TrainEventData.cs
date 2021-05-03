using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrainEventData
{
    public delegate void Event(TrainEventData data);

    public Train Train;
    public Train OnTrain;
    public Hand OnHand;

    public TrainEventData(Train train, Train onTrain, Hand onHand)
    {
        Train = train;
        OnTrain = onTrain;
        OnHand = onHand;
    }
}