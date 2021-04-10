using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Hand[] Left;
    public Hand[] Right;
    private Connections[] Connections;

    public Card(CardSO cardSO)
    {

    }

    //       A                B
    //  [2]--  --[2]   [2]---------[2]
    //      |  |        
    //  [1]--  --[1]   [1]---------[1]
    //                  
    //  [0]------[0]   [0]---------[0]
    //
    //         C   (multiply)
    //  [2]--  --------------------[2]
    //      |  |        
    //  [1]--  --------------------[1]
    //                  
    //  [0]------------------------[0]
    public static Card GenerateMultiplyCards(Card A, Card B)
    {
        int cpt = 0;
        Card C = new Card(new CardSO());

        foreach (var connection in A.Connections)
        {
            C.Connections[cpt].IndexStart = connection.IndexStart; 
            C.Connections[cpt].StartSide = connection.StartSide;

            C.Connections[cpt].IndexEnd = connection.IndexEnd;
            C.Connections[cpt].EndSide = connection.EndSide;

            cpt += 1;
        }

        return C;
    }

    //        A             B
    //  [2]--   /[2]   [2]\   --[2]
    //      |  /           \  |
    //  [1]-- /--[1] = [1]-- \--[1]
    //      /  |            | \
    //  [0]/   --[0]   [0]--   \[0]
    public static Card GenerateFlippedCard(Card A)
    {
        int cpt = 0;
        Card B = new Card(new CardSO());

        foreach (var connection in A.Connections)
        {
            B.Connections[cpt].IndexStart = connection.IndexEnd;
            B.Connections[cpt].StartSide = connection.StartSide;

            B.Connections[cpt].IndexEnd = connection.IndexStart;
            B.Connections[cpt].EndSide = connection.EndSide;

            cpt += 1;
        }

        return B;
    }
}

internal class Connections
{
    public int IndexStart;
    public int IndexEnd;
    public string StartSide = "left";
    public string EndSide = "right";
}
