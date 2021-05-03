using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardSpaceNavigationEventData
{
    public delegate void Event(CardSpaceNavigationEventData data);

    public Train Train;
    public CardSpace CardSpace;
    public Hand ViaHand;

    public CardSpaceNavigationEventData(Train train, CardSpace cardSpace, Hand viaHand)
    {
        Train = train;
        CardSpace = cardSpace;
        ViaHand = viaHand;
    }
}