using Assets.Scripts;
using ScienceGameJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Card : TrainHandler
{
    /// <summary>
    /// The cardspace the card is located currently. Null if the card is free
    /// </summary>
    public CardSpace CardSpace;

    [Header("Tracks autocreation elements")]
    public Track TrackPrefab;
    public Transform TrackContainer;

    /// <summary>
    /// Define if card can be played by the player. If not, then the card should not be draggable;
    /// </summary>
    public bool IsPlayable = true;

    public override void Start()
    {
        this.AreTracksAutoSnapped = false;
        this.TrainHandlerType = TrainHandlerType.Card;

        if (this.CardSpace == null)
            this.RegisterCardSpace(this.gameObject.GetComponentInParent<CardSpace>());

        this.GenerateTracksOwner();

        Board.Instance.RegisterCard(this);

        base.Start();
    }

    private Connections[] Connections;

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
        Card C = new Card();

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
        Card B = new Card();

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

    protected override void TrainArrivedOrLeave(Train train, Hand hand)
    {
        Hand exit = null;
        if (this.CardSpace != null)
        {
            if (hand.LeftHand)
            {
                if (this.CardSpace.positionInBox.x == 0)
                    exit = this.CardSpace.Box.HandsLeft[hand.Index];
                else
                {
                    var leftCardSpace = this.CardSpace.Box.CardSpaces[this.CardSpace.positionInBox.x - 1, this.CardSpace.positionInBox.y];
                    if (leftCardSpace != null && leftCardSpace.Card != null)
                        exit = leftCardSpace.Card.HandsRight[hand.Index];
                }
            }
            else
            {
                if (this.CardSpace.positionInBox.x == this.CardSpace.Box.CardSpaces.GetLength(0) - 1)
                    exit = this.CardSpace.Box.HandsRight[hand.Index];
                else
                {
                    var rightCardSpace = this.CardSpace.Box.CardSpaces[this.CardSpace.positionInBox.x + 1, this.CardSpace.positionInBox.y];
                    if (rightCardSpace != null && rightCardSpace.Card != null)
                        exit = rightCardSpace.Card.HandsLeft[hand.Index];
                }
            }
        }
        if (exit != null)
            train.PlaceOnHand(exit);
        else
        {
            // WE4RE LEAVING FROM A CARD WITHOUT A BOX; eXIT;
        }

    }

    public void SnapTracks(bool force = false)
    {
        foreach(Hand hand in this.AllHands){
            hand.SnapTrack(force);
        }
    }

    public void GenerateConnectedTracks(bool force = false)
    {
        foreach (Hand hand in this.AllHands)
        {
            hand.GenerateConnectedTrack();
        }

        this.GenerateTracksOwner();
        this.GenerateHandsOwner();
    }

    public void GenerateTracksOwner()
    {
        foreach (Track track in this.TrackContainer.GetComponentsInChildren<Track>())
        {
            track.RegisterTrainHandler(this);
        }
    }

    public override void RegenerateHands()
    {
        if (this.HandsContainer == null)
            return;

        float pixerperunit = Board.Instance.BoardCanvas.referencePixelsPerUnit;
        float canvascardheight = this.gameObject.GetComponent<RectTransform>().rect.height;
        float canvascardhlength = this.gameObject.GetComponent<RectTransform>().rect.height;

#if UNITY_EDITOR
        foreach (Transform child in this.HandsContainer)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                GameObject.DestroyImmediate(child.gameObject);

            };
        }
#endif

        int handcount = Board.Instance.HandsCount;
        int nbhands = handcount;
        int handsoffset = 0;


        if (Board.Instance.StartStation != this)
        {
            this.HandsLeft = new Hand[nbhands];
            for (int j = 0; j < this.HandsLeft.Length; j++)
            {
                Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                newHand.transform.localPosition = new Vector3(0, canvascardheight - ((0.5f + j) * (canvascardheight / (float)(nbhands + handsoffset))), 0);
                newHand.transform.Rotate(new Vector3(0, 0, 180));
                newHand.Index = j;
                newHand.LeftHand = true;
                newHand.name = "LEFT HAND #" + j;
                newHand.RegisterTrainHandler(this);
                this.HandsLeft[j] = newHand;
            }
        }
        if (Board.Instance.IsEndStation(this))
        {
            this.HandsRight = new Hand[nbhands];
            for (int j = 0; j < this.HandsRight.Length; j++)
            {
                Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                newHand.transform.localPosition = new Vector3(canvascardhlength, canvascardheight - ((0.5f + j) * (canvascardheight / (float)(nbhands + handsoffset))), 0);
                newHand.Index = j;
                newHand.LeftHand = false;
                newHand.name = "RIGHT HAND #" + j;
                newHand.RegisterTrainHandler(this);
                this.HandsRight[j] = newHand;
            }
        }
    }

    public void RegenerateTracks()
    {
        if (this.TrackContainer == null)
            return;

        float pixerperunit = Board.Instance.BoardCanvas.referencePixelsPerUnit;
        float canvascardheight = this.gameObject.GetComponent<RectTransform>().rect.height;
        float canvascardhlength = this.gameObject.GetComponent<RectTransform>().rect.height;

#if UNITY_EDITOR
        foreach (Transform child in this.TrackContainer)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                GameObject.DestroyImmediate(child.gameObject);

            };
        }
#endif

        int handcount = Board.Instance.HandsCount;
        int nbhands = handcount;
        int handsoffset = 0;


        if (Board.Instance.StartStation != this)
        {
            this.Tracks = new List<Track>();
            for (int j = 0; j < this.HandsLeft.Length; j++)
            {
                Track newTrack = GameObject.Instantiate<Track>(this.TrackPrefab, this.TrackContainer);
                newTrack.ConnectToHand(this.HandsLeft[j], true);
                newTrack.ConnectToHand(this.HandsRight[j], false);
                newTrack.RegisterTrainHandler(this);
            }
        }
        if (Board.Instance.IsEndStation(this))
        {
            this.HandsRight = new Hand[nbhands];
            for (int j = 0; j < this.HandsRight.Length; j++)
            {
                Hand newHand = GameObject.Instantiate<Hand>(this.HandPrefab, this.HandsContainer);
                newHand.transform.localPosition = new Vector3(canvascardhlength, canvascardheight - ((0.5f + j) * (canvascardheight / (float)(nbhands + handsoffset))), 0);
                newHand.Index = j;
                newHand.LeftHand = false;
                newHand.name = "RIGHT HAND #" + j;
                newHand.RegisterTrainHandler(this);
                this.HandsRight[j] = newHand;
            }
        }
    }

    public void RegisterCardSpace(CardSpace cardspace)
    {
        //if we deregister the cardspace, the, also deregister the card from cardspace
        if (cardspace == null)
        {
            if (this.CardSpace != null)
                this.CardSpace.RegisterCard(null);
        }
        else
        {
            cardspace.RegisterCard(this);
        }

        this.CardSpace = cardspace;
    }

}

internal class Connections
{
    public int IndexStart;
    public int IndexEnd;
    public string StartSide = "left";
    public string EndSide = "right";
}
